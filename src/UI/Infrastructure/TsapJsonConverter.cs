using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sally7.Protocol.Cotp;

namespace PlcMonitor.UI.Infrastructure
{
    public class TsapJsonConverter : JsonConverter<Tsap>
    {
        public override Tsap Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var (channel, position) = JsonSerializer.Deserialize<Wrapper>(ref reader, options) ??
                throw new Exception($"Tsap is required to have a value.");

            return new Tsap(channel, position);
        }

        public override void Write(Utf8JsonWriter writer, Tsap value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new Wrapper(value.Channel, value.Position), options);
        }

        private record Wrapper(byte Channel, byte Position);
    }
}