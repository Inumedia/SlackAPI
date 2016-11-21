using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests
{
    public class Update 
    {
        private readonly Config _config;

        public Update()
        {
            _config = Config.GetConfig();
        }

        [Fact]
        public void SimpleUpdate()
        {
            // given
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            var messageId = PostedMessage(client);
            UpdateResponse actual = null;

            // when
            using (var sync = new InSync(nameof(SlackClient.Update)))
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
            Assert.True(actual.ok, "Error while posting message to channel. ");
            Assert.Equal(actual.message.text, "[changed]");
            Assert.Equal(actual.message.type, "message");
        }

        private string PostedMessage(SlackSocketClient client)
        {
            string messageId = null;
            using (var sync = new InSync(nameof(SlackClient.PostMessage)))
            {
                client.PostMessage(
                    response =>
                    {
                        messageId = response.ts;
                        Assert.True(response.ok, "Error while posting message to channel. ");
                        sync.Proceed();
                    },
                    _config.Slack.TestChannel,
                    "Hi there!",
                    as_user: true);
            }
            return messageId;
        }

        [Fact]
        public void UpdatePresence()
        {
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            using (var sync = new InSync(nameof(SlackClient.EmitPresence)))
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