using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class Attachment
    {
        public string pretext;
        public string text;
        public string fallback;
        public string color;
        public Field[] fields;
        public string image_url;
        public string thumb_url;
        public string[] mrkdwn_in;
        public string title;
        ///I have absolutely no idea what goes on in here.
    }

    public class Field{
        public string title;
        public string value;
        public string @short;
    }
}
