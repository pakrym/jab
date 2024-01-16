```

BenchmarkDotNet v0.13.10, Windows 10 (10.0.19045.3930/22H2/2022Update)
AMD Ryzen 9 3900XT, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method         | Mean         | Error         | StdDev      | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------- |-------------:|--------------:|------------:|-------:|-------:|----------:|------------:|
| Jab_Singleton  |     8.763 ns |     0.5496 ns |   0.0301 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Singleton | 1,883.539 ns | 1,976.8621 ns | 108.3585 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Scoped     |     8.445 ns |     6.0425 ns |   0.3312 ns | 0.0038 |      - |      32 B |        1.00 |
| MEDI_Scoped    | 1,894.341 ns | 1,854.7252 ns | 101.6637 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Transient  |     8.695 ns |    11.2536 ns |   0.6168 ns | 0.0038 |      - |      32 B |        1.00 |
| MEDI_Transient | 1,894.840 ns | 2,000.5040 ns | 109.6544 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Mixed      |     9.722 ns |     2.2865 ns |   0.1253 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Mixed     | 2,454.882 ns | 2,626.4526 ns | 143.9647 ns | 1.0834 | 0.2689 |    9064 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Complex    |    13.402 ns |     4.5401 ns |   0.2489 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Complex   | 2,350.227 ns | 2,335.5694 ns | 128.0204 ns | 1.1330 | 0.2823 |    9496 B |           ? |
