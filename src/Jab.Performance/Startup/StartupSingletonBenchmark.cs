namespace Jab.Performance.Startup;

using BenchmarkDotNet.Attributes;
using Jab;
using Jab.Performance.Basic.Singleton;
using Microsoft.Extensions.DependencyInjection;

[MemoryDiagnoser]
public class StartupSingletonBenchmark
{
    [Benchmark(Baseline = true)]
    public void Jab()
    {
        var provider = new ContainerStartupSingleton();
        var _ = provider.GetService<IServiceProvider>();
    }

    [Benchmark]
    public void MEDI() 
    { 
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ISingleton1, Singleton1>();
        serviceCollection.AddSingleton<ISingleton2, Singleton2>();
        serviceCollection.AddSingleton<ISingleton3, Singleton3>();
        var provider = serviceCollection.BuildServiceProvider();
        var _ = provider.GetService<IServiceProvider>();
    }
}

[ServiceProvider]
[Singleton(typeof(ISingleton1), typeof(Singleton1))]
[Singleton(typeof(ISingleton2), typeof(Singleton2))]
[Singleton(typeof(ISingleton3), typeof(Singleton3))]
internal partial class ContainerStartupSingleton
{
}