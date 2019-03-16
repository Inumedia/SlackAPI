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
      public Text text { get; set; }
      public Accessory accessory { get; set; }
      public List<Element> elements { get; set; }
      public Title title { get; set; }
      public string image_url { get; set; }
      public string alt_text { get; set; }
      public List<BlockField> fields { get; set; }
   }
   public class Text
   {
      public string type { get; set; }
      public string text { get; set; }
      public bool? emoji { get; set; }
   }
   
   public class Placeholder
   {
      public string type { get; set; }
      public string text { get; set; }
      public bool emoji { get; set; }
   }
   
   public class Option
   {
      public Text text { get; set; }
      public string value { get; set; }
   }

   public class Accessory
   {
      public string type { get; set; }
      public string image_url { get; set; }
      public string alt_text { get; set; }
      public Text text { get; set; }
      public string value { get; set; }
      public Placeholder placeholder { get; set; }
      public List<Option> options { get; set; }
      public string initial_date { get; set; }
   }
   
   public class Element
   {
      public string type { get; set; }
      public Text text { get; set; }
      public string value { get; set; }
      public Text placeholder { get; set; }
      public List<Option> options { get; set; }
   }

   public class Title
   {
      public string type { get; set; }
      public string text { get; set; }
      public bool emoji { get; set; }
   }

   public class BlockField
   {
      public string type { get; set; }
      public string text { get; set; }
      public bool emoji { get; set; }
   }
   
}
