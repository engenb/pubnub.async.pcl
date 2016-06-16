using Autofac;
using PubNub.Push.Async.Services;

namespace PubNub.Push.Async.Autofac
{
    public class PubNubPushAsyncModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<PushService>()
                .As<IPushService>();
        }
    }
}