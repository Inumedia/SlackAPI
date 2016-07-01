using Newtonsoft.Json;
using System;

namespace SlackAPI.Models
{
    /// <summary>
    /// https://api.slack.com/types/im
    /// </summary>
    public class DirectMessage : SlackChannel
    {
        /// <summary>
        /// The user property is the ID of the user speaking
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "is_im")]
        public bool IsIm { get; set; }

        [JsonProperty(PropertyName = "is_org_shared")]
        public bool IsOrgShared { get; set; }

        [JsonProperty(PropertyName = "is_user_deleted")]
        public bool IsUserDeleted { get; set; }

        public override SlackChannelType Type { get; } = SlackChannelType.DirectMessage;
    }
}
