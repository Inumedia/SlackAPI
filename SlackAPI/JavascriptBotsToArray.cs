using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class JavascriptBotsToArray : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            List<Bot> bots = new List<Bot>();
            int d = reader.Depth;

            while (reader.Read() && reader.Depth > d)
            {
                Bot current = new Bot();
                int depth = reader.Depth;

                current.name = reader.Value.ToString();

                reader.Read();
                while (reader.Read() && reader.Depth > depth)
                {
                    if (reader.Value == null) break;
                    switch (reader.Value.ToString())
                    {
                        case "image_48":
                            reader.Read();
                            current.image_48 = reader.Value.ToString();
                            break;

                        case "image_64":
                            reader.Read();
                            current.image_48 = reader.Value.ToString();
                            break;

                        case "emoji":
                            reader.Read();
                            current.emoji = reader.Value.ToString();
                            break;
                    }
                }

                bots.Add(current);
            }

            return bots.ToArray();
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            //Not sure if this is correct :D
            throw new NotSupportedException("Too hackish for this shi.");
        }
    }
}
