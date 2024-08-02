using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
namespace BusinessIntelligence_API.Service
{
	public class NullEmptyStringConverter: JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(string);
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (string.IsNullOrEmpty(value?.ToString()))
			{
				writer.WriteNull();
			}
			else
			{
				writer.WriteValue(value);
			}
		}
	}
}
