
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.836 (1909/November2018Update/19H2)
AMD Ryzen 5 3600, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.202
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT


 Method |          Mean |      Error |     StdDev |        Median |
------- |--------------:|-----------:|-----------:|--------------:|
 Normal | 1,534.6059 ns | 30.4039 ns | 53.2499 ns | 1,546.0442 ns |
 Unsafe |   783.1249 ns | 15.6298 ns | 37.4479 ns |   768.5819 ns |
  Union |     0.9233 ns |  0.0086 ns |  0.0067 ns |     0.9254 ns |
