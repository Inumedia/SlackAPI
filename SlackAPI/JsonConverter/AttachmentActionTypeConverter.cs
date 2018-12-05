using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace SlackAPI.JsonConverter
{
    public class AttachmentActionTypeConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (AttachmentActionTypeEnum)Enum.Parse(typeof(AttachmentActionTypeEnum), reader.Value.ToString(), true);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            AttachmentActionTypeEnum type = (AttachmentActionTypeEnum)Enum.Parse(typeof(AttachmentActionTypeEnum), value.ToString());

            writer.WriteValue(type.ToString().ToLower());
        }
    }
}
