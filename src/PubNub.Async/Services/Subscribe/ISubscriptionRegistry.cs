using System.Collections.Generic;
using PubNub.Async.Configuration;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.Subscribe;

namespace PubNub.Async.Services.Subscribe
{
    public interface ISubscriptionRegistry
    {
        void Register(IPubNubEnvironment environment, Channel channel, MessageReceivedHandler handler);
        void Unregister(IPubNubEnvironment environment, Channel channel);
        
        IEnumerable<MessageReceivedHandler> MessageHandlers(IPubNubEnvironment environment, Channel channel);

        IEnumerable<Subscription> EnvironmentSubscriptions(string subscribeKey);
    }
}