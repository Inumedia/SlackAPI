using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SlackAPI.Models
{
    /// <summary>
    /// See: https://api.slack.com/docs/attachments
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// Provide this attachment with a visual header by providing a short string here.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        /// <summary>
        /// A plaintext message displayed to users using an interface that does not support attachments or interactive messages.
        /// Consider leaving a URL pointing to your service if the potential message actions are representable outside of Slack. 
        /// Otherwise, let folks know what they are missing.
        /// </summary>
        [JsonProperty("fallback", NullValueHandling = NullValueHandling.Ignore)]
        public string Fallback { get; set; }

        /// <summary>
        /// The provided string will act as a unique identifier for the collection of buttons within the attachment. 
        /// It will be sent back to your message button action URL with each invoked action. This field is required 
        /// when the attachment contains message buttons. It is key to identifying the interaction you're working with.
        /// </summary>
        [JsonProperty("callback_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CallbackId { get; set; }

        /// <summary>
        /// Used to visually distinguish an attachment from other messages. Accepts hex values and a few named colors as 
        /// documented in attaching content to messages. Use sparingly and according to best practices.
        /// </summary>
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }

        [JsonProperty("pretext", NullValueHandling = NullValueHandling.Ignore)]
        public string Pretext { get; set; }

        [JsonProperty("author_name", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthorName { get; set; }

        [JsonProperty("author_link", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthorLink { get; set; }

        [JsonProperty("author_icon", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthorIcon { get; set; }

        [JsonProperty("title_link", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleLink { get; set; }

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public Field[] Fields { get; set; }

        [JsonProperty("image_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageUrl { get; set; }

        [JsonProperty("thumb_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ThumbUrl { get; set; }

        [JsonProperty("mrkdwn_in", NullValueHandling = NullValueHandling.Ignore)]
        public string[] MrkdwnIn { get; set; }

        [JsonProperty("attachment_type", NullValueHandling = NullValueHandling.Ignore)]
        public string AttachmentType { get; set; }

        /// <summary>
        /// A collection of actions (buttons) to include in the attachment. Required when using message buttons and otherwise not useful. 
        /// A maximum of 5 actions may be provided.
        /// </summary>
        [JsonProperty("actions", NullValueHandling = NullValueHandling.Ignore)]
        public ActionField[] Actions { get; set; }
    }

    public class Field
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "short")]
        public bool IsShort { get; set; }
    }

    /// <summary>
    /// https://api.slack.com/docs/message-buttons#actioin_fields
    /// </summary>
    public class ActionField
    {
        /// <summary>
        /// Provide a string to give this specific action a name. The name will be returned to your Action URL along 
        /// with the message's callback_id when this action is invoked. Use it to identify this particular response path. 
        /// If multiple actions share the same name, only one of them can be in a triggered state.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The user-facing label for the message button representing this action. 
        /// Cannot contain markup. Best to keep these short and decisive.
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Your buttons can have a little extra visual importance added to them, which is especially useful when providing logical default action or highlighting something destructive.
        /// default — Yes, it's the default. Buttons will look simple.
        /// primary — Use this sparingly, when the button represents a key action to accomplish. You should probably only ever have one primary button within a set.
        /// danger — Use this when the consequence of the button click will result in the destruction of something, like a piece of data stored on your servers. Use even more sparingly than primary.
        /// </summary>
        [JsonProperty(PropertyName = "style")]
        public string Style { get; set; }

        /// <summary>
        /// Provide nothing but button here. There are no other types of actions today.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "button";

        /// <summary>
        /// Provide a string identifying this specific action. It will be sent to your Action URL along with the name and attachment's callback_id. 
        /// If providing multiple actions with the same name, value can be strategically used to differentiate intent.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// If you provide a JSON hash of confirmation fields, your button will pop up dialog with your indicated text and choices, 
        /// giving them one last chance to avoid a destructive action or other undesired outcome.
        /// </summary>
        [JsonProperty(PropertyName = "confirm")]
        public ConfirmationField Confirm { get; set; }
    }

    /// <summary>
    /// https://api.slack.com/docs/message-buttons#confirmation_fields
    /// </summary>
    public class ConfirmationField
    {
        /// <summary>
        /// Title the pop up window. Please be brief.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Describe in detail the consequences of the action and contextualize your button text choices.
        /// Required
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        /// <summary>
        /// The text label for the button to continue with an action. Keep it short. Defaults to Okay.
        /// </summary>
        [JsonProperty(PropertyName = "ok_text")]
        public string OkText { get; set; }

        /// <summary>
        /// The text label for the button to cancel the action. Keep it short. Defaults to Cancel.
        /// </summary>
        [JsonProperty(PropertyName = "dismiss_text")]
        public string DismissText { get; set; }
    }
}
