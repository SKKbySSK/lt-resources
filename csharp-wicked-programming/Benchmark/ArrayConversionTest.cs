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
        public void Union()
        {
            UnionArray union = new UnionArray()
            {
                Byte = array
            };
            float[] converted = union.Float;
        }

        public unsafe void UnionModifiedLength()
        {
            UnionArray union = new UnionArray()
            {
                Byte = array
            };

            Console.WriteLine($"Before : {union.Byte.Length}");

            // 配列の要素数を書き換えるコード
            // 1024の配列の長さを256に書き換える
            // ref https://devblogs.microsoft.com/premier-developer/managed-object-internals-part-3-the-layout-of-a-managed-array-3/
            fixed (byte* ptr = array)
            {
                // 64bitでは先頭要素の8バイト前、32bitでは4バイト前に配列の長さが記録されている
                if (Environment.Is64BitProcess)
                {
                    var lengthPtr = (long*)(ptr - 8);
                    *lengthPtr = union.Float.Length / 4;
                }
                else
                {
                    var lengthPtr = (long*)(ptr - 4);
                    *lengthPtr = union.Float.Length / 4;
                }
            }

            Console.WriteLine($"After : {union.Byte.Length}");

            // IndexOutOfRangeExceptionが発生すれば、Floatでの境界チェックが正しく動作している
            try
            {
                Console.Write("Range Check : ");
                var error = union.Float[union.Float.Length];
                Console.WriteLine("Failed");
            }
            catch(IndexOutOfRangeException)
            {
                Console.WriteLine("OK");
            }
        }
    }
}
