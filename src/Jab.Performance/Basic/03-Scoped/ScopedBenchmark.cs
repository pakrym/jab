namespace Jab.Performance.Basic.Scoped; 

using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MEDI = Microsoft.Extensions.DependencyInjection;


[MemoryDiagnoser]
public class ScopedBenchmark
{
    private readonly MEDI.ServiceProvider _provider;
    private readonly ContainerScoped _container = new();

    public ScopedBenchmark()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IScoped1, Scoped1>();
        serviceCollection.AddScoped<IScoped2, Scoped2>();
        serviceCollection.AddScoped<IScoped3, Scoped3>();
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
            using var scope = _container.CreateScope();

            if (NumbersOfClasses >= 1)
                scope.GetService<IScoped1>();
            if (NumbersOfClasses >= 2)
                scope.GetService<IScoped2>();
            if (NumbersOfClasses >= 3)
                scope.GetService<IScoped3>();
        }
    }

    [Benchmark]
    public void MEDI()
    {
        for (var i = 0; i < NumbersOfCalls; i++)
        {
            using var scope = _provider.CreateScope();

            if (NumbersOfClasses >= 1)
                scope.ServiceProvider.GetService<IScoped1>();
            if(NumbersOfClasses >= 2)
                scope.ServiceProvider.GetService<IScoped2>();
            if (NumbersOfClasses >= 3)
                scope.ServiceProvider.GetService<IScoped3>();
        }
    }
}

[ServiceProvider]
[Scoped(typeof(IScoped1), typeof(Scoped1))]
[Scoped(typeof(IScoped2), typeof(Scoped2))]
[Scoped(typeof(IScoped3), typeof(Scoped3))]
internal partial class ContainerScoped
{
}