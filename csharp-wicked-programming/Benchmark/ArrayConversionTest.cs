using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Benchmark
{
    [StructLayout(LayoutKind.Explicit)]
    struct UnionArray
    {
        [FieldOffset(0)]
        public float[] Float;

        [FieldOffset(0)]
        public byte[] Byte;
    }

    public class ArrayConversionTest
    {
        const int size = 1024;
        private readonly byte[] array = new byte[size];

        [Benchmark]
        public void Normal()
        {
            float[] converted = new float[size / sizeof(float)];
            for (int i = 0; i < converted.Length; i++)
            {
                converted[i] = BitConverter.ToSingle(array, i * 4);
            }
        }

        [Benchmark]
        public unsafe void Unsafe()
        {
            float[] converted = new float[size / sizeof(float)];
            fixed (float* convertedPtr = converted)
            fixed (byte* sourcePtr = array)
            {
                float* floatSourcePtr = (float*)sourcePtr;
                for (int i = 0; i < converted.Length; i++)
                {
                    convertedPtr[i] = floatSourcePtr[i];
                }
            }
        }

        [Benchmark]
        public unsafe void Union()
        {
            UnionArray union = new UnionArray()
            {
                Byte = array
            };

            return;
            // https://devblogs.microsoft.com/premier-developer/managed-object-internals-part-3-the-layout-of-a-managed-array-3/
            fixed (byte* ptr = union.Byte)
            {
                // 64bitでは8バイト、32bitでは4バイトで配列の長さが記録されている
                var lengthDelta = Environment.Is64BitProcess ? 8 : 4;
                var lengthPtr = (long*)(ptr - lengthDelta);
                *lengthPtr = union.Float.Length / 4;
            }
        }
    }
}
