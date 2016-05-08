namespace IntegrationTest.Helpers
{
    using SlackAPI;

    public class SlackMother
    {
        public static Attachment[] SomeAttachments => new[]
        {
            new Attachment()
            {
                fallback = "Required plain-text summary of the attachment.",
                color = "#36a64f",
                pretext = "Optional text that appears above the attachment block",
                author_name = "Bobby Tables",
                author_link = "http://flickr.com/bobby/",
                author_icon = "http://flickr.com/icons/bobby.jpg",
                title = "Slack API Documentation",
                title_link = "https://api.slack.com/",
                text = "Optional text that appears within the attachment",
                fields = new[]
                {
                    new Field() { title = "Priority", value = "High", @short = false },
                    new Field() { title = "Priority", value = "High", @short = true },
                    new Field() { title = "Priority", value = "High", @short = true }
                }
            }
        };
    }
}