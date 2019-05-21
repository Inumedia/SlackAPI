using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("oauth.access")]
    public class AccessTokenResponse : Response
    {
        public string access_token;
        public string scope;
        public string team_name;
        public string team_id { get; set; }
        public BotTokenResponse bot;
        public IncomingWebhook incoming_webhook { get; set; }
    }

    public class BotTokenResponse
    {
       public string emoji;
       public string image_24;
       public string image_32;
       public string image_48;
       public string image_72;
       public string image_192;

       public bool deleted;
       public UserProfile icons;
       public string id;
       public string name;
       public string bot_user_id;
       public string bot_access_token;
    }

    public class IncomingWebhook
    {
        public string channel { get; set; }
        public string channel_id { get; set; }
        public string configuration_url { get; set; }
        public string url { get; set; }
    }
}
