using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.Reflection;

BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);


//var config = ManualConfig.Create(DefaultConfig.Instance)
//                         .WithOptions(ConfigOptions.JoinSummary | ConfigOptions.DisableLogFile);

//BenchmarkRunner.Run(Assembly.GetExecutingAssembly(), config);