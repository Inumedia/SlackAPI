namespace SlackAPI.Tests.Helpers
{
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

        public static Attachment[] SomeAttachmentsWithActions => new[]
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
                },
                actions = new []
                {
                    new AttachmentAction("Button 1", "Button 1 Text") ,
                    new AttachmentAction("Button 2", "Button 2 Text") {style = "primary"},
                    new AttachmentAction("Button 3", "Button 3 Text") {style = "danger"},
                    new AttachmentAction("Button 4", "Button 4 Text") {style = "danger", confirm = new ActionConfirm {text = "Are you sure?????"} },
                    new AttachmentAction("Button 5", "Button 5 Text") {style = "danger", confirm = new ActionConfirm {text = "Do you really want to do this", dismiss_text = "No I don't", ok_text = "Sure I do", title = "Just checking"} }
                }
            }
        };
    }
}