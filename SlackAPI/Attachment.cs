namespace SlackAPI
{
    //See: https://api.slack.com/docs/attachments
    public class Attachment
    {
        public string callback_id;
        public string fallback;
        public string color;
        public string pretext;
        public string author_name;
        public string author_link;
        public string author_icon;
        public string title;
        public string title_link;
        public string text;
        public Field[] fields;
        
        public string image_url;
        public string thumb_url;
        public string[] mrkdwn_in;
        public AttachmentAction[] actions;

        public string footer;
        public string footer_icon;
    }

    public class Field{
        public string title;
        public string value;
        public bool @short;
    }

    //See: https://api.slack.com/docs/message-buttons#action_fields
    public class AttachmentAction
    {
        public AttachmentAction(string name, string text)
        {
            this.name = name;
            this.text = text;
        }
        public string name { get; }
        public string text { get; }
        public string style;
        public string type = "button";
        public string value;
        public ActionConfirm confirm;
    }

    //see: https://api.slack.com/docs/message-buttons#confirmation_fields
    public class ActionConfirm
    {
        public string title;
        public string text;
        public string ok_text;
        public string dismiss_text;
    }
}
