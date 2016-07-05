using System;
using Newtonsoft.Json.Linq;

namespace PubNub.Async.Services.Subscribe
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string SubscribeKey { get; set; }
        public string Channel { get; set; }

        public string SenderSessionUuid { get; set; }
        public long Sent { get; set; }

        public JToken MessageJson { get; set; }
    }
}
