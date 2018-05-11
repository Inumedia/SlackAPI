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

            // apparently deleted users do indeed have null values, so if the first user returned is deleted then there are failures.
            var someMember = actual.members.Where(x => !x.deleted).First();
            Assert.NotNull(someMember.id);
            Assert.NotNull(someMember.color);
            Assert.NotNull(someMember.real_name);
            Assert.NotNull(someMember.name);
            Assert.NotNull(someMember.team_id);
            Assert.NotNull(someMember.tz);
            Assert.NotNull(someMember.tz_label);
        }
    }
}