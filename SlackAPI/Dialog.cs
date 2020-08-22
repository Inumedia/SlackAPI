namespace SlackAPI
{
   //see https://api.slack.com/dialogs

   public class Dialog
   {
      public string callback_id { get; set; }
      public string title { get; set; }
      public string submit_label { get; set; }
      public bool? notify_on_cancel { get; set; }
      public string state { get; set; }
      public Element[] elements { get; set; }

      public abstract class Element
      {
         public string label { get; set; }
         public string name { get; set; }
         public string placeholder { get; set; }
         public bool? optional { get; set; }
         public string value { get; set; }
      }

      public class TextElement : Element
      {
         public string type { get; } = "text";
         public string subtype { get; set; }
         public int? max_length { get; set; }
         public int? min_length { get; set; }
         public string hint { get; set; }

      }

      public class TextAreaElement : Element
      {
         public string type { get; } = "textarea";
         public int? max_length { get; set; }
         public int? min_length { get; set; }
         public string hint { get; set; }

      }

      public class SelectElement : Element
      {
         public string type { get; } = "select";
         public string data_source { get; set; }
         public Option[] options { get; set; }
         public OptionGroup[] option_groups { get; set; }
         public Option[] selected_options { get; set; }
         public int? min_query_length { get; set; }
      }

      public class Option
      {
         public string label { get; set; }
         public string value { get; set; }
      }

      public class OptionGroup
      {
         public string label { get; set; }
         public Option[] options { get; set; }
      }

      public static class DataSourceTypes
      {
         public static string Users = "users";
         public static string Channels = "channels";
         public static string Conversations = "conversations";
         public static string External = "external";
      }

      public static class TextElementSubTypes
      {
         public static string Email = "email";
         public static string Number = "number";
         public static string Telephone = "telephone";
         public static string Url = "url";
      }
   }
}