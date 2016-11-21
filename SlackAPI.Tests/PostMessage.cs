using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests
{
    public class PostMessage
    {
        private readonly Config _config;

        public PostMessage()
        {
            _config = Config.GetConfig();
        }

        [Fact]
        public void SimpleMessageDelivery()
        {
            // given
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            PostMessageResponse actual = null;

            // when
            using (var sync = new InSync(nameof(SlackClient.PostMessage)))
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
            Assert.True(actual.ok, "Error while posting message to channel. ");
            Assert.Equal(actual.message.text, "Hi there!");
            Assert.Equal(actual.message.type, "message");
        }

        [Fact]
        public void Attachments()
        {
            // given
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            PostMessageResponse actual = null;

            // when
            using (var sync = new InSync(nameof(SlackClient.PostMessage)))
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
            Assert.True(actual.ok, "Error while posting message to channel. ");
        }

        [Fact]
        public void AttachmentsWithActions()
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
                    attachments: SlackMother.SomeAttachmentsWithActions);
            }

            // then
            Assert.True(actual.ok, "Error while posting message to channel. ");
        }
    }
}