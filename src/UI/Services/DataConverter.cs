using System;
using System.Runtime.CompilerServices;

namespace PlcMonitor.UI.Services
{
    public static class DataConverter
    {
        private delegate TElement Reader<out TElement>(ReadOnlySpan<byte> input);

        public static T ConvertToRuntime<T>(in ReadOnlySpan<byte> data, in ReadOnlySpan<byte> dataOrder)
        {
            var type = typeof(T);
            var isArray = type.IsArray;
            if (isArray) type = type.GetElementType()!;

            if (type == typeof(byte) || type == typeof(sbyte))
            {
                return Format<T, byte>(data, x => x[0], isArray);
            }

            if (type == typeof(short) || type == typeof(ushort))
            {
                var reader = GetReader<ushort>(dataOrder);
                return Format<T, ushort>(data, reader, isArray);
            }

            if (type == typeof(int) || type == typeof(uint) || type == typeof(float))
            {
                var reader = GetReader<int>(dataOrder);
                return Format<T, int>(data, reader, isArray);
            }

            if (type == typeof(long) || type == typeof(ulong) || type == typeof(double))
            {
                var reader = GetReader<ulong>(dataOrder);
                return Format<T, ulong>(data, reader, isArray);
            }

            throw new NotImplementedException($"Reading type {typeof(T)} is not supported.");
        }

        private static Reader<T> GetReader<T>(in ReadOnlySpan<byte> dataOrder)
        {
            Reader<T> CastReader<TSample>(Reader<TSample> reader)
            {
                return Unsafe.As<Reader<TSample>, Reader<T>>(ref reader);
            }

            var type = typeof(T);
            if (type == typeof(short) || type == typeof(ushort))
            {
                int idx0 = dataOrder[0], idx1 = dataOrder[1];

                return CastReader(input => (ushort) (input[idx0] | input[idx1] << 8));
            }

            if (type == typeof(int) || type == typeof(uint) || type == typeof(float))
            {
                int idx0 = dataOrder[0], idx1 = dataOrder[1], idx2 = dataOrder[2], idx3 = dataOrder[3];

                return CastReader(input => (int) (input[idx0] | input[idx1] << 8 | input[idx2] << 16 | input[idx3] << 24));
            }

            if (type == typeof(long) || type == typeof(ulong) || type == typeof(double))
            {
                var readL = GetReader<uint>(dataOrder.Slice(0, 4));
                var readR = GetReader<uint>(dataOrder.Slice(4, 4));

                return CastReader(input => (ulong) readL(input) << 32 | readR(input));
            }

            throw new ArgumentException($"Unsupported type '{type}' requested for conversion.");
        }

        private static T Format<T, TElement>(ReadOnlySpan<byte> input, Reader<TElement> reader, bool isArray) where TElement : struct
        {
            if (!isArray)
            {
                var value = reader(input);
                return Unsafe.As<TElement, T>(ref value);
            }

            var elementSize = Unsafe.SizeOf<TElement>();
            var arr = Unsafe.As<TElement[]>(Activator.CreateInstance(typeof(T),
                input.Length / elementSize))!;
            var target = arr.AsSpan();

            while (target.Length > 0)
            {
                target[0] = reader(input);
                target = target.Slice(1);
                input = input.Slice(elementSize);
            }

            return Unsafe.As<TElement[], T>(ref arr);
        }
    }
}