using System;
using SlackAPI.RPCMessages;
using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using System.Linq;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class PublishAppHomeTab
    {
        private readonly IntegrationFixture fixture;

        public PublishAppHomeTab(IntegrationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void SimpleMessageDelivery()
        {
            // given
            var client = this.fixture.UserClient;
            AppHomeTabResponse actual = null;
            var text = Guid.NewGuid().ToString("N");

            // when
            using (var sync = new InSync(nameof(SlackClient.PublishAppHomeTab)))
            {
                var section = new Block { type = BlockTypes.Section, text = new Text { type = TextTypes.Markdown, text = text } };
                client.PublishAppHomeTab(
                    response =>
                    {
                        actual = response;
                        sync.Proceed();
                    },
                    this.fixture.Config.DirectMessageUser,
                    new View { type = ViewTypes.Home, blocks = new IBlock[] { section } });
            }

            // then
            Assert.True(actual.ok, "Error while posting message to channel. ");
            Assert.Equal(text, actual.view.blocks[0].text.text);
            Assert.Equal(ViewTypes.Home, actual.view.type);
        }
    }
}