using System;
using System.Linq;
using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests
{
    public class JoinDirectMessageChannel
    {
        private readonly Config _config;

        public JoinDirectMessageChannel()
        {
            _config = Config.GetConfig();
        }

        [Fact]
        public void ShouldJoinDirectMessageChannel()
        {
            // given
            var client = ClientHelper.GetClient(_config.Slack.UserAuthToken);
            JoinDirectMessageChannelResponse actual = null;

            string userName = _config.Slack.DirectMessageUser;
            string user = client.Users.First(x => x.name.Equals(userName, StringComparison.InvariantCultureIgnoreCase)).id;

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