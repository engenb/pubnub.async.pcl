﻿using System.Runtime.Serialization;
using System.Threading.Tasks;
using PubNub.Async.Models.Channel;
using PubNub.Async.Presence.Services;

namespace PubNub.Async.Presence.Extensions
{
	public static class PresenceExtensions
	{
		public static async Task<TState> GetState<TState>(this string channel)
			where TState : class
		{
			return await new PubNubClient(channel)
				.GetState<TState>()
				.ConfigureAwait(false);
		}

		public static async Task<TState> GetState<TState>(this Channel channel)
			where TState : class
		{
			return await new PubNubClient(channel)
				.GetState<TState>()
				.ConfigureAwait(false);
		}

		public static async Task<TState> GetState<TState>(this PubNubClient client)
			where TState : class
		{
			return await PubNub.Environment
				.Resolve<IPresenceService>(client)
				.GetState<TState>()
				.ConfigureAwait(false);
		}

		public static async Task SetState<TState>(this string channel, TState state)
			where TState : class
		{
			await new PubNubClient(channel)
				.SetState(state)
				.ConfigureAwait(false);
		}

		public static async Task SetState<TState>(this Channel channel, TState state)
			where TState : class
		{
			await new PubNubClient(channel)
				.SetState(state)
				.ConfigureAwait(false);
		}

		public static async Task SetState<TState>(this PubNubClient client, TState state)
			where TState : class
		{
			await PubNub.Environment
				.Resolve<IPresenceService>(client)
				.SetState(state)
				.ConfigureAwait(false);
		}
	}
}
