using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.Storage;

namespace PlcMonitor.UI.Infrastructure
{
    public class PlcViewModelJsonConverter : JsonConverter<PlcConfiguration>
    {
        public override PlcConfiguration? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonException Ex(string reason) => new JsonException($"{reason} while attempting to read PLC from JSON.");
            void AssertRead(ref Utf8JsonReader reader, string noReadError) {
                if (!reader.Read()) throw Ex(noReadError);
            }

            if (reader.TokenType != JsonTokenType.StartArray) throw Ex($"Failed to read {JsonTokenType.StartArray}");
            AssertRead(ref reader, "Failed to read PLC type");

            var plcType = JsonSerializer.Deserialize<PlcType>(ref reader, options);
            AssertRead(ref reader, "Failed to read parameters");

            var nextOptions = GetOptions(plcType, options);

            var plc = JsonSerializer.Deserialize<PlcConfiguration>(ref reader, nextOptions);

            if (plc == null) throw new Exception($"{nameof(plc)} can't be null while while attempting to read PLC from JSON.");

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray) throw new Exception($"Failed to read {JsonTokenType.EndArray} while attempting to read PLC from JSON.");

            return plc;
        }

        public override void Write(Utf8JsonWriter writer, PlcConfiguration value, JsonSerializerOptions options)
        {
            var plcType = value.Plc switch
            {
                S7Plc => PlcType.S7,
                ModbusPlc => PlcType.Modbus,
                _ => throw new JsonException($"Invalid PLC value {value.Plc} supplied while serializing {nameof(PlcConfiguration)}.")
            };

            var nextOptions = GetOptions(plcType, options);

            writer.WriteStartArray();
            JsonSerializer.Serialize(writer, plcType, options);
            JsonSerializer.Serialize(writer, value, nextOptions);
            writer.WriteEndArray();
        }

        private JsonSerializerOptions GetOptions(PlcType plcType, JsonSerializerOptions options)
        {
            var converters = plcType switch
            {
                PlcType.S7 => GetS7JsonConverters(),
                PlcType.Modbus => GetModbusJsonConverters(),
                _ => throw new JsonException($"Invalid PLC type {plcType} specified.")
            };

            var nextOptions = new JsonSerializerOptions(options);
            nextOptions.Converters.Remove(this);
            foreach (var converter in converters)
                nextOptions.Converters.Insert(0, converter);

            return nextOptions;
        }

        private IEnumerable<JsonConverter> GetS7JsonConverters()
        {
            yield return new DerivedTypeJsonConverter<IPlc, S7Plc>();
            yield return new DerivedTypeJsonConverter<Variable, S7Variable>();
            yield return new VariableJsonConverter<S7Variable>();
            yield return new TsapJsonConverter();
        }

        private IEnumerable<JsonConverter> GetModbusJsonConverters()
        {
            yield return new DerivedTypeJsonConverter<IPlc, ModbusPlc>();
            yield return new DerivedTypeJsonConverter<Variable, ModbusVariable>();
            yield return new VariableJsonConverter<ModbusVariable>();
        }
    }
}