using System;
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

        private string Host { get; }
        private string SubscribeKey { get; }

        private ISubscriptionRegistry Subscriptions { get; }
        
        public long? SubscribeTimeToken { get; set; }

        private Task Monitor { get; set; }
        private CancellationTokenSource CancellationSource { get; set; }

        public SubscriptionMonitor(
            IPubNubEnvironment environment,
            ISubscriptionRegistry subscriptions)
        {
            Host = environment.Host;
            SubscribeKey = environment.SubscribeKey;

            Subscriptions = subscriptions;
        }

        public async Task<SubscribeResponse> Start()
        {
            SubscribeResponse ret = null;
            await Mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                if (CancellationSource == null)
                {
                    CancellationSource = new CancellationTokenSource();
                }

                if (!CancellationSource.IsCancellationRequested)
                {
                    var subResponse = await Subscribe();
                    ret = new SubscribeResponse {Success = true};

                    Monitor = Task.Run(async () =>
                    {
                        while (!CancellationSource.IsCancellationRequested)
                        {
                            await ReceiveMessages();
                        }
                    }, CancellationSource.Token);
                }
            }
            finally
            {
                Mutex.Release();
            }
            return ret;
        }

        public async Task Stop()
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            try
            {
                if (CancellationSource != null)
                {
                    CancellationSource.Cancel();
                    await Monitor;
                    SubscribeTimeToken = null;
                }
            }
            finally
            {
                CancellationSource = null;
                Mutex.Release();
            }
        }

        private async Task<PubNubSubscribeResponse> Subscribe()
        {
            try
            {
                var response = await BuildSubscribeUrl()
                    .GetAsync(CancellationSource.Token)
                    .ProcessResponse()
                    .ReceiveJson<PubNubSubscribeResponse>();

                SubscribeTimeToken = response.SubscribeTime.TimeToken;
                return response;
            }
            catch (FlurlHttpException ex)
            {
                if (!(ex.InnerException is TaskCanceledException))
                {
                    throw;
                }
            }
            return null;
        }

        private async Task ReceiveMessages()
        {
            var requestUrl = BuildSubscribeUrl();

            if (SubscribeTimeToken != null)
            {
                requestUrl.AppendPathSegment(SubscribeTimeToken);
            }

            try
            {
                var response = await requestUrl
                    .GetAsync(CancellationSource.Token)
                    .ProcessResponse()
                    .ReceiveJson<PubNubSubscribeResponse>();

                SubscribeTimeToken = response.SubscribeTime.TimeToken;

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

        private Url BuildSubscribeUrl()
        {
            return Host
                .AppendPathSegments("v2", "subscribe")
                .AppendPathSegment(SubscribeKey)
                .AppendPathSegment(Subscriptions.Channels(SubscribeKey))
                .AppendPathSegment("0");
        }
    }
}
