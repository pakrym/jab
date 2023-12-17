```

BenchmarkDotNet v0.13.10, Windows 10 (10.0.19045.3803/22H2/2022Update)
AMD Ryzen 9 3900XT, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method         | Mean         | Error        | StdDev      | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------- |-------------:|-------------:|------------:|-------:|-------:|----------:|------------:|
| Jab_Singleton  |     8.629 ns |     7.745 ns |   0.4245 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Singleton | 2,177.868 ns | 4,000.891 ns | 219.3023 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |              |             |        |        |           |             |
| Jab_Scoped     |     7.988 ns |     6.414 ns |   0.3515 ns | 0.0038 |      - |      32 B |        1.00 |
| MEDI_Scoped    | 1,897.986 ns | 1,878.578 ns | 102.9712 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |              |             |        |        |           |             |
| Jab_Transient  |     8.279 ns |     6.090 ns |   0.3338 ns | 0.0038 |      - |      32 B |        1.00 |
| MEDI_Transient | 1,864.865 ns | 2,109.098 ns | 115.6068 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |              |             |        |        |           |             |
| Jab_Mixed      |    10.311 ns |     7.034 ns |   0.3856 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Mixed     | 2,475.742 ns | 2,388.959 ns | 130.9469 ns | 1.0834 | 0.2689 |    9064 B |           ? |
|                |              |              |             |        |        |           |             |
| Jab_Complex    |    14.194 ns |    24.354 ns |   1.3349 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Complex   | 2,382.348 ns | 1,215.594 ns |  66.6308 ns | 1.1330 | 0.2823 |    9496 B |           ? |
