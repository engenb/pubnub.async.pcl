using System;
using PubNub.Async.Configuration;

namespace PubNub.Push.Async.Configuration
{
	public static class PubNubPushExtensions
	{
		public static void UsePushNotifications(this IPubNubEnvironment environment)
		{
			var registrar = environment as IRegisterService;
			if(registrar == null) throw new InvalidOperationException($"Incompatible Environment: {nameof(environment)} must implement ${typeof(IRegisterService).Name}");

			//registrar.Register();
		}
	}
}
