using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
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
        
        private long? LastTimeToken { get; set; }

        private Task Monitor { get; set; }
        private CancellationTokenSource CancellationSource { get; set; }

        public SubscriptionMonitor(
            string host,
            string subscribeKey,
            ISubscriptionRegistry subscriptions)
        {
            Host = host;
            SubscribeKey = subscribeKey;

            Subscriptions = subscriptions;
        }

        public async Task<SubscribeResponse> Start()
        {
            var ret = new SubscribeResponse();
            await Mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                if (CancellationSource == null)
                {
                    CancellationSource = new CancellationTokenSource();
                    await Task.Run(ReceiveMessages, CancellationSource.Token);
                    if (!CancellationSource.IsCancellationRequested)
                    {
                        Monitor = Task.Run(async () =>
                        {
                            while (!CancellationSource.IsCancellationRequested)
                            {
                                await ReceiveMessages();
                            }
                        }, CancellationSource.Token);
                        ret.Success = true;
                    }
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
                }
            }
            finally
            {
                CancellationSource = null;
                Mutex.Release();
            }
        }

        private async Task ReceiveMessages()
        {
            var envSubscriptions = Subscriptions.EnvironmentSubscriptions(SubscribeKey);
            var channels = string.Join(",", envSubscriptions.Select(x => x.Channel.Name));

            var requestUrl = Host
                .AppendPathSegments("v2", "subscribe")
                .AppendPathSegment(SubscribeKey)
                .AppendPathSegment(channels)
                .AppendPathSegment("0");

            if (LastTimeToken != null)
            {
                requestUrl.AppendPathSegment(LastTimeToken);
            }

            var response = await requestUrl
                .GetAsync(CancellationSource.Token)
                .ProcessResponse()
                .ReceiveJson<PubNubSubscribeResponse>();
            
            LastTimeToken = response.SubscribeTime.TimeToken;

            if (response.Messages.Any())
            {
                foreach (var message in response.Messages)
                {
                    var eventArgs = new MessageReceivedEventArgs
                    {
                        SubscribeKey = message.SubscribeKey,
                        SenderSessionUuid = message.SessionUuid,
                        Channel = message.Channel,
                        Sent = message.Processed.TimeToken,
                        MessageJson = message.Data
                    };

                    var handlers = envSubscriptions
                        .Where(x => x.Environment.SubscribeKey == message.SubscribeKey
                                    && x.Channel.Name == message.Channel)
                        .SelectMany(x => x.Handlers);

                    foreach (var handler in handlers)
                    {
                        handler(eventArgs);
                    }
                }
            }
        }
    }
}
