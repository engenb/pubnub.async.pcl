using System;
using Autofac;
using PubNub.Async.Configuration;

namespace PubNub.Async.Autofac
{
	public class PubNubAutofacBootstrapper : IStartable
	{
		private Lazy<IPubNubEnvironment> LazyEnvironment { get; }

		public PubNubAutofacBootstrapper(Lazy<IPubNubEnvironment> lazyEnvironment)
		{
			LazyEnvironment = lazyEnvironment;
		}

		public void Start()
		{
			var lazySettings = PubNub.InternalEnvironment;
			PubNub.InternalEnvironment = LazyEnvironment;
			if (lazySettings.IsValueCreated)
			{
				var oldSettings = lazySettings.Value;
				var newSettings = PubNub.Environment;
				//copy the Environment over
				newSettings.AuthenticationKey = oldSettings.AuthenticationKey;
				newSettings.CipherKey = oldSettings.CipherKey;
				newSettings.MinutesToTimeout = oldSettings.MinutesToTimeout;
				newSettings.Origin = oldSettings.Origin;
				newSettings.PublishKey = oldSettings.PublishKey;
				newSettings.SdkVersion = oldSettings.SdkVersion;
				newSettings.SecretKey = oldSettings.SecretKey;
				newSettings.SessionUuid = oldSettings.SessionUuid;
				newSettings.SslEnabled = oldSettings.SslEnabled;
				newSettings.SubscribeKey = oldSettings.SubscribeKey;
			}
		}
	}
}
