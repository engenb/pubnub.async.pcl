using System;
using System.Linq;
using System.Threading.Tasks;
using PubNub.Async.Configuration;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.Subscribe;

namespace PubNub.Async.Services.Subscribe
{
    public class SubscribeService : ISubscribeService
    {
        private Func<string, string, ISubscriptionMonitor> MonitorFactory { get; }
        private ISubscriptionRegistry Subscriptions { get; }
        
        private IPubNubEnvironment Environment { get; }
        private Channel Channel { get; }

        public SubscribeService(
            IPubNubClient client,
            Func<string, string, ISubscriptionMonitor> monitorFactory,
            ISubscriptionRegistry subscriptions)
        {
            Environment = client.Environment;
            Channel = client.Channel;

            MonitorFactory = monitorFactory;
            Subscriptions = subscriptions;
        }

        public async Task<SubscribeResponse> Subscribe(MessageReceivedHandler handler)
        {
            var monitor = MonitorFactory(Environment.Host, Environment.SubscribeKey);
            await monitor.Stop();

            Subscriptions.Register(Environment, Channel, handler);

            return await monitor.Start();
        }

        public async Task Unsubscribe()
        {
            var monitor = MonitorFactory(Environment.Host, Environment.SubscribeKey);
            await monitor.Stop();

            Subscriptions.Unregister(Environment, Channel);

            if (Subscriptions.EnvironmentSubscriptions(Environment.SubscribeKey).Any())
            {
                await monitor.Start();
            }
        }
    }
}
