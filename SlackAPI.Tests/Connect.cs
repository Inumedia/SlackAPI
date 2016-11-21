using System;
using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using SlackAPI.WebSocketMessages;
using Xunit;

namespace SlackAPI.Tests
{
    public class Connect
    {
        const string TestText = "Test :D";
        private readonly Config _config;

        public Connect()
        {
            _config = Config.GetConfig();
        }

        [Fact]
        public void TestConnectAsUser()
        {
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            Assert.True(client.IsConnected, "Invalid, doesn't think it's connected.");
        }

        [Fact(Skip = "Unable to get a working test with data we have in config.json")]
        public void TestGetAccessToken()
        {
            // assemble
            var clientId = _config.Slack.ClientId;
            var clientSecret = _config.Slack.ClientSecret;
            var authCode = _config.Slack.AuthCode;

            // act
            var accessTokenResponse = GetAccessToken(clientId, clientSecret, "", authCode);
            
            // assert
            Assert.NotNull(accessTokenResponse);
            Assert.NotNull(accessTokenResponse.bot);
            Assert.NotNull(accessTokenResponse.bot.bot_user_id);
            Assert.NotNull(accessTokenResponse.bot.bot_access_token);
        }

        private AccessTokenResponse GetAccessToken(string clientId, string clientSecret, string redirectUri, string authCode)
        {
            AccessTokenResponse accessTokenResponse = null;

            using (var sync = new InSync(nameof(SlackClient.GetAccessToken)))
            {
                SlackClient.GetAccessToken(response =>
                {
                    accessTokenResponse = response;
                    sync.Proceed();

                }, clientId, clientSecret, redirectUri, authCode);
            }

            return accessTokenResponse;
        }

        [Fact]
        public void TestConnectAsBot()
        {
            var client = ClientHelper.GetClient(_config.Slack.BotAuthToken);
            Assert.True(client.IsConnected, "Invalid, doesn't think it's connected.");
        }

        [Fact]
        public void TestConnectPostAndDelete()
        {
            // given
            SlackSocketClient client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            string channel = _config.Slack.TestChannel;

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
