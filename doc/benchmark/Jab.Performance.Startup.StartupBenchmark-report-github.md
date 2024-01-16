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
| Jab_Singleton  |     8.541 ns |     3.4715 ns |   0.1903 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Singleton | 2,162.589 ns | 2,026.5482 ns | 111.0819 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Scoped     |     9.227 ns |     4.8011 ns |   0.2632 ns | 0.0038 |      - |      32 B |        1.00 |
| MEDI_Scoped    | 1,895.721 ns | 2,150.1331 ns | 117.8560 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Transient  |     8.397 ns |     1.7967 ns |   0.0985 ns | 0.0038 |      - |      32 B |        1.00 |
| MEDI_Transient | 2,116.654 ns | 4,289.4329 ns | 235.1183 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Mixed      |     9.697 ns |     0.1884 ns |   0.0103 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Mixed     | 2,519.538 ns | 1,988.6074 ns | 109.0023 ns | 1.0834 | 0.2689 |    9064 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Complex    |    13.230 ns |     9.9845 ns |   0.5473 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Complex   | 2,429.244 ns | 1,541.6226 ns |  84.5015 ns | 1.1330 | 0.2823 |    9496 B |           ? |
