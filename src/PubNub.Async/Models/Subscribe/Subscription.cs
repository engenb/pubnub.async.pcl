using System.Threading.Tasks;
using PubNub.Async.Configuration;
using PubNub.Async.Services.Subscribe;

namespace PubNub.Async.Models.Subscribe
{
    public abstract class Subscription
    {
        public string SubscribeKey { get; }
        public string Channel { get; }

        public bool Encrypted { get; }
        
        protected Subscription(IPubNubEnvironment environment, Channel.Channel channel)
        {
            SubscribeKey = environment.SubscribeKey;
            Channel = channel.Name;
            Encrypted = channel.Encrypted;
        }

        public async Task OnMessageReceived(PubNubSubscribeResponseMessage message)
        {
            if (SubscribeKey == message.SubscribeKey && Channel == message.Channel)
            {
                await ProcessMessage(message).ConfigureAwait(false);
            }
        }

        public abstract Task ProcessMessage(PubNubSubscribeResponseMessage message);

        public override bool Equals(object obj)
        {
            if (!(obj is Subscription))
            {
                return false;
            }
            var that = (Subscription) obj;
            return this.SubscribeKey == that.SubscribeKey
                   && this.Channel == that.Channel;
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash*23 + SubscribeKey.GetHashCode();
            hash = hash*23 + Channel.GetHashCode();

            return hash;
        }
    }

    public class Subscription<TMessage> : Subscription
    {
        public event MessageReceivedHandler<TMessage> MessageReceived;

        public Subscription(
            IPubNubEnvironment environment,
            Channel.Channel channel,
            MessageReceivedHandler<TMessage> handler = null) :
            base(environment, channel)
        {
            if (handler != null)
            {
                MessageReceived += handler;
            }
        }

        public void Add(MessageReceivedHandler<TMessage> handler)
        {
            MessageReceived += handler;
        }

        public void Remove(MessageReceivedHandler<TMessage> handler)
        {
            MessageReceived -= handler;
        }

        public override async Task ProcessMessage(PubNubSubscribeResponseMessage message)
        {
            if (MessageReceived != null)
            {
                await MessageReceived(new MessageReceivedEventArgs<TMessage>
                {
                    SubscribeKey = message.SubscribeKey,
                    SenderSessionUuid = message.SessionUuid,
                    Channel = message.Channel,

                    MessageJson = message.Data,

                    Sent = message.Processed.TimeToken
                }).ConfigureAwait(false);
            }
        }
    }

}
