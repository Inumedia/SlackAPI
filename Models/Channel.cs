using Newtonsoft.Json;
using System;

namespace SlackAPI.Models
{
    /// <summary>
    /// https://api.slack.com/types/channel
    /// </summary>
    public class Channel : SlackChannel
    {
        [JsonProperty(PropertyName = "is_channel")]
        public bool IsChannel { get; set; }

        /// <summary>
        /// Is the user ID of the member that created this private channel.
        /// </summary>
        [JsonProperty(PropertyName = "creator")]
        public string CreatorUserId { get; set; }

        /// <summary>
        /// Will be true if the channel is archived.
        /// </summary>
        [JsonProperty(PropertyName = "is_archived")]
        public bool IsArchived { get; set; }

        [JsonProperty(PropertyName = "is_mpim")]
        public bool IsMpim { get; set; }

        /// <summary>
        /// Is a list of user ids for all users in this channel. 
        /// This includes any disabled accounts that were in this channel when they were disabled.
        /// </summary>
        [JsonProperty(PropertyName = "members")]
        public string[] Members { get; set; }

        /// <summary>
        /// Will be true if the calling member is part of the channel.
        /// </summary>
        [JsonProperty(PropertyName = "is_member")]
        public bool IsMember { get; set; }

        /// <summary>
        /// Will be true if this channel is the "general" channel that includes all regular team members. 
        /// In most teams this is called #general but some teams have renamed it.
        /// </summary>
        [JsonProperty(PropertyName = "is_general")]
        public bool IsGeneral { get; set; }

        [JsonProperty(PropertyName = "is_group")]
        public bool IsGroup { get; set; }

        [Obsolete]
        [JsonProperty(PropertyName = "num_members")]
        public int NumMembers { get; set; }

        /// <summary>
        /// Provide information about the channel topic
        /// </summary>
        [JsonProperty(PropertyName = "topic")]
        public OwnedStampedMessage Topic { get; set; }

        /// <summary>
        /// Provide information about the channel purpose
        /// </summary>
        [JsonProperty(PropertyName = "purpose")]
        public OwnedStampedMessage Purpose { get; set; }

        public override SlackChannelType Type { get { return IsChannel ? SlackChannelType.Channel : SlackChannelType.Group; } }
    }
}
