using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace SlackAPI.JsonConverter
{
    public class AttachmentActionStyleConverter : Newtonsoft.Json.JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (AttachmentActionStyleEnum)Enum.Parse(typeof(AttachmentActionStyleEnum), reader.Value.ToString(), true);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            AttachmentActionStyleEnum style = (AttachmentActionStyleEnum)Enum.Parse(typeof(AttachmentActionStyleEnum), value.ToString());

            writer.WriteValue(style.ToString().ToLower());
        }
    }
}
