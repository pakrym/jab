namespace Jab.Performance.Basic.Transient; 

using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MEDI = Microsoft.Extensions.DependencyInjection;


[MemoryDiagnoser]
public class TransientBenchmark
{
    private readonly MEDI.ServiceProvider _provider;
    private readonly ContainerTransient _container = new();

    public TransientBenchmark()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ITransient1, Transient1>();
        serviceCollection.AddTransient<ITransient2, Transient2>();
        serviceCollection.AddTransient<ITransient3, Transient3>();
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
                _container.GetService<ITransient1>();
            if (NumbersOfClasses >= 2)
                _container.GetService<ITransient2>();
            if (NumbersOfClasses >= 3)
                _container.GetService<ITransient3>();
        }
    }

    [Benchmark]
    public void MEDI()
    {
        for (var i = 0; i < NumbersOfCalls; i++)
        {
            if (NumbersOfClasses >= 1)
                _provider.GetService<ITransient1>();
            if(NumbersOfClasses >= 2)
                _provider.GetService<ITransient2>();
            if (NumbersOfClasses >= 3)
                _provider.GetService<ITransient3>();
        }
    }
}

[ServiceProvider]
[Transient(typeof(ITransient1), typeof(Transient1))]
[Transient(typeof(ITransient2), typeof(Transient2))]
[Transient(typeof(ITransient3), typeof(Transient3))]
internal partial class ContainerTransient
{
}