using System;
using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using SlackAPI.WebSocketMessages;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class Connect
    {
        const string TestText = "Test :D";
        private readonly IntegrationFixture fixture;

        public Connect(IntegrationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TestConnectAsUser()
        {
            var client = this.fixture.UserClient;
            Assert.True(client.IsConnected, "Invalid, doesn't think it's connected.");
        }

        [Fact]
        public void TestConnectAsBot()
        {
            var client = this.fixture.BotClient;
            Assert.True(client.IsConnected, "Invalid, doesn't think it's connected.");
        }

        [Fact]
        public void TestConnectPostAndDelete()
        {
            // given
            SlackSocketClient client = this.fixture.UserClient;
            string channel = this.fixture.Config.TestChannel;

            // when
            DateTime messageTimestamp = PostMessage(client, channel);
            DeletedResponse deletedResponse = DeleteMessage(client, channel, messageTimestamp);

            // then
            Assert.NotNull(deletedResponse);
            Assert.True(deletedResponse.ok);
            Assert.Equal(channel, deletedResponse.channel);
            Assert.Equal(messageTimestamp, deletedResponse.ts);
        }

        private static DateTime PostMessage(SlackSocketClient client, string channel)
        {
            MessageReceived sendMessageResponse = null;

            using (var sync = new InSync(nameof(SlackSocketClient.SendMessage)))
            {
                client.SendMessage(response =>
                {
                    sendMessageResponse = response;
                    sync.Proceed();
                }, channel, TestText);
            }

            Assert.NotNull(sendMessageResponse);
            Assert.Equal(TestText, sendMessageResponse.text);

            return sendMessageResponse.ts;
        }

        private static DeletedResponse DeleteMessage(SlackSocketClient client, string channel, DateTime messageTimestamp)
        {
            DeletedResponse deletedResponse = null;

            using (var sync = new InSync(nameof(SlackClient.DeleteMessage)))
            {
                client.DeleteMessage(response =>
                {
                    deletedResponse = response;
                    sync.Proceed();
                }, channel, messageTimestamp);
            }

            return deletedResponse;
        }
    }
}
