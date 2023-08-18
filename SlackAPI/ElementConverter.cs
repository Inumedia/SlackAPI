using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackAPI
{
    public class ElementConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var element = JObject.Load(reader);
            var elementTypeString = element["type"]?.Value<string>();
            switch (elementTypeString)
            {
                case ElementTypes.Button:
                    return element.ToObject<ButtonElement>(serializer);
                case ElementTypes.ChannelSelect:
                    return element.ToObject<ChannelSelectElement>(serializer);
                case ElementTypes.ConversationSelect:
                    return element.ToObject<ConversationSelectElement>(serializer);
                case ElementTypes.DatePicker:
                    return element.ToObject<DatePickerElement>(serializer);
                case ElementTypes.ExternalSelect:
                    return element.ToObject<ExternalSelectElement>(serializer);
                case ElementTypes.Image:
                    return element.ToObject<ImageElement>(serializer);
                case ElementTypes.Markdown:
                    return element.ToObject<MarkdownElement>(serializer);
                case ElementTypes.Overflow:
                    return element.ToObject<OverflowElement>(serializer);
                case ElementTypes.UserSelect:
                    return element.ToObject<UserSelectElement>(serializer);
                case ElementTypes.StaticSelect:
                    return element.ToObject<StaticSelectElement>(serializer);
                default:
                    return element.ToObject<Element>(serializer);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IElement);
        }
    }
}