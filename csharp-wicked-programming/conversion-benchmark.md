```
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.836 (1909/November2018Update/19H2)
AMD Ryzen 5 3600, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.202
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
```


 Method |          Mean |      Error |     StdDev |
------- |--------------:|-----------:|-----------:|
 Normal | 1,517.8060 ns | 29.9942 ns | 50.1136 ns |
 Unsafe | 1,074.6857 ns | 21.2422 ns | 46.6270 ns |
  Union |     0.6836 ns |  0.0077 ns |  0.0069 ns |
