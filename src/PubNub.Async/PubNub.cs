using System;

namespace PubNub.Async
{
	public class PubNub
	{
		public string SessionUUID { get; private set; }
		
		public string PublishKey { get; }
		public string SubscribeKey { get; }
		public string SecretKey { get; }
		public string CipherKey { get; }
		public bool SslEnabled { get; }

		public PubNub(string publishKey, string subscribeKey) :
			this(publishKey, subscribeKey, "")
		{
		}

		public PubNub(string publishKey, string subscribeKey, string secretKey) :
			this(publishKey, subscribeKey, secretKey, "", true)
		{
		}

		public PubNub(string publishKey, string subscribeKey, string secretKey, string cipherKey, bool sslEnabled)
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

			SessionUUID = Guid.NewGuid().ToString();
		}
	}
}
