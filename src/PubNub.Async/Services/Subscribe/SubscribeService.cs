using System;
using System.Linq;
using System.Threading.Tasks;
using PubNub.Async.Configuration;
using PubNub.Async.Models;
using PubNub.Async.Models.Subscribe;

namespace PubNub.Async.Services.Subscribe
{
    public class SubscribeService : ISubscribeService
    {
        private ISubscriptionMonitor Monitor { get; }
        private ISubscriptionRegistry Subscriptions { get; }
        
        private IPubNubEnvironment Environment { get; }
        private Channel Channel { get; }

        public SubscribeService(
            IPubNubClient client,
            Func<IPubNubEnvironment, ISubscriptionMonitor> monitorFactory,
            ISubscriptionRegistry subscriptions)
        {
            Environment = client.Environment;
            Channel = client.Channel;

            Monitor = monitorFactory(Environment);
            Subscriptions = subscriptions;
        }

        public async Task<SubscribeResponse> Subscribe<TMessage>(MessageReceivedHandler<TMessage> handler)
        {
            await Monitor.Stop();

            Subscriptions.Register(Environment, Channel, handler);

            return await Monitor.Start();
        }

        public async Task Unsubscribe<TMessage>(MessageReceivedHandler<TMessage> handler)
        {
            await Monitor.Stop();

            Subscriptions.Unregister(Environment, Channel, handler);

            if (Subscriptions.Get(Environment.SubscribeKey).Any())
            {
                await Monitor.Start();
            }
        }

        public async Task Unsubscribe()
        {
            await Monitor.Stop();

            Subscriptions.Unregister(Environment, Channel);

            if (Subscriptions.Get(Environment.SubscribeKey).Any())
            {
                await Monitor.Start();
            }
        }
    }
}
