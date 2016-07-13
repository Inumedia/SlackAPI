using System.Linq;
using IntegrationTest.Configuration;
using IntegrationTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTest
{
    using SlackAPI;

    [TestClass]
    public class Update 
    {
        private readonly Config _config;

        public Update()
        {
            _config = Config.GetConfig();
        }

        [TestMethod]
        public void SimpleUpdate()
        {
            // given
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            var messageId = PostedMessage(client);
            UpdateResponse actual = null;

            // when
            using (var sync = new InSync())
            {
                client.Update(
                    response =>
                    {
                        actual = response;
                        sync.Proceed();
                    },
                    messageId,
                    _config.Slack.TestChannel,
                    "[changed]",
                    attachments: SlackMother.SomeAttachments,
                    as_user: true);
            }

            // then
            Assert.IsTrue(actual.ok, "Error while posting message to channel. ");
            Assert.AreEqual(actual.message.text, "[changed]");
            Assert.AreEqual(actual.message.type, "message");
        }

        private string PostedMessage(SlackSocketClient client)
        {
            string messageId = null;
            using (var sync = new InSync())
            {
                client.PostMessage(
                    response =>
                    {
                        messageId = response.ts;
                        Assert.IsTrue(response.ok, "Error while posting message to channel. ");
                        sync.Proceed();
                    },
                    _config.Slack.TestChannel,
                    "Hi there!",
                    as_user: true);
            }
            return messageId;
        }

        [TestMethod()]
        public void UpdatePresence()
        {
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            using (var sync = new InSync())
            {
                client.EmitPresence((presence) =>
                {
                    presence.AssertOk();
                    sync.Proceed();
                }, Presence.away);
            }
        }
    }
}