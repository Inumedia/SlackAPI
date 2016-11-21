using Newtonsoft.Json;
using SlackAPI.Models;
using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("file_public")]
    public class FilePublic : SlackSocketMessage
    {
        /// <summary>
        /// Is the ID of the file.
        /// </summary>
        [JsonProperty(PropertyName = "file_id")]
        public string FileId { get; set; }

        /// <summary>
        /// The user property is the ID of the user speaking
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "file")]
        public File File { get; set; }
        
        /// <summary>
        /// The event_ts is the unique (per-event) timestamp.
        /// </summary>
        [JsonProperty(PropertyName = "event_ts")]
        public DateTime EventTs { get; set; }

    }
}
