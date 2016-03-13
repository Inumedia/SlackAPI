﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackAPI;
using System;
using System.Threading;
using IntegrationTest.Configuration;
using IntegrationTest.Helpers;
using Polly;
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
            var waiter = new EventWaitHandle(false, EventResetMode.ManualReset);
            MessageReceived sendMessageResponse = null;

            client.SendMessage(response =>
            {
                sendMessageResponse = response;
                waiter.Set();
            }, channel, TestText);

            Policy
                .Handle<AssertFailedException>()
                .WaitAndRetry(15, x => TimeSpan.FromSeconds(0.2), (exception, span) => Console.WriteLine("Retrying in {0} seconds", span.TotalSeconds))
                .Execute(() =>
                {
                    Assert.IsTrue(waiter.WaitOne(), "Still waiting for things to happen...");
                });

            Assert.IsNotNull(sendMessageResponse, "sendMessageResponse != null");
            Assert.AreEqual(TestText, sendMessageResponse.text, "Got invalid returned text, something's not right here...");

            return sendMessageResponse.ts;
        }

        private static DeletedResponse DeleteMessage(SlackSocketClient client, string channel, DateTime messageTimestamp)
        {
            DeletedResponse deletedResponse = null;
            var waiter = new EventWaitHandle(false, EventResetMode.ManualReset);

            client.DeleteMessage(response =>
            {
                deletedResponse = response;
                waiter.Set();
            }, channel, messageTimestamp);

            Policy
                .Handle<AssertFailedException>()
                .WaitAndRetry(15, x => TimeSpan.FromSeconds(0.2), (exception, span) => Console.WriteLine("Retrying in {0} seconds", span.TotalSeconds))
                .Execute(() =>
                {
                    Assert.IsTrue(waiter.WaitOne(), "Still waiting for things to happen...");
                });

            return deletedResponse;
        }
    }
}
