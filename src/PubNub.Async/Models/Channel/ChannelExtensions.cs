namespace PubNub.Async.Models.Channel
{
    public static class ChannelExtensions
    {
        public static Channel Encrypted(this Channel channel)
        {
            channel.Encrypted = true;
            return channel;
        }

        public static Channel EncryptedWith(this Channel channel, string cipher)
        {
            channel.Encrypted = true;
            channel.Cipher = cipher;
            return channel;
        }
    }
}
