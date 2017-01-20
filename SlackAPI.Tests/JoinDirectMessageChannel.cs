using System;
using System.Linq;
using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class JoinDirectMessageChannel
    {
        private readonly IntegrationFixture fixture;

        public JoinDirectMessageChannel(IntegrationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void ShouldJoinDirectMessageChannel()
        {
            // given
            var client = this.fixture.UserClient;
            JoinDirectMessageChannelResponse actual = null;

            string userName = this.fixture.Config.DirectMessageUser;
            string user = client.Users.First(x => x.name.Equals(userName, StringComparison.OrdinalIgnoreCase)).id;

            // when
            using (var sync = new InSync(nameof(SlackClient.JoinDirectMessageChannel)))
            {
                client.JoinDirectMessageChannel(response =>
                {
                    actual = response;
                    sync.Proceed();;
                }, user);
            }

            // then
            Assert.True(actual.ok, "Error while joining user channel");
            Assert.NotEmpty(actual.channel.id);
        }
    }
}