using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackAPI;
using System;
using System.Threading;
using IntegrationTest.Configuration;
using IntegrationTest.Helpers;

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
            var wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            DateTime ts = DateTime.MinValue;
            SlackAPI.WebSocketMessages.MessageReceived a = null;
            client.SendMessage((resp) =>
            {
                a = resp;
                wait.Set();
            }, channel, TestText);

            Assert.IsTrue(wait.WaitOne(1000), "Took too long for Slack to acknowledge message.");
            
            ts = a.ts;
            Assert.AreEqual(a.text, TestText, "Got invalid returned text, something's not right here...");

            DeletedResponse r = null;
            wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            client.DeleteMessage((resp) =>
            {
                r = resp;
                wait.Set();
            }, channel, ts);

            // then
            Assert.IsTrue(wait.WaitOne(1000), "Took too long for Slack to acknowledge delete.");
            Assert.IsTrue(r.ok, "Message not deleted!");
            Assert.AreEqual(r.channel, channel, "Got invalid channel? Something's not right here...");
            Assert.AreEqual(r.ts, ts, "Got invalid time stamp? Something's not right here...");
        }
    }
}
