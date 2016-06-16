using Flurl.Http.Testing;
using PubNub.Async;
using PubNub.Async.Extensions;
using PubNub.Push.Async.Models;
using PubNub.Push.Async.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PubNub.Push.Async.Tests
{
    public class PushServiceTests
    {
        private PushService CreateSubject(
            IPubNubClient client = null)
        {
            var mockClient = client ??
                "channel"
                    .ConfigurePubNub(c =>
                    {
                        c.SubscribeKey = "subkey";
                        c.SslEnabled = true;
                    });

            return new PushService(mockClient);
        }

        [Fact]
        public async Task Register__Given_NoTokenProvided__Then_ThrowsException()
        {
            var subject = CreateSubject();
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => subject.Register(DeviceType.Android, null));
            Assert.Equal("token", exception.ParamName);
        }

        [Fact]
        public async Task Register__Given_NoBodyReturnedInResponse__Then_ReturnsError()
        {
            var subject = CreateSubject();
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(200, null);
                var response = await subject.Register(DeviceType.Android, "token");
                Assert.NotNull(response.Error);
            }
        }

        [Fact]
        public async Task Register__Given_BodyReturnedInResponse__Then_ReturnsEmptyResponse()
        {
            var subject = CreateSubject();
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(200, new object[] { });
                var response = await subject.Register(DeviceType.Android, "token");
                Assert.Null(response.Error);
            }
        }

        [Fact]
        public async Task Revoke__Given_NoTokenProvided__Then_ThrowsException()
        {
            var subject = CreateSubject();
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => subject.Revoke(DeviceType.Android, null));
            Assert.Equal("token", exception.ParamName);
        }

        [Fact]
        public async Task Revoke__Given_NoBodyReturnedInResponse__Then_ReturnsError()
        {
            var subject = CreateSubject();
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(200, null);
                var response = await subject.Revoke(DeviceType.Android, "token");
                Assert.NotNull(response.Error);
            }
        }

        [Fact]
        public async Task Revoke_Given_BodyReturnedInResponse__Then_ReturnsEmptyResponse()
        {
            var subject = CreateSubject();
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(200, new object[] { });
                var response = await subject.Revoke(DeviceType.Android, "token");
                Assert.Null(response.Error);
            }
        }
    }
}