using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using PubNub.Async.Configuration;
using PubNub.Async.Extensions;
using PubNub.Async.Models.Subscribe;

namespace PubNub.Async.Services.Subscribe
{
    public class SubscriptionMonitor : ISubscriptionMonitor
    {
        protected static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        
        private CancellationTokenSource CancellationSource { get; set; }

        private ISubscriptionRegistry Subscriptions { get; }

        private IDictionary<string, long> SubscribeTimeTokens { get; }

        private IList<Task> MonitorTasks { get; }

        public SubscriptionMonitor(ISubscriptionRegistry subscriptions)
        {
            Subscriptions = subscriptions;

            SubscribeTimeTokens = new ConcurrentDictionary<string, long>();
            MonitorTasks = new List<Task>();
        }

        public void Register(IPubNubEnvironment environment, long subscribeTimeToken)
        {
            SubscribeTimeTokens[environment.AuthenticationKey] = subscribeTimeToken;
        }

        public async Task Start(IPubNubEnvironment environment)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                if (CancellationSource == null)
                {
                    CancellationSource = new CancellationTokenSource();
                }

                if (!CancellationSource.IsCancellationRequested)
                {
                    var subs = Subscriptions.Get(environment.SubscribeKey);
                    var authSubs = subs.GroupBy(x => x.AuthenticationKey);
                    foreach (var authSub in authSubs)
                    {
                        MonitorTasks.Add(Task.Run(async () =>
                        {
                            while (!CancellationSource.IsCancellationRequested)
                            {
                                await ReceiveMessages(environment, authSub.Key, authSub);
                            }
                        }, CancellationSource.Token));
                    }
                }
            }
            finally
            {
                Mutex.Release();
            }
        }

        public async Task Stop(IPubNubEnvironment environment)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            try
            {
                CancellationSource?.Cancel();
                await Task.WhenAll(MonitorTasks);
            }
            finally
            {
                MonitorTasks.Clear();
                CancellationSource = null;
                Mutex.Release();
            }
        }

        private async Task ReceiveMessages(
            IPubNubEnvironment environment,
            string authenticationKey,
            IEnumerable<Subscription> subscriptions)
        {
            var channelsCsv = string.Join(",", subscriptions.Select(x => x.ChannelName).ToArray());

            var requestUrl = environment.Host
                .AppendPathSegments("v2", "subscribe")
                .AppendPathSegment(environment.SubscribeKey)
                .AppendPathSegment(channelsCsv)
                .AppendPathSegment("0")
                .SetQueryParam("uuid", environment.SessionUuid)
                .SetQueryParam("auth", authenticationKey);

            if (SubscribeTimeTokens.ContainsKey(authenticationKey))
            {
                requestUrl.AppendPathSegment(SubscribeTimeTokens[authenticationKey]);
            }

            try
            {
                var response = await requestUrl
                    .GetAsync(CancellationSource.Token)
                    .ProcessResponse()
                    .ReceiveJson<PubNubSubscribeResponse>();

                SubscribeTimeTokens[authenticationKey] = response.SubscribeTime.TimeToken;

                if (response.Messages.Any())
                {
                    foreach (var message in response.Messages)
                    {
                        Subscriptions.MessageReceived(message);
                    }
                }
            }
            catch (FlurlHttpException ex)
            {
                if (!(ex.InnerException is TaskCanceledException))
                {
                    throw;
                }
            }
        }
    }
}
