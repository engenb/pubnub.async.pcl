using System;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using PubNub.Async.Configuration;
using PubNub.Async.Extensions;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.Subscribe;
using PubNub.Async.Services.Subscribe;
using PubNub.Async.Tests.Common;
using Xunit;

namespace PubNub.Async.Tests.Services.Subscribe
{
    public class SubscribeServiceTests : AbstractTest
    {
        [Fact]
        public async Task Subscribe__Given_ConfiguredEnvironmentAndChannel__When_MonitorStopped__Then_AddSubscriptionStartMonitor()
        {
            var subscribeKey = Fixture.Create<string>();
            var sessionUuid = Fixture.Create<string>();
            var channelName = Fixture.Create<string>();

            long? stopCalledTicks = null;
            long? startCalledTicks = null;
            long? registerCalledTicks = null;
            
            var channel = new Channel(channelName);

            MessageReceivedHandler handler = args => { };

            var expectedResponse = Fixture.Create<SubscribeResponse>();

            var mockEnv = new Mock<IPubNubEnvironment>();
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

            var mockMonitor = new Mock<ISubscriptionMonitor>();
            mockMonitor
                .Setup(x => x.Stop())
                .Returns(Task.FromResult(1))
                .Callback(() => stopCalledTicks = DateTime.UtcNow.Ticks);
            mockMonitor
                .Setup(x => x.Start())
                .Callback(() => startCalledTicks = DateTime.UtcNow.Ticks)
                .ReturnsAsync(expectedResponse);

            var mockRegistry = new Mock<ISubscriptionRegistry>();
            mockRegistry
                .Setup(x => x.Register(mockEnv.Object, channel, handler))
                .Callback<IPubNubEnvironment, Channel, MessageReceivedHandler>((e, c, h) => registerCalledTicks = DateTime.UtcNow.Ticks);

            var subject = new SubscribeService(mockClient.Object, (s, s1) => mockMonitor.Object, mockRegistry.Object);

            var result = await subject.Subscribe(handler);

            Assert.Same(expectedResponse, result);

            mockMonitor.Verify(x => x.Stop(), Times.Once);
            mockRegistry.Verify(x => x.Register(mockEnv.Object, channel, handler), Times.Once);
            mockMonitor.Verify(x => x.Start(), Times.Once);

            Assert.True(stopCalledTicks < registerCalledTicks);
            Assert.True(stopCalledTicks < startCalledTicks);
            Assert.True(registerCalledTicks < startCalledTicks);
        }
    }
}
