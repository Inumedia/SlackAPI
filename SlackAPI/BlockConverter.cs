using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackAPI
{
    public class BlockConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var block = JObject.Load(reader);
            var blockTypeString = block["type"]?.Value<string>();
            switch (blockTypeString)
            {
                case BlockTypes.Actions:
                    return block.ToObject<ActionsBlock>(serializer);
                case BlockTypes.Context:
                    return block.ToObject<ContextBlock>(serializer);
                case BlockTypes.Divider:
                    return block.ToObject<DividerBlock>(serializer);
                case BlockTypes.Header:
                    return block.ToObject<HeaderBlock>(serializer);
                case BlockTypes.Image:
                    return block.ToObject<ImageBlock>(serializer);
                case BlockTypes.Section:
                    return block.ToObject<SectionBlock>(serializer);
                default:
                    return block.ToObject<Block>(serializer);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IBlock);
        }
    }
}