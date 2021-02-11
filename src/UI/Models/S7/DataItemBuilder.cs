using System;
using System.Reflection;
using PlcMonitor.UI.Infrastructure;
using ReflEx;
using Sally7;

namespace PlcMonitor.UI.Models.S7
{
    public class DataItemBuilder
    {
        private static readonly MethodInfo GenericBuildMethod = Extract.GenericMethodDefinition(() => BuildDataItem<int>(default, default, default, default));

        public static IDataItem<TValue> BuildDataItem<TValue>(int dataBlock, int startByte, int bitIndex, int length)
        {
            return new DataBlockDataItem<TValue>
            {
                Bit = bitIndex,
                DbNumber = dataBlock,
                Length = length,
                StartByte = startByte
            };
        }

        public static IDataItem BuildDataItem(in string address, in int length)
        {
            if (!AddressParser.TryParse(address, out var dataBlock, out var typeCode, out var startByte, out int? bitIndex))
                throw new ArgumentException($"Address {address} doesn't match expected address format.",
                    nameof(address));

            var type = TypeCodeLookup.TypeCodeToType(typeCode);
            if (length > 1) type = type.MakeArrayType();

            return (IDataItem) GenericBuildMethod.MakeGenericMethod(type).Invoke(
                null, new object[] { dataBlock, startByte, bitIndex ?? 0, length })!;
        }
    }
}