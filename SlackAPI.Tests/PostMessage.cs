using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class PostMessage
    {
        private readonly IntegrationFixture fixture;

        public PostMessage(IntegrationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void SimpleMessageDelivery()
        {
            // given
            var client = this.fixture.UserClient;
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
                    this.fixture.Config.TestChannelId,
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
            var client = this.fixture.UserClient;
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
                    this.fixture.Config.TestChannelId,
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
            var client = this.fixture.UserClient;
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
                    this.fixture.Config.TestChannelId,
                    string.Empty,
                    attachments: SlackMother.SomeAttachmentsWithActions);
            }

            // then
            Assert.True(actual.ok, "Error while posting message to channel. ");
        }
    }
}