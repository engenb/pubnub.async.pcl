using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PubNub.Async.Configuration;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.Subscribe;

namespace PubNub.Async.Services.Subscribe
{
    public class SubscriptionRegistry : ISubscriptionRegistry
    {
        private IDictionary<string, IDictionary<int, Subscription>> Subscriptions { get; }
        
        public SubscriptionRegistry()
        {
            Subscriptions = new ConcurrentDictionary<string, IDictionary<int, Subscription>>();
        }

        public void Register(IPubNubEnvironment environment, Channel channel, MessageReceivedHandler handler)
        {
            var subscription = new Subscription(environment, channel, handler);
            // if we're already subscribed, just add the new handler
            var subHash = subscription.GetHashCode();
            if (!Subscriptions.ContainsKey(environment.SubscribeKey))
            {
                Subscriptions[environment.SubscribeKey] = new ConcurrentDictionary<int, Subscription>();
            }
            var subs = Subscriptions[environment.SubscribeKey];

            if (subs.ContainsKey(subHash))
            {
                subs[subHash].Add(handler);
            }
            else
            {
                // if not, add it
                subs[subHash] = subscription;
            }
        }

        public void Unregister(IPubNubEnvironment environment, Channel channel)
        {
            if (Subscriptions.ContainsKey(environment.SubscribeKey))
            {
                Subscriptions[environment.SubscribeKey].Remove(new Subscription(environment, channel).GetHashCode());
            }
        }

        public IEnumerable<MessageReceivedHandler> MessageHandlers(IPubNubEnvironment environment, Channel channel)
        {
            var subHash = new Subscription(environment, channel).GetHashCode();
            return Subscriptions.ContainsKey(environment.SubscribeKey)
                && Subscriptions[environment.SubscribeKey].ContainsKey(subHash)
                ? Subscriptions[environment.SubscribeKey][subHash].Handlers.ToArray()
                : new MessageReceivedHandler[0];
        }

        public IEnumerable<Subscription> EnvironmentSubscriptions(string subscribeKey)
        {
            return Subscriptions.ContainsKey(subscribeKey)
                ? Subscriptions[subscribeKey].Values
                : new Subscription[0];
        }
    }
}
