using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http.Testing;
using Moq;
using Ploeh.AutoFixture;
using PubNub.Async.Configuration;
using PubNub.Async.Models;
using PubNub.Async.Models.Subscribe;
using PubNub.Async.Services.Access;
using PubNub.Async.Services.Crypto;
using PubNub.Async.Services.Subscribe;
using PubNub.Async.Tests.Common;
using Xunit;

namespace PubNub.Async.Tests.Services.Subscribe
{
    public class SubscribeServiceTests : AbstractTest
    {
        [Fact]
        public async Task Unsubscribe__Given_ConfiguredEnvironmentAndChannel__Then_RemoveChannelSubscription()
        {
            var subscribeKey = Fixture.Create<string>();
            var sessionUuid = Fixture.Create<string>();
            var channelName = Fixture.Create<string>();

            long? stopCalledTicks = null;
            long? startCalledTicks = null;
            long? unregisterCalledTicks = null;

            var channel = new Channel(channelName);
            
            var mockEnv = new Mock<IPubNubEnvironment>();
            mockEnv
                .SetupGet(x => x.Host)
                .Returns("https://pubsub.pubnub.com");
            mockEnv
                .SetupGet(x => x.SubscribeKey)
                .Returns(subscribeKey);
            mockEnv
                .SetupGet(x => x.SessionUuid)
                .Returns(sessionUuid);

            var mockClient = new Mock<IPubNubClient>();
            mockClient
                .SetupGet(x => x.Channel)
                .Returns(channel);
            mockClient
                .SetupGet(x => x.Environment)
                .Returns(mockEnv.Object);

            var mockAccess = new Mock<IAccessManager>();

            var mockMonitor = new Mock<ISubscriptionMonitor>();
            mockMonitor
                .Setup(x => x.Stop(mockEnv.Object))
                .Callback(() => stopCalledTicks = DateTime.UtcNow.Ticks)
                .Returns(Task.FromResult(1));
            mockMonitor
                .Setup(x => x.Start(mockEnv.Object))
                .Callback(() => startCalledTicks = DateTime.UtcNow.Ticks)
                .Returns(Task.FromResult(1));

            var mockRegistry = new Mock<ISubscriptionRegistry>();
            mockRegistry
                .Setup(x => x.Unregister(mockEnv.Object, channel))
                .Callback(() => unregisterCalledTicks = DateTime.UtcNow.Ticks);
            mockRegistry
                .Setup(x => x.Get(subscribeKey))
                .Returns(new Subscription[] {new Subscription<string>(Mock.Of<ICryptoService>(), mockEnv.Object, channel)});

            var subject = new SubscribeService(
                mockClient.Object,
                mockAccess.Object,
                mockMonitor.Object,
                mockRegistry.Object);

            await subject.Unsubscribe();

            mockMonitor.Verify(x => x.Stop(mockEnv.Object), Times.Once);
            mockRegistry.Verify(x => x.Unregister(mockEnv.Object, channel), Times.Once);
            mockMonitor.Verify(x => x.Start(mockEnv.Object), Times.Once);

            Assert.True(stopCalledTicks <= unregisterCalledTicks);
            Assert.True(stopCalledTicks <= startCalledTicks);
            Assert.True(unregisterCalledTicks <= startCalledTicks);
        }
    }
}
