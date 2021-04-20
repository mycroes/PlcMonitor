using System;
using System.Runtime.CompilerServices;

namespace PlcMonitor.UI.Services
{
    public static class DataConverter
    {
        private delegate TElement Reader<out TElement>(ReadOnlySpan<byte> input);
        private delegate void Writer<in TElement>(Span<byte> data, TElement value);

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

            throw new NotImplementedException($"Reading type '{typeof(T)}' is not supported.");
        }

        public static void ConvertToDevice<T>(Span<byte> data, in ReadOnlySpan<byte> dataOrder, in T value)
        {
            var type = typeof(T);
            var isArray = type.IsArray;
            if (isArray) type = type.GetElementType()!;

            if (type == typeof(byte) || type == typeof(sbyte))
            {
                Format<T, byte>(data, value, (o, v) => o[0] = v, isArray);
            }
            else if (type == typeof(short) || type == typeof(ushort))
            {
                var writer = GetWriter<ushort>(dataOrder);
                Format<T, ushort>(data, value, writer, isArray);
            }
            else if (type == typeof(int) || type == typeof(uint) || type == typeof(float))
            {
                var writer = GetWriter<int>(dataOrder);
                Format<T, int>(data, value, writer, isArray);
            }
            else if (type == typeof(long) || type == typeof(ulong) || type == typeof(double))
            {
                var writer = GetWriter<ulong>(dataOrder);
                Format<T, ulong>(data, value, writer, isArray);
            }
            else
            {
                throw new NotImplementedException($"Writing type '{typeof(T)}' is not supported.");
            }
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

        private static Writer<T> GetWriter<T>(in ReadOnlySpan<byte> dataOrder)
        {
            Writer<T> CastWriter<TSample>(Writer<TSample> writer) => Unsafe.As<Writer<TSample>, Writer<T>>(ref writer);

            var type = typeof(T);
            if (type == typeof(short) || type == typeof(ushort))
            {
                int idx0 = dataOrder[0], idx1 = dataOrder[1];

                return CastWriter<ushort>((output, value) => {
                    output[idx0] = (byte) value;
                    output[idx1] = (byte) (value >> 8);
                });
            }

            if (type == typeof(int) || type == typeof(uint) || type == typeof(float))
            {
                int idx0 = dataOrder[0], idx1 = dataOrder[1], idx2 = dataOrder[2], idx3 = dataOrder[3];

                return CastWriter<int>((output, value) => {
                    output[idx0] = (byte) value;
                    output[idx1] = (byte) (value >> 8);
                    output[idx2] = (byte) (value >> 16);
                    output[idx3] = (byte) (value >> 24);
                });
            }

            if (type == typeof(long) || type == typeof(ulong) || type == typeof(double))
            {
                var writeL = GetWriter<uint>(dataOrder.Slice(0, 4));
                var writeR = GetWriter<uint>(dataOrder.Slice(4, 4));

                return CastWriter<ulong>((output, value) => {
                    writeL(output, (uint) (value >> 32));
                    writeR(output, (uint) value);
                });
            }

            throw new ArgumentException($"Unsupported type '{type}' request for conversion.");
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

        private static void Format<T, TElement>(Span<byte> data, T value, Writer<TElement> writer, bool isArray)
            where TElement : struct
        {
            if (!isArray)
            {
                writer.Invoke(data, Unsafe.As<T, TElement>(ref value));
                return;
            }

            var elementSize = Unsafe.SizeOf<TElement>();
            var source = Unsafe.As<T, TElement[]>(ref value).AsSpan();

            while (data.Length > 0)
            {
                writer.Invoke(data, source[0]);
                data = data.Slice(elementSize);
                source = source.Slice(1);
            }
        }
    }
}