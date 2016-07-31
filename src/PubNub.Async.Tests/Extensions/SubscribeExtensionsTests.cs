using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using PubNub.Async.Configuration;
using PubNub.Async.Extensions;
using PubNub.Async.Models;
using PubNub.Async.Models.Subscribe;
using PubNub.Async.Services.Subscribe;
using PubNub.Async.Tests.Common;
using Xunit;

namespace PubNub.Async.Tests.Extensions
{
	public class SubscribeExtensionsTests : AbstractTest
	{
		[Fact]
		public async Task Subscribe__Given_StringAndHandler__Then_Subscribe()
		{
			var expectedChannelName = Fixture.Create<string>();
			var expectedResult = Fixture.Create<SubscribeResponse>();

			IPubNubClient capturedClient = null;

			MessageReceivedHandler<object> expectedHandler = args => Task.CompletedTask;

			var mockSub = new Mock<ISubscribeService>();
			mockSub
				.Setup(x => x.Subscribe(expectedHandler))
				.ReturnsAsync(expectedResult);

			var mockEnv = new Mock<IPubNubEnvironment>();
			mockEnv
				.Setup(x => x.Resolve<ISubscribeService>(It.IsAny<IPubNubClient>()))
				.Callback<IPubNubClient>(x => capturedClient = x)
				.Returns(mockSub.Object);

			PubNub.InternalEnvironment = new Lazy<IPubNubEnvironment>(() => mockEnv.Object);

			var result = await expectedChannelName.Subscribe(expectedHandler);

			Assert.Equal(expectedChannelName, capturedClient.Channel.Name);
			Assert.Same(expectedResult, result);
		}
		[Fact]
		public async Task Subscribe__Given_ChannelAndHandler__Then_Subscribe()
		{
			var expectedChannelName = Fixture.Create<string>();
			var expectedResult = Fixture.Create<SubscribeResponse>();

			var channel = new Channel(expectedChannelName);

			IPubNubClient capturedClient = null;

			MessageReceivedHandler<object> expectedHandler = args => Task.CompletedTask;

			var mockSub = new Mock<ISubscribeService>();
			mockSub
				.Setup(x => x.Subscribe(expectedHandler))
				.ReturnsAsync(expectedResult);

			var mockEnv = new Mock<IPubNubEnvironment>();
			mockEnv
				.Setup(x => x.Resolve<ISubscribeService>(It.IsAny<IPubNubClient>()))
				.Callback<IPubNubClient>(x => capturedClient = x)
				.Returns(mockSub.Object);

			PubNub.InternalEnvironment = new Lazy<IPubNubEnvironment>(() => mockEnv.Object);

			var result = await channel.Subscribe(expectedHandler);

			Assert.Equal(expectedChannelName, capturedClient.Channel.Name);
			Assert.Same(expectedResult, result);
		}
	}
}
