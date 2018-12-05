using System.Collections.Generic;
using Newtonsoft.Json;
using SlackAPI.JsonConverter;

namespace SlackAPI
{
    //See: https://api.slack.com/docs/message-buttons#action_fields
    public class AttachmentAction
    {
        public AttachmentAction(AttachmentActionTypeEnum actionType, string name, string text)
        {
            this.type = actionType;
            this.name = name;
            this.text = text;
        }

        public string name { get; }
        public string text { get; }

        [JsonConverter(typeof(AttachmentActionStyleConverter))]
        public AttachmentActionStyleEnum style;

        [JsonConverter(typeof(AttachmentActionTypeConverter))]
        public AttachmentActionTypeEnum type;

        public string value;

        public AttachmentActionOption[] options;

        public ActionConfirm confirm;
    }

    public enum AttachmentActionStyleEnum
    {
        Default,
        Primary,
        Danger
    }

    public enum AttachmentActionTypeEnum
    {
        Button,
        Select
    }
}
