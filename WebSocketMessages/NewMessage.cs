using Newtonsoft.Json;
using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("message")]
    [SlackSocketRouting("message", "bot_message")]
    public class NewMessage : SlackSocketMessage
    {
        /// <summary>
        /// The user property is the ID of the user speaking
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        public string UserId { get; set; }

        /// <summary>
        /// The channel property is the ID of the channel, private group or DM channel this message is posted in
        /// </summary>
        [JsonProperty(PropertyName = "channel")]
        public string ChannelId { get; set; }

        /// <summary>
        /// The text property is the text spoken
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "team")]
        public string TeamId { get; set; }

        /// <summary>
        /// The ts is the unique (per-channel) timestamp.
        /// </summary>
        [JsonProperty(PropertyName = "ts")]
        public DateTime Ts { get; set; }

        public NewMessage()
        {
            type = "message";
        }
    }
}
