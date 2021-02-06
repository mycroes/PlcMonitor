using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using PlcMonitor.UI.Models.Storage;

namespace PlcMonitor.UI.Infrastructure
{
    public class ValueWithTypeCodeJsonConverter : JsonConverter<ValueWithTypeCode>
    {
        public override ValueWithTypeCode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonException Ex(string reason) => new JsonException($"{reason} while attempting to read stored value from JSON.");
            void AssertRead(ref Utf8JsonReader reader, string noReadError) {
                if (!reader.Read()) throw Ex(noReadError);
            }

            if (reader.TokenType != JsonTokenType.StartArray) throw Ex($"Failed to read {JsonTokenType.StartArray}");

            AssertRead(ref reader, $"Failed to read {nameof(ValueWithTypeCode.TypeCode)}");
            var typeCode = JsonSerializer.Deserialize<TypeCode>(ref reader, options);

            AssertRead(ref reader, $"Failed to read {nameof(ValueWithTypeCode.IsArray)}");
            var isArray = reader.GetBoolean();

            var valueType = GetValueType(typeCode, isArray);
            AssertRead(ref reader, $"Failed to read {nameof(ValueWithTypeCode.Value)}");

            var value = JsonSerializer.Deserialize(ref reader, valueType, options) ?? Ex("Value can't be null");

            AssertRead(ref reader, $"Failed to read {nameof(JsonTokenType.EndArray)}");
            if (reader.TokenType != JsonTokenType.EndArray) throw Ex($"Failed to read {nameof(JsonTokenType.EndArray)} after value");

            return new ValueWithTypeCode(typeCode, isArray, value);
        }

        public override void Write(Utf8JsonWriter writer, ValueWithTypeCode value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            JsonSerializer.Serialize(writer, value.TypeCode, options);
            JsonSerializer.Serialize(writer, value.IsArray, options);
            JsonSerializer.Serialize(writer, value.Value, value.Value.GetType(), options);
            writer.WriteEndArray();
        }

        private Type GetValueType(TypeCode typeCode, bool isArray)
        {
            if (isArray) return GetValueType(typeCode, false).MakeArrayType();

            return typeCode switch
            {
                TypeCode.Boolean => typeof(bool),
                TypeCode.Byte => typeof(byte),
                TypeCode.Char => typeof(char),
                TypeCode.DateTime => typeof(DateTime),
                TypeCode.Double => typeof(double),
                TypeCode.Int16 => typeof(short),
                TypeCode.Int32 => typeof(int),
                TypeCode.Int64 => typeof(long),
                TypeCode.SByte => typeof(sbyte),
                TypeCode.Single => typeof(float),
                TypeCode.String => typeof(string),
                TypeCode.UInt16 => typeof(ushort),
                TypeCode.UInt32 => typeof(uint),
                TypeCode.UInt64 => typeof(ulong),
                _ => throw new ArgumentException($"Unsupported {nameof(TypeCode)} {typeCode} while attempting to read stored value.")
            };
        }
    }
}