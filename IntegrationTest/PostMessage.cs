using System;
using System.Linq;
using System.Threading;
using IntegrationTest.Configuration;
using IntegrationTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;

namespace IntegrationTest
{
    using SlackAPI;

    [TestClass]
    public class PostMessage
    {
        private readonly Config _config;

        public PostMessage()
        {
            _config = Config.GetConfig();
        }

        [TestMethod]
        public void SimpleMessageDelivery()
        {
            // given
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            PostMessageResponse actual = null;

            // when
            using (var sync = new InSync())
            {
                client.PostMessage(
                    response =>
                    {
                        actual = response;
                        sync.Proceed();
                    },
                    _config.Slack.TestChannel,
                    "Hi there!");
            }

            // then
            Assert.IsTrue(actual.ok, "Error while posting message to channel. ");
            Assert.AreEqual(actual.message.text, "Hi there!");
            Assert.AreEqual(actual.message.type, "message");
        }

        [TestMethod]
        public void Attachments()
        {
            // given
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            PostMessageResponse actual = null;

            // when
            using (var sync = new InSync())
            {
                client.PostMessage(
                    response =>
                    {
                        actual = response;
                        sync.Proceed();
                    },
                    _config.Slack.TestChannel,
                    string.Empty,
                    attachments: SlackMother.SomeAttachments);
            }

            // then
            Assert.IsTrue(actual.ok, "Error while posting message to channel. ");
        }
    }
}