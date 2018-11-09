using System;
using SlackAPI.RPCMessages;
using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using System.Linq;
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
                    this.fixture.Config.TestChannel,
                    "Hi there!");
            }

            // then
            Assert.True(actual.ok, "Error while posting message to channel. ");
            Assert.Equal("Hi there!", actual.message.text);
            Assert.Equal("message", actual.message.type);
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
                    this.fixture.Config.TestChannel,
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
                    this.fixture.Config.TestChannel,
                    string.Empty,
                    attachments: SlackMother.SomeAttachmentsWithActions);
            }

            // then
            Assert.True(actual.ok, "Error while posting message to channel. ");
        }

        [Fact]
        public void PostEphemeralMessage()
        {
            // given
            var client = this.fixture.UserClient;
            PostEphemeralResponse actual = null;

            string userName = this.fixture.Config.DirectMessageUser;
            string userId = client.Users.First(x => x.name.Equals(userName, StringComparison.OrdinalIgnoreCase)).id;

            // when
            using (var sync = new InSync(nameof(SlackClient.PostEphemeralMessage)))
            {
                client.PostEphemeralMessage(
                    response =>
                    {
                        actual = response;
                        sync.Proceed();
                    },
                    this.fixture.Config.TestChannel,
                    "Hi there!",
                    userId);
            }

            // then
            Assert.True(actual.ok, "Error while posting message to channel. ");
            Assert.Null(actual.error);
        }
    }
}