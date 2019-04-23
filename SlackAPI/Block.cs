namespace SlackAPI
{
	using System;
	using System.Collections.Generic;

	using System.Globalization;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;
	using Newtonsoft.Json.Linq;

	[JsonConverter(typeof(JsonBlockConverter))]
	public abstract class Block
	{
		[JsonProperty("type")]
		public string Type { get; set; }
		[JsonProperty("block_id")]
		public string Block_ID { get; set; }

		private class JsonBlockConverter : JsonCreationConverter<Block>
		{
			protected override Block Create(Type objectType, JObject jsonObject)
			{
				var typeName = jsonObject["type"].ToString();
				switch (typeName)
				{
					case "section":
						return new SectionBlock();
					case "divider":
						return new DividerBlock();
					case "image":
						return new ImageBlock();
					case "actions":
						return new ActionBlock();
					case "context":
						return new ContextBlock();
					default: return null;
				}
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				//				case "section":
				//	return new SectionBlock();
				//case "divider":
				//	return new DividerBlock();
				//case "image":
				//	return new ImageBlock();
				//case "actions":
				//	return new ActionBlock();
				//case "context":
				//	return new ContextBlock();
				//JObject jo = null;
				//if (value is SectionBlock)
				//{
				//	SectionBlock obj = (SectionBlock)value;
				//	JToken jt = JToken.FromObject(obj);
				//	Console.WriteLine(jt.ToString());
				//}
				//else if (value is DividerBlock)
				//{
				//	DividerBlock obj = (DividerBlock)value;
				//	jo = JObject.FromObject(obj);
				//}
				//else if (value is ImageBlock)
				//{
				//	ImageBlock obj = (ImageBlock)value;
				//	jo = JObject.FromObject(obj);
				//}
				//else if (value is ActionBlock)
				//{
				//	ActionBlock obj = (ActionBlock)value;
				//	jo = JObject.FromObject(obj);
				//}
				//else if (value is ContextBlock)
				//{
				//	ContextBlock obj = (ContextBlock)value;
				//	jo = JObject.FromObject(obj);
				//}
				//else
				//{
				//	throw new Exception("Unknown block type!");
				//}
				//jo.WriteTo(writer);
				Block baseBlock = (Block)value;
				JObject block = new JObject();
				block["type"] = baseBlock.Type;
				block["block_id"] = baseBlock.Block_ID;
				if (value is SectionBlock)
				{
					SectionBlock derivedBlock = (SectionBlock)value;
					if (derivedBlock.Fields != null) block["fields"] = JArray.FromObject(derivedBlock.Fields);
					if (derivedBlock.Accessory != null) block["accessory"] = JObject.FromObject(derivedBlock.Accessory);
					if (derivedBlock.Text != null) block["text"] = JObject.FromObject(derivedBlock.Text);
				}
				else if (value is ImageBlock)
				{
					ImageBlock derivedBlock = (ImageBlock)value;
					if (derivedBlock.ImageURL != null) block["image_url"] = derivedBlock.ImageURL;
					if (derivedBlock.AltText != null) block["alt_text"] = derivedBlock.AltText;
					if (derivedBlock.Title != null) block["title"] = JObject.FromObject(derivedBlock.Title);
				}
				else if (value is ActionBlock)
				{
					ActionBlock derivedBlock = (ActionBlock)value;
					if (derivedBlock.Elements != null) block["elements"] = JArray.FromObject(derivedBlock.Elements);
				}
				else if (value is ContextBlock)
				{
					ContextBlock derivedBlock = (ContextBlock)value;
					if (derivedBlock.Elements != null) block["elements"] = JArray.FromObject(derivedBlock.Elements);
				}
				block.WriteTo(writer);
			}
		}
	}

	public class SectionBlock : Block
	{
		[JsonProperty("fields")]
		public SlackText[] Fields { get; set; }
		[JsonProperty("accessory")]
		public SlackElement Accessory { get; set; }
		[JsonProperty("text")]
		public SlackText Text { get; set; }
	}

	public class DividerBlock : Block
	{

	}

	public class ImageBlock : Block
	{
		[JsonProperty("image_url")]
		public string ImageURL { get; set; }
		[JsonProperty("alt_text")]
		public string AltText { get; set; }
		[JsonProperty("title")]
		public SlackText Title { get; set; }
	}

	public class ActionBlock : Block
	{
		[JsonProperty("elements")]
		public InteractiveElement[] Elements { get; set; }
	}

	public class ContextBlock : Block
	{
		[JsonProperty("elements")]
		public NonInteractiveElement[] Elements { get; set; }
	}

	[JsonConverter(typeof(JsonElementConverter))]
	public abstract class SlackElement
	{
		[JsonProperty("type")]
		public string Type { get; set; }

		private class JsonElementConverter : JsonCreationConverter<SlackElement>
		{
			protected override SlackElement Create(Type objectType, JObject jsonObject)
			{
				var typeName = jsonObject["type"].ToString();
				switch (typeName)
				{
					case "mrkdwn":
						return new SlackText();
					case "plain_text":
						return new SlackText();
					case "image":
						return new ImageElement();
					case "button":
						return new ButtonElement();
					case "static_select":
						return new StaticSelectElement();
					case "external_select":
						return new ExternalSelectElement();
					case "users_select":
						return new UserSelectElement();
					case "conversations_select":
						return new ConversationSelectElement();
					case "channels_select":
						return new ChannelSelectElement();
					case "overflow":
						return new OverflowElement();
					case "datepicker":
						return new DatePickerElement();
					default: return null;
				}
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				SlackElement baseElement = (SlackElement)value;
				JObject jo = new JObject();
				jo["type"] = baseElement.Type;
				if (value is SlackText)
				{
					SlackText derivedElement = (SlackText)value;
					jo["text"] = derivedElement.Text;
					if (derivedElement.Type != "mrkdwn")
					{
						jo["emoji"] = derivedElement.Emoji;
					}
					if (derivedElement.Type != "plain_text")
					{
						jo["verbatim"] = derivedElement.Verbatim;
					}
				}
				else if (value is ImageElement)
				{
					ImageElement derivedElement = (ImageElement)value;
					if (derivedElement.Image_URL != null) jo["image_url"] = derivedElement.Image_URL;
					if (derivedElement.Alt_Text != null) jo["alt_text"] = derivedElement.Alt_Text;
				}
				else if (value is InteractiveElement)
				{
					InteractiveElement interactiveElement = (InteractiveElement)value;
					jo["action_id"] = interactiveElement.Action_ID;
					if (interactiveElement.Confirm != null) jo["confirm"] = JObject.FromObject(interactiveElement.Confirm);
					if (value is ButtonElement)
					{
						ButtonElement buttonElement = (ButtonElement)value;
						jo["text"] = JObject.FromObject(buttonElement.Text);
						if (buttonElement.URL != null) jo["url"] = buttonElement.URL;
						if (buttonElement.Value != null) jo["value"] = buttonElement.Value;
						if (buttonElement.Style != null) jo["style"] = buttonElement.Style;
					}
					else if (value is SelectOrDatePickerElement)
					{
						SelectOrDatePickerElement selectOrDatePickerElement = (SelectOrDatePickerElement)value;
						if (selectOrDatePickerElement.PlaceHolder != null) jo["placeholder"] = JObject.FromObject(selectOrDatePickerElement.PlaceHolder);
						if (value is StaticSelectElement)
						{
							StaticSelectElement staticSelectElement = (StaticSelectElement)value;
							if (staticSelectElement.Options != null) jo["options"] = JArray.FromObject(staticSelectElement.Options);
							if (staticSelectElement.OptionGroups != null) jo["option_groups"] = JArray.FromObject(staticSelectElement.OptionGroups);
							if (staticSelectElement.InitialOption != null) jo["initial_option"] = JArray.FromObject(staticSelectElement.InitialOption);
						}
						else if (value is ExternalSelectElement)
						{
							ExternalSelectElement externalSelectElement = (ExternalSelectElement)value;
							if (externalSelectElement.InitialOption != null) jo["initial_option"] = JObject.FromObject(externalSelectElement.InitialOption);
							if (externalSelectElement.MinQueryLength != 0) jo["min_query_length"] = externalSelectElement.MinQueryLength;
						}
						else if (value is UserSelectElement)
						{
							UserSelectElement userSelectElement = (UserSelectElement)value;
							if (userSelectElement.InitialUser != null) jo["initial_user"] = JObject.FromObject(userSelectElement.InitialUser);
						}
						else if (value is ConversationSelectElement)
						{
							ConversationSelectElement conversationSelectElement = (ConversationSelectElement)value;
							if (conversationSelectElement.InitialConversation != null) jo["initial_conversation"] = conversationSelectElement.InitialConversation;
						}
						else if (value is ChannelSelectElement)
						{
							ChannelSelectElement channelSelectElement = (ChannelSelectElement)value;
							if (channelSelectElement.InitialChannel != null) jo["initial_channel"] = channelSelectElement.InitialChannel;
						}
						else if (value is DatePickerElement)
						{
							DatePickerElement datePickerElement = (DatePickerElement)value;
							if (datePickerElement.InitialDate != null) jo["initial_date"] = datePickerElement.InitialDate;
						}
						else
						{
							throw new Exception("Unknown element type!");
						}
					}
					else if (value is OverflowElement)
					{
						OverflowElement overflowElement = (OverflowElement)value;
						jo["options"] = JArray.FromObject(overflowElement.Options);
					}
					else
					{
						throw new Exception("Unknown element type!");
					}
				}
				else
				{
					throw new Exception("Unknown element type!");
				}
				jo.WriteTo(writer);
			}
		}
	}

	public abstract class NonInteractiveElement : SlackElement
	{

	}

	public partial class SlackText : NonInteractiveElement
	{
		[JsonProperty("text")]
		public string Text { get; set; }
		[JsonProperty("emoji")]
		public bool Emoji { get; set; }
		[JsonProperty("verbatim")]
		public bool Verbatim { get; set; }
	}

	public class ImageElement : NonInteractiveElement
	{
		[JsonProperty("image_url")]
		public string Image_URL { get; set; }
		[JsonProperty("alt_text")]
		public string Alt_Text { get; set; }
	}

	public abstract class InteractiveElement : SlackElement
	{
		[JsonProperty("action_id")]
		public string Action_ID { get; set; }
		[JsonProperty("confirm")]
		public SlackConfirm Confirm { get; set; }
	}

	public class ButtonElement : InteractiveElement
	{
		[JsonProperty("text")]
		public SlackText Text { get; set; }
		[JsonProperty("url")]
		public string URL { get; set; }
		[JsonProperty("value")]
		public string Value { get; set; }
		[JsonProperty("style")]
		public string Style { get; set; }
	}

	public abstract class SelectOrDatePickerElement : InteractiveElement
	{
		[JsonProperty("placeholder")]
		public SlackText PlaceHolder { get; set; }
	}

	public class StaticSelectElement : SelectOrDatePickerElement
	{
		[JsonProperty("options")]
		public SlackOption[] Options { get; set; }
		[JsonProperty("option_groups")]
		public SlackOptionGroup[] OptionGroups { get; set; }
		[JsonProperty("initial_option")]
		public SlackOption InitialOption { get; set; }
	}

	public class ExternalSelectElement : SelectOrDatePickerElement
	{
		[JsonProperty("initial_option")]
		public SlackOption InitialOption { get; set; }
		[JsonProperty("min_query_length")]
		public int MinQueryLength { get; set; }
	}

	public class UserSelectElement : SelectOrDatePickerElement
	{
		[JsonProperty("initial_user")]
		public string InitialUser { get; set; }
	}

	public class ConversationSelectElement : SelectOrDatePickerElement
	{
		[JsonProperty("initial_conversation")]
		public string InitialConversation { get; set; }
	}

	public class ChannelSelectElement : SelectOrDatePickerElement
	{
		[JsonProperty("initial_channel")]
		public string InitialChannel { get; set; }
	}

	public class DatePickerElement : SelectOrDatePickerElement
	{
		[JsonProperty("initial_date")]
		public string InitialDate { get; set; }
	}

	public class OverflowElement : InteractiveElement
	{
		[JsonProperty("options")]
		public SlackOption[] Options { get; set; }
	}

	public partial class SlackConfirm
	{
		[JsonProperty("title")]
		public SlackText Title { get; set; }
		[JsonProperty("text")]
		public SlackText Text { get; set; }
		[JsonProperty("confirm")]
		public SlackText Confirm { get; set; }
		[JsonProperty("deny")]
		public SlackText Deny { get; set; }
	}

	public partial class SlackOption
	{
		[JsonProperty("text")]
		public SlackText Text { get; set; }
		[JsonProperty("value")]
		public string Value { get; set; }
	}

	public partial class SlackOptionGroup
	{
		[JsonProperty("label")]
		public SlackText Label { get; set; }
		[JsonProperty("options")]
		public SlackOption[] Options { get; set; }
	}


	public abstract class JsonCreationConverter<T> : JsonConverter
	{
		protected abstract T Create(Type objectType, JObject jsonObject);

		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType,
		  object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;
			JObject jsonObject = JObject.Load(reader);
			T target = Create(objectType, jsonObject);
			serializer.Populate(jsonObject.CreateReader(), target);
			return target;
		}

		public override void WriteJson(JsonWriter writer, object value,
	   JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}

}
