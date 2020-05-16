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

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ArrayConversionTest>(new CustomConfig());
        }

        private static void SampleStruct()
        {
            SampleStruct data = new SampleStruct();
            data.A = 0xDDCCBBAA;
            Console.WriteLine(data.B);
        }

        private class CustomConfig : ManualConfig
        {
            public CustomConfig()
            {
                AddExporter(MarkdownExporter.Default);
                AddValidator(JitOptimizationsValidator.DontFailOnError);
                AddLogger(ConsoleLogger.Default);
                AddColumnProvider(DefaultColumnProviders.Instance);
            }
        }
    }
}
