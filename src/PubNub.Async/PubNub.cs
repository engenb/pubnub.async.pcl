using System;
using Flurl;

namespace PubNub.Async
{
	public class PubNub
	{
	    internal string SdkVersion => "PubNub-CSharp-.NET/3.7.1";
        public string Host => "pubsub.pubnub.com";

		public string SessionUuid { get; private set; }
		public string AuthenticationKey { get; set; }

		public string PublishKey { get; }
		public string SubscribeKey { get; }
		public string SecretKey { get; }
		public string CipherKey { get; }
		public bool SslEnabled { get; }
		
		public PubNub(string publishKey, string subscribeKey, string secretKey = null, string sessionId = null, string cipherKey = null, bool sslEnabled = true)
		{
			if (string.IsNullOrWhiteSpace(publishKey)) publishKey = string.Empty;
			if (string.IsNullOrWhiteSpace(subscribeKey)) subscribeKey = string.Empty;
			if (string.IsNullOrWhiteSpace(secretKey)) secretKey = string.Empty;
			if (string.IsNullOrWhiteSpace(cipherKey)) cipherKey = string.Empty;

			PublishKey = publishKey;
			SubscribeKey = subscribeKey;
			SecretKey = secretKey;
			CipherKey = cipherKey;
			SslEnabled = sslEnabled;
            
			SessionUuid = sessionId ?? Guid.NewGuid().ToString();
		}

		internal string PrepareUrl()
		{
			return (SslEnabled
				? "https://"
				: "http://")
			       + Host;
		}
	}
}
