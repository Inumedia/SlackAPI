using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using System.Linq;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class PresenceSub
    {
        private readonly IntegrationFixture fixture;

        public PresenceSub(IntegrationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void PresenceSubscribe()
        {
            var client = this.fixture.UserClientWithPresence;
            var directMessageUser = client.Users.FirstOrDefault(x => x.name == this.fixture.Config.DirectMessageUser);
            Assert.NotNull(directMessageUser);

            //UserListResponse actual = null;
            using (var sync = new InSync(nameof(SlackClient.UserLookup)))
            {
                client.OnPresenceChangeReceived += (user) =>
                {

                };
                client.OnUserChangeReceived += (user) =>
                {

                };
                client.SendPresenceSub(new[] { directMessageUser.id });
                sync.Proceed();
            }
        }
    }
}