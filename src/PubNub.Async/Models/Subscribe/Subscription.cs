using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PubNub.Async.Configuration;
using PubNub.Async.Services.Crypto;
using PubNub.Async.Services.Subscribe;

namespace PubNub.Async.Models.Subscribe
{
    public abstract class Subscription
    {
        public string SubscribeKey => Environment.SubscribeKey;
        public string AuthenticationKey => Environment.AuthenticationKey;
        public string ChannelName => Channel.Name;
        
        protected IPubNubEnvironment Environment { get; }
        protected Channel Channel { get; }

        protected Subscription(IPubNubEnvironment environment, Channel channel)
        {
            Environment = environment;
            Channel = channel;
        }

        public async Task OnMessageReceived(PubNubSubscribeResponseMessage message)
        {
            if (SubscribeKey == message.SubscribeKey && ChannelName == message.Channel)
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
                   && this.AuthenticationKey == that.AuthenticationKey
                   && this.Channel == that.Channel;
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash*23 + SubscribeKey.GetHashCode();
            hash = hash*23 + AuthenticationKey.GetHashCode();
            hash = hash*23 + Channel.GetHashCode();

            return hash;
        }
    }

    public class Subscription<TMessage> : Subscription
    {
        private ICryptoService Crypto { get; }

        public event MessageReceivedHandler<TMessage> MessageReceived;

        public Subscription(
            ICryptoService crypto,
            IPubNubEnvironment environment,
            Channel channel) :
            base(environment, channel)
        {
            Crypto = crypto;
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
                JToken decryptedMsgJson = null;
                var msg = default(TMessage);

                try
                {
                    if (Channel.Encrypted)
                    {
                        var decrypted = Crypto.Decrypt(Channel.Cipher, message.Data.ToObject<string>());
                        decryptedMsgJson = JToken.Parse(decrypted);
                    }
                    else
                    {
                        decryptedMsgJson = message.Data;
                    }
                    msg = decryptedMsgJson.ToObject<TMessage>();
                }
                catch (Exception)
                {
                    //TODO: warn of decryption failure (wrong cipher?) conversion failure (wrong model?)
                }

                await MessageReceived(new MessageReceivedEventArgs<TMessage>(
                   message.SubscribeKey,
                   message.Channel,
                   message.SessionUuid,
                   message.Processed.TimeToken,
                   message.Data,
                   decryptedMsgJson,
                   msg)).ConfigureAwait(false);
            }
        }
    }

}
