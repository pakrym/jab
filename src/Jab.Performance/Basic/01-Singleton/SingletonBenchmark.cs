namespace Jab.Performance.Basic.Singleton; 

using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MEDI = Microsoft.Extensions.DependencyInjection;


[MemoryDiagnoser]
public class SingletonBenchmark
{
    private readonly MEDI.ServiceProvider _provider;
    private readonly ContainerSingleton _container = new();

    public SingletonBenchmark()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ISingleton1, Singleton1>();
        serviceCollection.AddSingleton<ISingleton2, Singleton2>();
        serviceCollection.AddSingleton<ISingleton3, Singleton3>();
        _provider = serviceCollection.BuildServiceProvider();
    }

    [Params(1, 10, 100)]
    public int NumbersOfCalls { get; set; }

    [Params(1, 2, 3)]
    public int NumbersOfClasses { get; set; }

    [Benchmark(Baseline = true)]
    public void Jab()
    {
        for (var i = 0; i < NumbersOfCalls; i++)
        {
            
            if (NumbersOfClasses >= 1)
                _container.GetService<ISingleton1>();
            if (NumbersOfClasses >= 2)
                _container.GetService<ISingleton2>();
            if (NumbersOfClasses >= 3)
                _container.GetService<ISingleton3>();
        }
    }

    [Benchmark]
    public void MEDI()
    {
        for (var i = 0; i < NumbersOfCalls; i++)
        {
            if (NumbersOfClasses >= 1)
                _provider.GetService<ISingleton1>();
            if(NumbersOfClasses >= 2)
                _provider.GetService<ISingleton2>();
            if (NumbersOfClasses >= 3)
                _provider.GetService<ISingleton3>();
        }
    }
}

[ServiceProvider]
[Singleton(typeof(ISingleton1), typeof(Singleton1))]
[Singleton(typeof(ISingleton2), typeof(Singleton2))]
[Singleton(typeof(ISingleton3), typeof(Singleton3))]
internal partial class ContainerSingleton
{
}