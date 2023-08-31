namespace SlackAPI.Tests.Helpers
{
    public class SlackMother
    {
        public static IBlock[] SomeBlocks => new IBlock[]
        {
            new ContextBlock
            {
                elements = new IElement[]{
                    new Text
                    {
                        type = TextTypes.Markdown,
                        text = "<http://flickr.com/bobby/|Bobby Tables>"

                    }
                }
            },
            new SectionBlock
            {
                text = new Text
                {
                    type = TextTypes.Markdown,
                    text = "<https://api.slack.com/reference/messaging/blocks|Slack Blocks Documentation>"
                },
                accessory = new ImageElement()
                {
                    image_url = "https://imgs.xkcd.com/comics/exploits_of_a_mom.png",
                    alt_text = "Required for image elements"
                }
            },
            new DividerBlock(),
            new SectionBlock
            {
                fields = new []
                {
                    new Text
                    {
                        type = TextTypes.Markdown,
                        text = "*Priority*\nHigh"
                    },
                    new Text
                    {
                        type = TextTypes.Markdown,
                        text = "*Priority*\nHigh"
                    },
                    new Text
                    {
                        type = TextTypes.Markdown,
                        text = "*Priority*\nHigh"
                    },
                    new Text
                    {
                        type = TextTypes.PlainText,
                        text = "*Priority*\nHigh"
                    }
                }
            }
        };

        public static IBlock[] SomeBlocksWithActions => new IBlock[]
       {
            new ContextBlock
            {
                elements = new IElement[]{
                    new Text
                    {
                            type = TextTypes.Markdown,
                            text = "<http://flickr.com/bobby/|Bobby Tables>"
                    }
                },

            },
            new SectionBlock
            {
                text = new Text
                {
                    type = TextTypes.Markdown,
                    text = "<https://api.slack.com/reference/messaging/blocks|Slack Blocks Documentation>"
                },
                accessory = new OverflowElement
                {
                    options = new []
                    {
                        new Option
                        {
                            text = new Text
                            {
                                type = TextTypes.PlainText,
                                text = "Option 1 Text"
                            },
                            value = "option 1"
                        },
                        new Option
                        {
                            text = new Text
                            {
                                type = TextTypes.PlainText,
                                text = "Option 2 Text"
                            },
                            value = "option 2"
                        },
                    }
                }
            },
            new DividerBlock(),
            new SectionBlock
            {
                fields = new []
                {
                    new Text
                    {
                        type = TextTypes.Markdown,
                        text = "*Priority*\nHigh"
                    },
                    new Text
                    {
                        type = TextTypes.Markdown,
                        text = "*Priority*\nHigh"
                    },
                    new Text
                    {
                        type = TextTypes.Markdown,
                        text = "*Priority*\nHigh"
                    },
                    new Text
                    {
                        type = TextTypes.PlainText,
                        text = "*Priority*\nHigh"
                    }
                }
            },
            new SectionBlock
            {
                text = new Text
                {
                    text = "Pick a date"
                },
                accessory = new Element
                {
                    type = ElementTypes.DatePicker,
                    initial_date = "1977-05-25",
                    placeholder = new Text
                    {
                        text = "Select a date"
                    }
                }
            },
            new ActionsBlock
            {
                block_id = "Optional unique identifier for a block",
                elements = new IElement[]
                {
                    new ButtonElement
                    {
                        text = new Text
                        {
                            text = "Button 1 Text"
                        },
                        value = "Button 1",
                        style = ButtonStyles.Danger,
                        confirm = new Confirm
                        {
                            title = new Text
                            {
                                text = "Are you sure?"
                            },
                            text = new Text
                            {
                                text = "Did you press Button 1?"
                            },
                            confirm = new Text
                            {
                                text = "I did"
                            },
                            deny = new Text
                            {
                                text = "I didn't"
                            }
                        }
                    },
                    new ButtonElement
                    {
                        text = new Text
                        {
                            text = "Button 2 Text"
                        },
                        value = "Button 2",
                    },
                    new ButtonElement
                    {
                        text = new Text
                        {
                            text = "Button 3 Text"
                        },
                        value = "Button 3",
                        style = ButtonStyles.Primary,
                    }
                }
            }
        };

        public static IBlock[] InputBlock => new IBlock[]
        {
            new InputBlock()
            {
                element = new InputElement()
                {
                    multiline = true,
                    action_id = "asdf"
                },
                label = new Label()
                {
                    text = "My Label"
                }
            }
        };
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