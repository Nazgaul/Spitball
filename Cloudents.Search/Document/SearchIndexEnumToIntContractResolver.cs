﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Infrastructure;
using Newtonsoft.Json.Converters;

namespace Cloudents.Search.Document
{
    public class SearchIndexEnumToIntContractResolver : DefaultContractResolver
    {

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var p = base.CreateProperty(member, memberSerialization);
            if (p.PropertyType?.IsEnum == true)
            {
                p.PropertyType = typeof(int);
            }

            if (p.PropertyType == typeof(Guid))
            {
                p.PropertyType = typeof(string);
            }
            var nullableType = Nullable.GetUnderlyingType(p.PropertyType);
            if (nullableType != null)
            {
                if (nullableType == typeof(Guid))
                {
                    p.PropertyType = typeof(string);
                }

                if (nullableType.IsEnum)
                {
                    p.PropertyType = typeof(int);
                }
            }

            var att = member.GetCustomAttribute<JsonConverterAttribute>();
            if (att != null)
            {
                if (att.ConverterType == typeof(StringTypeConverter))
                {
                    p.PropertyType = typeof(string);
                }
                if (att.ConverterType == typeof(CountryConverter))
                {
                    p.PropertyType = typeof(int);
                }

                if (att.ConverterType == typeof(StringEnumConverter))
                {
                    p.PropertyType = typeof(string);
                }
            }


            return p;
        }
    }


    //TODO this is duplication of EnumerationConverter
    public class CountryConverter : JsonConverter<Country?>
    {
        public override void WriteJson(JsonWriter writer, Country? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }
            writer.WriteValue(value.Id);
        }

        public override Country? ReadJson(JsonReader reader, Type objectType, Country? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var val = reader.Value;
            if (val == null)
            {
                return null;
            }

            var id = (int)Convert.ChangeType(val, TypeCode.Int32);
            //var id = (int)val;
            return Enumeration.FromValue<Country>(id);

        }
    }

    public class StringTypeConverter : JsonConverter
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore

        };
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {

            var json = JsonConvert.SerializeObject(value, _jsonSerializerSettings);
            serializer.Serialize(writer, json);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {

            var json = reader.Value?.ToString();
            if (json == null)
            {
                return null;
            }
            var result = JsonConvert.DeserializeObject(json, objectType, _jsonSerializerSettings);
            return result;
        }

        public override bool CanConvert(Type objectType) => true;
    }
}