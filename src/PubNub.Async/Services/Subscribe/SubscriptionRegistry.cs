using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PubNub.Async.Configuration;
using PubNub.Async.Models;
using PubNub.Async.Models.Subscribe;

namespace PubNub.Async.Services.Subscribe
{
    public class SubscriptionRegistry : ISubscriptionRegistry
    {
        private IResolveSubscription ResolveSubscription { get; }

        private IDictionary<string, ISet<Subscription>> Subscriptions { get; }

        public SubscriptionRegistry(IResolveSubscription resolveSubscription)
        {
            ResolveSubscription = resolveSubscription;

            Subscriptions = new ConcurrentDictionary<string, ISet<Subscription>>();
        }

        public string Channels(string subscribeKey)
        {
            return Subscriptions.ContainsKey(subscribeKey)
                ? string.Join(",", Subscriptions[subscribeKey]
                    .Select(x => x.ChannelName)
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
            var subscribeKey = environment.SubscribeKey;
            if (!Subscriptions.ContainsKey(subscribeKey))
            {
                Subscriptions[subscribeKey] = new HashSet<Subscription>();
            }

            var sub = Subscriptions[subscribeKey]
                .SingleOrDefault(x => x.SubscribeKey == subscribeKey && x.ChannelName == channel.Name) as Subscription<TMessage>;

            if (sub == null)
            {
                sub = ResolveSubscription.Resolve<TMessage>(environment, channel);
                Subscriptions[subscribeKey].Add(sub);
            }

            sub.MessageReceived += handler;
        }

        public void Unregister<TMessage>(IPubNubEnvironment environment, Channel channel, MessageReceivedHandler<TMessage> handler)
        {
            var subscribeKey = environment.SubscribeKey;
            if (Subscriptions.ContainsKey(subscribeKey))
            {
                var subs = Subscriptions[subscribeKey];
                var sub = subs.SingleOrDefault(x => x.SubscribeKey == subscribeKey && x.ChannelName == channel.Name) as Subscription<TMessage>;
                if (sub != null)
                {
                    sub.MessageReceived -= handler;
                }
            }
        }

        public void Unregister(IPubNubEnvironment environment, Channel channel)
        {
            var subscribeKey = environment.SubscribeKey;
            if (Subscriptions.ContainsKey(subscribeKey))
            {
                var subs = Subscriptions[subscribeKey];
                var sub = subs.SingleOrDefault(x => x.SubscribeKey == subscribeKey && x.ChannelName == channel.Name);
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
                    .Where(x => x.ChannelName == message.Channel)
                    .ToArray();
                foreach (var channelSub in channelSubs)
                {
                    await channelSub.OnMessageReceived(message);
                }
            }
        }
    }
}
