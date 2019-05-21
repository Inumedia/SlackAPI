using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class BlockMessage
    {
        private readonly IntegrationFixture fixture;

        public BlockMessage(IntegrationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Blocks()
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
                    blocks: SlackMother.SomeBlocks);
            }

            // then
            Assert.True(actual.ok, "Error while posting message to channel. ");
        }

        [Fact]
        public void BlocksWithActions()
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
                    blocks: SlackMother.SomeBlocksWithActions);
            }

            // then
            Assert.True(actual.ok, "Error while posting message to channel. ");
        }
    }
}