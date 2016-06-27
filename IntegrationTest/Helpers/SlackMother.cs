using SlackAPI;
using SlackAPI.Models;

namespace IntegrationTest.Helpers
{
    public class SlackMother
    {
        public static Attachment[] SomeAttachments => new[]
        {
            new Attachment()
            {
                Fallback = "Required plain-text summary of the attachment.",
                Color = "#36a64f",
                Pretext = "Optional text that appears above the attachment block",
                AuthorName = "Bobby Tables",
                AuthorLink = "http://flickr.com/bobby/",
                AuthorIcon = "http://flickr.com/icons/bobby.jpg",
                Title = "Slack API Documentation",
                TitleLink = "https://api.slack.com/",
                Text = "Optional text that appears within the attachment",
                Fields = new[]
                {
                    new Field() { Title = "Priority", Value = "High", IsShort = false },
                    new Field() { Title = "Priority", Value = "High", IsShort = true },
                    new Field() { Title = "Priority", Value = "High", IsShort = true }
                }
            }
        };
    }
}