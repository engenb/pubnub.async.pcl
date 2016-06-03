using PubNub.Async.Configuration;
using PubNub.Async.Models.Channel;

namespace PubNub.Async
{
    public class PubNubClient
	{
	    public IPubNubSettings Settings { get; }

	    public Channel Channel { get; set; }

	    public PubNubClient(Channel channel)
	    {
		    Settings = PubNub.GlobalSettings.Clone();
		    Channel = channel;
	    }

	    public PubNubClient(string channel) : this(new Channel(channel))
	    {
	    }

		public string PrepareUrl()
		{
			return (Settings.SslEnabled
				? "https://"
				: "http://")
				   + Settings.Origin;
		}
	}
}
