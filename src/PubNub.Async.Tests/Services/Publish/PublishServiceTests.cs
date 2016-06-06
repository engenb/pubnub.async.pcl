using System;
using System.ComponentModel;
using System.Threading.Tasks;
using PubNub.Async.Extensions;
using PubNub.Async.Models.Channel;
using PubNub.Async.Tests.Properties;
using Xunit;

namespace PubNub.Async.Tests.Services.Publish
{
	public class PublishServiceTests : IDisposable
	{
		public PublishServiceTests()
		{
			PubNub.Configure(c =>
			{
				c.PublishKey = Settings.Default.NoFeaturesPublishKey;
				c.SubscribeKey = Settings.Default.NoFeaturesSubscribeKey;
			});
		}
		
		[Fact(Skip = "PN message limite = 32K, but I think http URL can't even support that length")]
		[Category("integration")]
		public async Task Publish__Given_Message__When_MessageTooLarge__Then_ReturnError()
		{
			var message = new PublishTestMessage
			{
				Message = MessageLargerThan32K.Value
			};

			var response = await Settings.Default.PublishDecryptedChannel
				.Publish(message, false);

			Assert.False(response.Success);
			Assert.Equal("Message Too Large", response.Message);
		}

		[Fact]
		[Category("integration")]
		public async Task Publish__Given_Message__When_NoSecretKeyAndNotEncrypted__Then_Publish()
		{
			var message = new PublishTestMessage
			{
				Message = "Hello World!"
			};

			var response = await Settings.Default.PublishDecryptedChannel
				.Publish(message, false);

			Assert.True(response.Success);
			Assert.Equal("Sent", response.Message);
			Assert.True(response.Sent > 0);
		}

		[Fact]
		[Category("integration")]
		public async Task Publish__Given_Message__When_NoSecretKeyAndEncrypted__Then_Publish()
		{
			var message = new PublishTestMessage
			{
				Message = "Hello World!"
			};

			var response = await Settings.Default.PublishEncryptedChannel
				.EncryptedWith(Settings.Default.CipherKey)
				.Publish(message, false);

			Assert.True(response.Success);
			Assert.Equal("Sent", response.Message);
			Assert.True(response.Sent > 0);
		}

		public class PublishTestMessage
		{
			public string Message { get; set; }
		}

		public void Dispose()
		{
			PubNub.GlobalSettings.Reset();
		}
	}
}
