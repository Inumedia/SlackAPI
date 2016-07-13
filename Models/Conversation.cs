using Newtonsoft.Json;
using System;

namespace SlackAPI.Models
{
    /// <summary>
    /// Base class from all slack conversations
    /// </summary>
    public class Conversation
    {
        /// <summary>
        /// Is the ID of the channel, private group or DM channel.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }

        [JsonProperty(PropertyName = "has_pins")]
        public bool HasPins { get; set; }

        [Obsolete]
        [JsonProperty(PropertyName = "is_starred")]
        public bool IsStarred { get; set; }

        [JsonProperty(PropertyName = "is_open")]
        public bool IsOpen { get; set; }

        [JsonProperty(PropertyName = "last_read")]
        public DateTime LastRead { get; set; }

        [JsonProperty(PropertyName = "latest")]
        public Message Latest { get; set; }

        [JsonProperty(PropertyName = "unread_count")]
        public int UnreadCount { get; set; }

        [JsonProperty(PropertyName = "unread_count_display")]
        public string UnreadCountDisplay { get; set; }

        public string Name { get; set; }

        public virtual SlackChannelType Type { get; }

    }
}
