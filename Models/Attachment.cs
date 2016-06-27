using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SlackAPI.Models
{
    //See: https://api.slack.com/docs/attachments
    public class Attachment
    {
        [JsonProperty(PropertyName = "fallback")]
        public string Fallback { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "pretext")]
        public string Pretext { get; set; }

        [JsonProperty(PropertyName = "author_name")]
        public string AuthorName { get; set; }

        [JsonProperty(PropertyName = "author_link")]
        public string AuthorLink { get; set; }

        [JsonProperty(PropertyName = "author_icon")]
        public string AuthorIcon { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "title_link")]
        public string TitleLink { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public Field[] Fields { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "thumb_url")]
        public string ThumbUrl { get; set; }

        [JsonProperty(PropertyName = "mrkdwn_in")]
        public string[] MrkdwnIn { get; set; }
    }

    public class Field{
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "short")]
        public bool IsShort { get; set; }
    }
}
