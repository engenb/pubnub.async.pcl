using System.Threading.Tasks;
using PubNub.Async.Configuration;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.Subscribe;

namespace PubNub.Async.Services.Subscribe
{
    public interface ISubscriptionRegistry
    {
        string Channels(string subscribeKey);
        Subscription[] Get(string subscribeKey);

        void Register<TMessage>(IPubNubEnvironment environment, Channel channel, MessageReceivedHandler<TMessage> handler);
        void Unregister(IPubNubEnvironment environment, Channel channel);

        Task MessageReceived(PubNubSubscribeResponseMessage message);
    }
}