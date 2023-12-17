namespace Jab.Performance.Startup;

using BenchmarkDotNet.Attributes;
using Jab;
using Jab.Performance.Basic.Singleton;
using Microsoft.Extensions.DependencyInjection;

public partial class StartupBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("01", "Singleton", "Jab")]
    public IServiceProvider Jab_Singleton()
    {
        var provider = new ContainerStartupSingleton();
        return provider.GetService<IServiceProvider>();
    }

    [Benchmark, BenchmarkCategory("01", "Singleton", "MEDI")]
    public IServiceProvider MEDI_Singleton() 
    { 
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ISingleton1, Singleton1>();
        serviceCollection.AddSingleton<ISingleton2, Singleton2>();
        serviceCollection.AddSingleton<ISingleton3, Singleton3>();
        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetService<IServiceProvider>()!;
    }
}

[ServiceProvider]
[Singleton(typeof(ISingleton1), typeof(Singleton1))]
[Singleton(typeof(ISingleton2), typeof(Singleton2))]
[Singleton(typeof(ISingleton3), typeof(Singleton3))]
internal partial class ContainerStartupSingleton
{
}