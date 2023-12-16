namespace Jab.Performance.Startup;

using BenchmarkDotNet.Attributes;
using Jab;
using Jab.Performance.Basic.Transient;
using Microsoft.Extensions.DependencyInjection;

[MemoryDiagnoser]
public class StartupTransientBenchmark
{
    [Benchmark(Baseline = true)]
    public void Jab()
    {
        var provider = new ContainerStartupTransient();
        var _ = provider.GetService<IServiceProvider>();
    }

    [Benchmark]
    public void MEDI()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ITransient1, Transient1>();
        serviceCollection.AddTransient<ITransient2, Transient2>();
        serviceCollection.AddTransient<ITransient3, Transient3>();
        var provider = serviceCollection.BuildServiceProvider();
        var _ = provider.GetService<IServiceProvider>();
    }

}

[ServiceProvider]
[Transient(typeof(ITransient1), typeof(Transient1))]
[Transient(typeof(ITransient2), typeof(Transient2))]
[Transient(typeof(ITransient3), typeof(Transient3))]
internal partial class ContainerStartupTransient
{
}