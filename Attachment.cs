using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    //See: https://api.slack.com/docs/attachments
    public class Attachment
    {
        public string pretext;
        public string text;
        public string fallback;
        public string color;
        
        public string author_name;
        public string author_link;
        public string author_icon;
        
        public string title;
        public string title_link;
        
        public Field[] fields;
        
        public string image_url;
        public string thumb_url;
        public string[] mrkdwn_in;
    }

    public class Field{
        public string title;
        public string value;
        public string @short;
    }
}
