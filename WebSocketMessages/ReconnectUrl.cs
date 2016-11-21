using Newtonsoft.Json;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("reconnect_url")]
    public class ReconnectUrl : SlackSocketMessage
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}
