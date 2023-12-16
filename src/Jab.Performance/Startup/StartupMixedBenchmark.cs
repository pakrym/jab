namespace Jab.Performance.Startup;

using BenchmarkDotNet.Attributes;
using Jab;
using Jab.Performance.Basic.Singleton;
using Jab.Performance.Basic.Transient;
using Jab.Performance.Basic.Mixed;
using Microsoft.Extensions.DependencyInjection;

[MemoryDiagnoser]
public class StartupMixedBenchmark
{
    [Benchmark(Baseline = true)]
    public void Jab()
    {
        var provider = new ContainerStartupMixed();
        var _ = provider.GetService<IServiceProvider>();
    }

    [Benchmark]
    public void MEDI()
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
        var provider = serviceCollection.BuildServiceProvider();
        var _ = provider.GetService<IServiceProvider>();
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
internal partial class ContainerStartupMixed
{
}