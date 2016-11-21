using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SlackAPI
{
    public static class Extensions
    {
        internal static readonly IList<JsonConverter> Converters = new List<JsonConverter> { new JavascriptDateTimeConverter() };

        /// <summary>
        /// Converts to a propert JavaScript timestamp interpretted by Slack.  Also handles converting to UTC.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static string ToProperTimeStamp(this DateTime that, bool toUTC = true)
        {
            if (toUTC)
            {
                return ((that.ToUniversalTime().Ticks - 621355968000000000m) / 10000000m).ToString("F6");
            }
            else
                return that.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
        }

        public static K Deserialize<K>(this string data)
            where K : class
        {
            return JsonConvert.DeserializeObject<K>(data, CreateSettings(data));
        }

        public static object Deserialize(this string data, Type type)
        {
            return JsonConvert.DeserializeObject(data, type, CreateSettings(data));
        }

        private static JsonSerializerSettings CreateSettings(object contextData)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Context = new StreamingContext(StreamingContextStates.Other, contextData);
            settings.Converters = Converters;

            return settings;
        }
    }
}
