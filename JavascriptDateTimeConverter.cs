using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    class JavascriptDateTimeConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            double value = double.Parse(reader.Value.ToString());
            return new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds(value)).ToLocalTime();
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            //Not sure if this is correct :D
            writer.WriteValue(((DateTime)value).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }
    }
}
