using System;
using PubNub.Async.Configuration;

namespace PubNub.Async
{
	public class PubNub
	{
		private static readonly object SettingsLock = new object();

		private static Lazy<IPubNubEnvironment> _environment;
		internal static Lazy<IPubNubEnvironment> InternalEnvironment
		{
			get { return _environment ?? (_environment = new Lazy<IPubNubEnvironment>(() => new DefaultPubNubEnvironment())); }
			set { _environment = value; }
		}

		public static IPubNubEnvironment Environment => InternalEnvironment.Value;

		public static void Configure(Action<IPubNubEnvironment> configureSettings)
		{
			lock (SettingsLock)
			{
				configureSettings(Environment);
			}
		}
	}
}