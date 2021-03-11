using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using PlcMonitor.UI.Models.Storage;

namespace PlcMonitor.UI.Infrastructure
{
    public class VariableJsonConverter<TVariable> : JsonConverter<TVariable>
        where TVariable : Variable
    {
        public override TVariable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonException Ex(string reason) => new JsonException($"{reason} while attempting to read variable from JSON.");
            void AssertRead(ref Utf8JsonReader reader, string noReadError) {
                if (!reader.Read()) throw Ex(noReadError);
            }

            if (reader.TokenType != JsonTokenType.StartArray) throw Ex($"Failed to read {JsonTokenType.StartArray}");
            AssertRead(ref reader, "Failed to read next token after variable start");
            var variable = JsonSerializer.Deserialize<TVariable>(ref reader);
            if (variable == null) throw Ex($"Variable can't be null");

            AssertRead(ref reader, "Failed to read next token after variable definition");

            if (reader.TokenType == JsonTokenType.EndArray) return variable;

            var value = JsonSerializer.Deserialize(ref reader, GetValueType(variable.TypeCode, variable.Length));
            if (value == null) throw Ex("Variable value can't be null");

            AssertRead(ref reader, "Failed to move to lastChange token");
            var lastChange = JsonSerializer.Deserialize<DateTimeOffset>(ref reader);

            AssertRead(ref reader, "Failed to move to lastRead token");
            var lastRead = JsonSerializer.Deserialize<DateTimeOffset>(ref reader);

            AssertRead(ref reader, "Failed to move to end of variable token");
            if (reader.TokenType != JsonTokenType.EndArray) throw Ex($"Failed to read {JsonTokenType.EndArray}");

            variable.State = new VariableState(value, lastChange, lastRead);

            return variable;
        }

        public override void Write(Utf8JsonWriter writer, TVariable value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            JsonSerializer.Serialize(writer, value);

            if (value.State is { })
            {
                JsonSerializer.Serialize(writer, value.State.Value, GetValueType(value.TypeCode, value.Length));
                JsonSerializer.Serialize(writer, value.State.LastChange);
                JsonSerializer.Serialize(writer, value.State.LastRead);
            }

            writer.WriteEndArray();
        }

        private static Type GetValueType(TypeCode typeCode, int length)
        {
            return typeCode == TypeCode.String ? typeof(string) : GetValueType(typeCode, length > 1);
        }

        private static Type GetValueType(TypeCode typeCode, bool isArray)
        {
            if (isArray) return GetValueType(typeCode, false).MakeArrayType();

            return TypeCodeLookup.TypeCodeToType(typeCode);
        }
    }
}