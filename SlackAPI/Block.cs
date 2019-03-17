using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
   //see https://api.slack.com/reference/messaging/blocks
   public class Block
   {
      public string type { get; set; }
      public string block_id { get; set; }
      public Text text { get; set; }
      public Element accessory { get; set; }
      public List<Element> elements { get; set; }
      public Text title { get; set; }
      public string image_url { get; set; }
      public string alt_text { get; set; }
      public List<Text> fields { get; set; }
   }
   public class Text
   {
      public string type { get; set; } = TextTypes.PlainText;
      public string text { get; set; }
      public bool? emoji { get; set; }
      public bool? verbatim { get; set; }
   }
   
   public class Option
   {
      public Text text { get; set; }
      public string value { get; set; }
   }

   public class OptionGroups
   {
      public Text label { get; set; }
      public List<Option> options { get; set; }
   }

   public class Confirm
   {
      public Text title { get; set; }
      public Text text { get; set; }
      public Text confirm { get; set; }
      public Text deny { get; set; }
   }

   public class Element
   {
      public string type { get; set; }
      public string action_id { get; set; }
      public Text text { get; set; }
      public string value { get; set; }
      public Text placeholder { get; set; }
      public List<Option> options { get; set; }
      public List<OptionGroups> option_groups { get; set; }
      public string image_url { get; set; }
      public string alt_text { get; set; }
      public string initial_date { get; set; }
      public string initial_user { get; set; }
      public string initial_channel { get; set; }
      public Confirm confirm { get; set; }
   }

   public static class BlockTypes
   {
      public const string Section = "section";
      public const string Divider = "divider";
      public const string Actions = "actions";
      public const string Context = "context";
      public const string Image = "image";
   }

   public static class TextTypes
   {
      public const string Markdown = "mrkdwn";
      public const string PlainText = "plain_text";
   }

   public static class ElementTypes
   {
      public const string Image = "image";
      public const string Button = "button";
      public const string StaticSelect = "static_select";
      public const string ExternalSelect = "external_select";
      public const string UserSelect = "user_select";
      public const string ChannelSelect = "channel_select";
      public const string Overflow = "overflow";
      public const string DatePicker = "date_picker";
   }


}
