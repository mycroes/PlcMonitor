using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlcMonitor.UI.Infrastructure
{
    public class DerivedTypeJsonConverter<TBase, TDerived> : JsonConverter<TBase>
        where TBase : notnull
        where TDerived : notnull, TBase
    {
        public override TBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<TDerived>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (TDerived) value, options);
        }
    }
}