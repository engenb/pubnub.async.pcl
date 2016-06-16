using Autofac;

namespace PubNub.Push.Async.Autofac
{
    public static class AutofacExtensions
    {
        public static void RegisterPubNubPush(this ContainerBuilder builder)
        {
            builder.RegisterModule<PubNubPushAsyncModule>();
        }
    }
}