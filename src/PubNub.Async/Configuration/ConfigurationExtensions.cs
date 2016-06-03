using System;
using PubNub.Async.Models.Channel;

namespace PubNub.Async.Configuration
{
	public static class ConfigurationExtensions
	{
		public static PubNubClient ConfigureClient(this PubNubClient client, Action<IPubNubSettings> action)
		{
			action(client.Settings);
			return client;
		}

		public static PubNubClient ConfigureClient(this Channel channel, Action<IPubNubSettings> action)
		{
			return new PubNubClient(channel).ConfigureClient(action);
		}

		public static PubNubClient ConfigureClient(this string channel, Action<IPubNubSettings> action)
		{
			return new PubNubClient(new Channel(channel)).ConfigureClient(action);
		}
	}
}
