namespace PubNub.Async.Models.Channel
{
    public static class ChannelExtensions
    {
	    public static PubNubClient Encrypted(this string channel)
	    {
		    return new PubNubClient(channel).Encrypted();
	    }

        public static PubNubClient Encrypted(this Channel channel)
        {
			return new PubNubClient(channel).Encrypted();
		}

		public static PubNubClient Encrypted(this PubNubClient client)
		{
			client.Channel.Encrypted = true;
			return client;
		}

		public static PubNubClient EncryptedWith(this string channel, string cipher)
		{
			return new PubNubClient(channel).EncryptedWith(cipher);
		}

		public static PubNubClient EncryptedWith(this Channel channel, string cipher)
        {
			return new PubNubClient(channel).EncryptedWith(cipher);
        }

	    public static PubNubClient EncryptedWith(this PubNubClient client, string cipher)
	    {
		    client.Channel.Encrypted = true;
		    client.Channel.Cipher = cipher;
		    return client;
	    }
    }
}
