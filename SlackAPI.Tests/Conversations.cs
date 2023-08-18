using System.Linq;
using System.Threading.Tasks;
using SlackAPI.RPCMessages;
using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class Conversations
    {
        private readonly IntegrationFixture fixture;

        public Conversations(IntegrationFixture fixture)
        {
            this.fixture = fixture;
        }
        [Fact]
        public void ConversationList()
        {
            var client = this.fixture.UserClient;
            ConversationsListResponse actual = null;
            using (var sync = new InSync(nameof(SlackClient.ChannelLookup)))
            {
                client.GetConversationsList(response =>
                {
                    actual = response;
                    sync.Proceed();
                });
            }

            Assert.True(actual.ok, "Error while fetching conversation list.");
            Assert.True(actual.channels.Any());

            // check to null
            var someChannel = actual.channels.First();
            Assert.NotNull(someChannel.id);
            Assert.NotNull(someChannel.name);
        }

        [Fact]
        public async Task ConversationHistory()
        {
            var michaelChannel = new Channel
            {

                id = "D05N9LHH8UV"
            };

            var brandcrowdTechChannel = new Channel
            {
                id = "CCPTQJ64B"
            };
            var client = this.fixture.BotClientAsync;
            var response = await client.GetConversationsHistoryAsync(michaelChannel);
        }
    }
}