using System.Collections.Generic;
using PubNub.Async.Configuration;
using PubNub.Async.Services.Subscribe;

namespace PubNub.Async.Models.Subscribe
{
    public class Subscription
    {
        public IPubNubEnvironment Environment { get; }
        public Channel.Channel Channel { get; }

        public ISet<MessageReceivedHandler> Handlers { get;}

        public Subscription(IPubNubEnvironment environment, Channel.Channel channel, MessageReceivedHandler handler = null)
        {
            Environment = environment.Clone();
            Channel = channel.Clone();

            Handlers = new HashSet<MessageReceivedHandler>();

            if (handler != null)
            {
                Handlers.Add(handler);
            }
        }

        public void Add(MessageReceivedHandler handler)
        {
            Handlers.Add(handler);
        }

        public void Remove(MessageReceivedHandler handler)
        {
            Handlers.Remove(handler);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Subscription))
            {
                return false;
            }
            var that = (Subscription) obj;
            return this.Environment.SubscribeKey == that.Environment.SubscribeKey
                   && this.Environment.SessionUuid == that.Environment.SessionUuid
                   && this.Channel == that.Channel;
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash*23 + Environment.SubscribeKey.GetHashCode();
            hash = hash*23 + Environment.SessionUuid.GetHashCode();
            hash = hash*23 + Channel.Name.GetHashCode();

            return hash;
        }
    }
}
