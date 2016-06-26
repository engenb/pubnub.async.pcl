using System.Threading.Tasks;
using PubNub.Async.Models.Channel;
using PubNub.Async.Presence.Models;
using PubNub.Async.Presence.Services;

namespace PubNub.Async.Presence.Extensions
{
	public static class PresenceExtensions
	{
		public static async Task<SessionStateResponse<TState>> GetState<TState>(this string channel)
			where TState : class
		{
			return await new PubNubClient(channel)
				.GetState<TState>()
				.ConfigureAwait(false);
		}

		public static async Task<SessionStateResponse<TState>> GetState<TState>(this Channel channel)
			where TState : class
		{
			return await new PubNubClient(channel)
				.GetState<TState>()
				.ConfigureAwait(false);
		}

		public static async Task<SessionStateResponse<TState>> GetState<TState>(this IPubNubClient client)
			where TState : class
		{
			return await PubNub.Environment
				.Resolve<IPresenceService>(client)
				.SessionState<TState>()
				.ConfigureAwait(false);
		}

		public static async Task<SessionStateResponse<TState>> SetState<TState>(this string channel, TState state)
			where TState : class
		{
			return await new PubNubClient(channel)
				.SetState(state)
				.ConfigureAwait(false);
		}

		public static async Task<SessionStateResponse<TState>> SetState<TState>(this Channel channel, TState state)
			where TState : class
		{
			return await new PubNubClient(channel)
				.SetState(state)
				.ConfigureAwait(false);
		}

		public static async Task<SessionStateResponse<TState>> SetState<TState>(this IPubNubClient client, TState state)
			where TState : class
		{
			return await PubNub.Environment
				.Resolve<IPresenceService>(client)
				.SessionState(state)
				.ConfigureAwait(false);
		}
	}
}