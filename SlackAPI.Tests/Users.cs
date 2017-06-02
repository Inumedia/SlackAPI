using System.Linq;
using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class Users
    {
        private readonly IntegrationFixture fixture;

        public Users(IntegrationFixture fixture)
        {
            this.fixture = fixture;
        }
        [Fact]
        public void UserList()
        {
            var client = this.fixture.UserClient;
            UserListResponse actual = null;
            using (var sync = new InSync(nameof(SlackClient.UserLookup)))
            {
                client.GetUserList(response =>
                {
                    actual = response;
                    sync.Proceed();
                });
            }

            Assert.True(actual.ok, "Error while fetching user list.");
            Assert.True(actual.members.Any());
        }
    }
}