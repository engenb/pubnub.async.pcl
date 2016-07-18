using System;
using Newtonsoft.Json.Linq;

namespace PubNub.Async.Services.Subscribe
{
    public class MessageReceivedEventArgs<TMessage> : EventArgs
    {
        public string SubscribeKey { get; set; }
        public string Channel { get; set; }

        public string SenderSessionUuid { get; set; }
        public long Sent { get; set; }

        public JToken MessageJson { get; set; }

        private TMessage _message;

        public TMessage Message
        {
            get
            {
                if (_message == null && MessageJson != null)
                {
                    _message = MessageJson.ToObject<TMessage>();
                }
                return _message;
            }
        }
    }
}
