using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    internal class JavascriptDateTimeConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            decimal value = decimal.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture);
            DateTime res = new DateTime(621355968000000000 + (long)(value * 10000000m)).ToLocalTime();
            System.Diagnostics.Debug.Assert(
                Decimal.Equals(
                    Decimal.Parse(res.ToProperTimeStamp()), 
                    Decimal.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture)), 
                "Precision loss :(");
            return res;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            //Not sure if this is correct :D
            writer.WriteValue(((DateTime)value).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }
    }
}
