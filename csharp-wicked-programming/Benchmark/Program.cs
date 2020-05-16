using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using System;
using System.Runtime.InteropServices;

namespace Benchmark
{
    [StructLayout(LayoutKind.Explicit)]
    struct SampleStruct
    {
        [FieldOffset(0)]
        public uint A;

        [FieldOffset(1)]
        public byte B;
    }

    class CustomConfig : ManualConfig
    {
        public CustomConfig()
        {
            AddExporter(MarkdownExporter.Default);
            AddValidator(JitOptimizationsValidator.DontFailOnError);
            AddLogger(ConsoleLogger.Default);
            AddColumnProvider(DefaultColumnProviders.Instance);
        }
    }

    class Program
    {
        static unsafe void Main(string[] args)
        {
            int x = 50;
            int* ptr = &x; // xのポインタを取り出す
            *ptr /= 2; // ポインタの指すデータを2で割る(50 / 2 = 25)

            Console.WriteLine(x);
        }

        // 配列の要素数書き換えデモ
        private static void UnionModifiedTest()
        {
            var test = new ArrayConversionTest();
            test.UnionModifiedLength();
        }

        // 各変換方法のベンチマーク
        private static void Benchmark()
        {
            BenchmarkRunner.Run<ArrayConversionTest>(new CustomConfig());
        }

        // SampleStructのデモ
        private static void SampleStruct()
        {
            SampleStruct data = new SampleStruct();
            data.A = 0xDDCCBBAA;
            Console.WriteLine(data.B);
        }
    }
}
