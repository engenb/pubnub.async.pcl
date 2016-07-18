using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PubNub.Async.Configuration;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.Subscribe;

namespace PubNub.Async.Services.Subscribe
{
    public class SubscriptionRegistry : ISubscriptionRegistry
    {
        private IDictionary<string, ISet<Subscription>> Subscriptions { get; }

        public SubscriptionRegistry()
        {
            Subscriptions = new ConcurrentDictionary<string, ISet<Subscription>>();
        }

        public string Channels(string subscribeKey)
        {
            return Subscriptions.ContainsKey(subscribeKey)
                ? string.Join(",", Subscriptions[subscribeKey]
                    .Select(x => x.Channel)
                    .ToArray())
                : string.Empty;
        }

        public Subscription[] Get(string subscribeKey)
        {
            return Subscriptions.ContainsKey(subscribeKey)
                ? Subscriptions[subscribeKey].ToArray()
                : new Subscription[0];
        }

        public void Register<TMessage>(IPubNubEnvironment environment, Channel channel, MessageReceivedHandler<TMessage> handler)
        {
            var subscription = new Subscription<TMessage>(environment, channel, handler);

            var subscribeKey = environment.SubscribeKey;
            if (!Subscriptions.ContainsKey(subscribeKey))
            {
                Subscriptions[subscribeKey] = new HashSet<Subscription>();
            }

            Subscriptions[subscribeKey].Add(subscription);
        }

        public void Unregister(IPubNubEnvironment environment, Channel channel)
        {
            var subscribeKey = environment.SubscribeKey;
            if (Subscriptions.ContainsKey(subscribeKey))
            {
                var subs = Subscriptions[subscribeKey];
                var sub = subs.SingleOrDefault(x => x.SubscribeKey == subscribeKey && x.Channel == channel.Name);
                if (sub != null)
                {
                    subs.Remove(sub);
                }
            }
        }

        public async Task MessageReceived(PubNubSubscribeResponseMessage message)
        {
            var subscribeKey = message.SubscribeKey;
            if (Subscriptions.ContainsKey(subscribeKey))
            {
                var subs = Subscriptions[subscribeKey];
                var channelSubs = subs
                    .Where(x => x.Channel == message.Channel)
                    .ToArray();
                foreach (var channelSub in channelSubs)
                {
                    await channelSub.OnMessageReceived(message);
                }
            }
        }
    }
}
