namespace PubNub.Async.Models.Channel
{
    public class Channel
    {
        public string Name { get; }

        public bool Encrypted { get; set; }
        public string Cipher { get; set; }

        public Channel(string name)
        {
            Name = name;
        }
    }
}
