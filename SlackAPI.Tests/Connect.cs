using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackAPI;
using System;
using IntegrationTest.Configuration;
using IntegrationTest.Helpers;
using SlackAPI.WebSocketMessages;

namespace IntegrationTest
{
    [TestClass]
    public class Connect
    {
        const string TestText = "Test :D";
        private readonly Config _config;

        public Connect()
        {
            _config = Config.GetConfig();
        }

        [TestMethod]
        public void TestConnectAsUser()
        {
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            Assert.IsTrue(client.IsConnected, "Invalid, doesn't think it's connected.");
        }

        [TestMethod, Ignore]
        public void TestGetAccessToken()
        {
            // assemble
            var clientId = _config.Slack.ClientId;
            var clientSecret = _config.Slack.ClientSecret;
            var authCode = _config.Slack.AuthCode;

            // act
            var accessTokenResponse = GetAccessToken(clientId, clientSecret, "", authCode);
            
            // assert
            Assert.IsNotNull(accessTokenResponse, "accessTokenResponse != null");
            Assert.IsNotNull(accessTokenResponse.bot, "bot != null");
            Assert.IsNotNull(accessTokenResponse.bot.bot_user_id, "bot.user_id != null");
            Assert.IsNotNull(accessTokenResponse.bot.bot_access_token, "bot.bot_access_token != null");
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

        [TestMethod]
        public void TestConnectAsBot()
        {
            var client = ClientHelper.GetClient(_config.Slack.BotAuthToken);
            Assert.IsTrue(client.IsConnected, "Invalid, doesn't think it's connected.");
        }

        [TestMethod]
        public void TestConnectPostAndDelete()
        {
            // given
            SlackSocketClient client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            string channel = _config.Slack.TestChannel;

            // when
            DateTime messageTimestamp = PostMessage(client, channel);
            DeletedResponse deletedResponse = DeleteMessage(client, channel, messageTimestamp);

            // then
            Assert.IsNotNull(deletedResponse, "No response was found");
            Assert.IsTrue(deletedResponse.ok, "Message not deleted!");
            Assert.AreEqual(channel, deletedResponse.channel, "Got invalid channel? Something's not right here...");
            Assert.AreEqual(messageTimestamp, deletedResponse.ts, "Got invalid time stamp? Something's not right here...");
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

            Assert.IsNotNull(sendMessageResponse, "sendMessageResponse != null");
            Assert.AreEqual(TestText, sendMessageResponse.text, "Got invalid returned text, something's not right here...");

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
