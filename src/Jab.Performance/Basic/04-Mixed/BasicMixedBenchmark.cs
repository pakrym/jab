namespace Jab.Performance.Basic.Mixed;

using BenchmarkDotNet.Attributes;
using Jab.Performance.Basic.Singleton;
using Jab.Performance.Basic.Transient;
using Microsoft.Extensions.DependencyInjection;
using MEDI = Microsoft.Extensions.DependencyInjection;

[ShortRunJob]
[MemoryDiagnoser]
public class BasicMixedBenchmark
{
    private readonly MEDI.ServiceProvider _provider;
    private readonly ContainerMixed _container = new();

    public BasicMixedBenchmark()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<IMix1, Mix1>();
        serviceCollection.AddTransient<IMix2, Mix2>();
        serviceCollection.AddTransient<IMix3, Mix3>();
        serviceCollection.AddTransient<ITransient1, Transient1>();
        serviceCollection.AddTransient<ITransient2, Transient2>();
        serviceCollection.AddTransient<ITransient3, Transient3>();
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
            using var scope = _container.CreateScope();

            if (NumbersOfClasses >= 1)
                scope.GetService<IMix1>();
            if (NumbersOfClasses >= 2)
                scope.GetService<IMix2>();
            if (NumbersOfClasses >= 3)
                scope.GetService<IMix3>();
        }
    }

    [Benchmark]
    public void MEDI()
    {
        for (var i = 0; i < NumbersOfCalls; i++)
        {
            using var scope = _provider.CreateScope();

            if (NumbersOfClasses >= 1)
                scope.ServiceProvider.GetService<IMix1>();
            if (NumbersOfClasses >= 2)
                scope.ServiceProvider.GetService<IMix2>();
            if (NumbersOfClasses >= 3)
                scope.ServiceProvider.GetService<IMix3>();
        }
    }
}

[ServiceProvider]
[Transient(typeof(IMix1), typeof(Mix1))]
[Transient(typeof(IMix2), typeof(Mix2))]
[Transient(typeof(IMix3), typeof(Mix3))]
[Transient(typeof(ITransient1), typeof(Transient1))]
[Transient(typeof(ITransient2), typeof(Transient2))]
[Transient(typeof(ITransient3), typeof(Transient3))]
[Singleton(typeof(ISingleton1), typeof(Singleton1))]
[Singleton(typeof(ISingleton2), typeof(Singleton2))]
[Singleton(typeof(ISingleton3), typeof(Singleton3))]
internal partial class ContainerMixed
{
}