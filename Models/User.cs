using Newtonsoft.Json;
using System;

namespace SlackAPI.Models
{
    public class User
    {
        /// <summary>
        /// Is the ID of the user.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public bool IsSlackBot
        {
            get
            {
                return Id.Equals("USLACKBOT", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "deleted")]
        public bool Deleted { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "profile")]
        public UserProfile Profile { get; set; }

        [JsonProperty(PropertyName = "is_admin")]
        public bool IsAdmin { get; set; }

        [JsonProperty(PropertyName = "is_owner")]
        public bool IsOwner { get; set; }

        [JsonProperty(PropertyName = "has_files")]
        public bool HasFiles { get; set; }

        [JsonProperty(PropertyName = "presence")]
        public string Presence { get; set; }

        [JsonProperty(PropertyName = "is_bot")]
        public bool IsBot { get; set; }

        [JsonProperty(PropertyName = "tz")]
        public string Tz { get; set; }

        [JsonProperty(PropertyName = "tz_label")]
        public string TzLabel { get; set; }

        [JsonProperty(PropertyName = "tz_offset")]
        public int TzOffset { get; set; }

    }
}
