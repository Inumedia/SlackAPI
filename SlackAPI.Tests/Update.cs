﻿using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using System;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class Update
    {
        private readonly IntegrationFixture fixture;

        public Update(IntegrationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void SimpleUpdate()
        {
            // given
            var client = this.fixture.UserClient;
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
                    this.fixture.Config.TestChannelId,
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
                    this.fixture.Config.TestChannelId,
                    "Hi there!",
                    as_user: true);
            }
            return messageId;
        }

        [Fact]
        public void UpdatePresence()
        {
            var client = this.fixture.UserClient;
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