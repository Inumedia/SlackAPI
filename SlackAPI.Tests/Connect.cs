using System;
using System.Linq;
using System.Net;
using System.Threading;
using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using SlackAPI.WebSocketMessages;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class Connect : IDisposable
    {
        const string TestText = "Test :D";
        private readonly IntegrationFixture fixture;

        private SlackSocketClient slackClient;

        public Connect(IntegrationFixture fixture)
        {
            this.fixture = fixture;

            // Extra wait to mitigate Slack throttling
            Thread.Sleep(2000);
        }

        public void Dispose()
        {
            slackClient?.CloseSocket();
        }

        [Fact]
        public void TestConnectAsUser()
        {
            slackClient = this.fixture.CreateUserClient();
            Assert.True(slackClient.IsConnected, "Invalid, doesn't think it's connected.");
        }

        [Fact]
        public void TestConnectAsBot()
        {
            slackClient = this.fixture.CreateBotClient();
            Assert.True(slackClient.IsConnected, "Invalid, doesn't think it's connected.");
        }

        [Fact]
        public void TestConnectWithWrongProxySettings()
        {
            var proxySettings = new WebProxy { Address = new Uri("http://127.0.0.1:8080")};
            Assert.Throws<InvalidOperationException>(() => this.fixture.CreateUserClient(proxySettings));
            Assert.Throws<InvalidOperationException>(() => this.fixture.CreateBotClient(proxySettings));
        }

        [Fact]
        public void TestConnectPostAndDelete()
        {
            // given
            slackClient = this.fixture.CreateUserClient();
            string channel = this.fixture.Config.TestChannel;

            // when
            DateTime messageTimestamp = PostMessage(slackClient, channel);
            DeletedResponse deletedResponse = DeleteMessage(slackClient, channel, messageTimestamp);

            // then
            Assert.NotNull(deletedResponse);
            Assert.True(deletedResponse.ok);
            Assert.Equal(channel, deletedResponse.channel);
            Assert.Equal(messageTimestamp, deletedResponse.ts);
        }

        [Fact]
        public void TestConnectGetPresenceChanges()
        {
            // Arrange
            int presenceChangesRaisedCount = 0;
            using (var sync = new InSync(nameof(TestConnectGetPresenceChanges), this.fixture.ConnectionTimeout))
            {
                void OnPresenceChanged(SlackSocketClient sender, PresenceChange e)
                {
                    if (++presenceChangesRaisedCount == sender.Users.Count)
                    {
                        sync.Proceed();
                    }
                }

                // Act
                slackClient = this.fixture.CreateUserClient(maintainPresenceChangesStatus: true, presenceChanged: OnPresenceChanged);
            }

            // Assert
            Assert.True(slackClient.Users.All(x => x.presence != null));
        }

        [Fact(Skip = "Not stable on AppVeyor")]
        public void TestManualSubscribePresenceChangeAndManualPresenceChange()
        {
            // Arrange
            slackClient = this.fixture.CreateUserClient();
            using (var sync = new InSync())
            {
                slackClient.OnPresenceChanged += x =>
                {
                    if (x.user == slackClient.MySelf.id)
                    {
                        // Assert
                        sync.Proceed();
                    }
                };

                slackClient.SubscribePresenceChange(slackClient.MySelf.id);
            }

            using (var sync = new InSync())
            {
                slackClient.OnPresenceChanged += x =>
                {
                    if (x is ManualPresenceChange && x.user == slackClient.MySelf.id)
                    {
                        // Assert
                        sync.Proceed();
                    }
                };

                // Act
                slackClient.EmitPresence(x => x.AssertOk(), Presence.away);
                slackClient.EmitPresence(x => x.AssertOk(), Presence.auto);
            }
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

            Assert.NotNull(sendMessageResponse);
            Assert.Equal(TestText, sendMessageResponse.text);

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
