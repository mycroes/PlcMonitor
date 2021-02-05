using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.Storage;

namespace PlcMonitor.UI.Infrastructure
{
    public class PlcJsonConverter : JsonConverter<IPlc>
    {
        public override IPlc? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) throw new Exception($"Failed to read {JsonTokenType.StartArray} while attempting to read PLC from JSON.");
            if (!reader.Read()) throw new Exception($"Failed to read type while reading PLC from JSON.");

            var plcType = JsonSerializer.Deserialize<PlcType>(ref reader, options);

            if (!reader.Read()) throw new Exception($"Failed to read parameters while reading PLC from JSON.");

            IPlc? plc = plcType switch
            {
                PlcType.Modbus => JsonSerializer.Deserialize<ModbusPlc>(ref reader, options),
                PlcType.S7 => JsonSerializer.Deserialize<S7Plc>(ref reader, options),
                _ => throw new ArgumentOutOfRangeException($"Invalid PlcType {plcType} while reading PLC from JSON.")
            };

            if (plc == null) throw new Exception($"{nameof(plc)} can't be null while while attempting to read PLC from JSON.");

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray) throw new Exception($"Failed to read {JsonTokenType.EndArray} while attempting to read PLC from JSON.");

            return plc;
        }

        public override void Write(Utf8JsonWriter writer, IPlc value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            JsonSerializer.Serialize(writer, GetPlcType(value), options);
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
            writer.WriteEndArray();
        }

        private static PlcType GetPlcType(IPlc plc)
        {
            return plc switch
            {
                ModbusPlc _ => PlcType.Modbus,
                S7Plc _ => PlcType.S7,
                _ => throw new ArgumentException($"Unsupported PLC {plc}.", nameof(plc))
            };
        }
    }
}