using PubNub.Async.Configuration;
using PubNub.Push.Async.Services;
using System;

namespace PubNub.Push.Async.Configuration
{
    public static class PubNubPushExtensions
    {
        public static void UsePushNotifications(this IPubNubEnvironment environment)
        {
            var registrar = environment as IRegisterService;
            if (registrar == null)
            {
                throw new InvalidOperationException($"Incompatible Environment: {nameof(environment)} must implement ${typeof(IRegisterService).Name}");
            }

            registrar.Register<IPushService>(client => new PushService(client));
        }
    }
}