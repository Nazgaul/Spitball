using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using Newtonsoft.Json.Serialization;

namespace Cloudents.Core
{
    //TODO: find a better place to put it
    //[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, 
    //    NamingStrategyParameters = new object[] {typeof(CamelCaseNamingStrategy)},
    //    ItemConverterType = new object[] {typeof(StringEnumConverter)},
    //    //ItemConverterParameters = new object[] {typeof(StringEnumConverter) }

    //    )]
    //[JsonConverter(typeof(StringEnumConverter), true)]
    //[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore,
    //    NamingStrategyType = typeof(CamelCaseNamingStrategy)
        
    //)]
    [JsonConverter(typeof(SignalRTransportTypeConverter))]
    public class SignalRTransportType
    {
        public SignalRTransportType(SignalRType type, SignalRAction action, IEnumerable data)
        {
            Type = type;
            Action = action;
            Data = data;
        }

        public SignalRTransportType(SignalRType type, SignalRAction action, object data)
        {
            Type = type;
            Action = action;
            Data = new[] { data };
        }

        [JsonConverter(typeof(StringEnumConverter), true)]
        public SignalRType Type { get; }
        [JsonConverter(typeof(StringEnumConverter), true)]
        public SignalRAction Action { get; set; }

        public IEnumerable Data { get; }
    }

    //This is temp solution until  https://github.com/Azure/azure-functions-signalrservice-extension/issues/34 resolved
    public class SignalRTransportTypeConverter : JsonConverter<SignalRTransportType>
    {
        public override void WriteJson(JsonWriter writer, SignalRTransportType value, JsonSerializer serializer)
        {
            var settings = new JsonSerializer()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            settings.Converters.Add(new StringEnumConverter(true));
            //writer.WriteValue(value);
            writer.WriteStartObject();
            
            writer.WritePropertyName("type");

            settings.Serialize(writer, value.Type);

            writer.WritePropertyName("action");
            settings.Serialize(writer,value.Action);
            if (value.Data != null)
            {
                writer.WritePropertyName("data");
                settings.Serialize(writer, value.Data);
            }

            writer.WriteEndAsync();
            //writer.WriteValue();
            //var t = JToken.FromObject(value);
            //t.WriteTo(writer);
        }

        public override bool CanRead => false;

        public override SignalRTransportType ReadJson(JsonReader reader, Type objectType, SignalRTransportType existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }


    public enum SignalRAction
    {
        Add,
        Delete,
        Update
    }

    public enum SignalRType
    {
        Question
    }
}
