namespace Jab.Performance.Startup;

using BenchmarkDotNet.Attributes;
using Jab;
using Jab.Performance.Basic.Scoped;
using Microsoft.Extensions.DependencyInjection;

[MemoryDiagnoser]
public class StartupScopedBenchmark
{
    [Benchmark(Baseline = true)]
    public void Jab()
    {
        var provider = new ContainerStartupScoped();
        var _ = provider.GetService<IServiceProvider>();
    }

    [Benchmark]
    public void MEDI()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IScoped1, Scoped1>();
        serviceCollection.AddScoped<IScoped2, Scoped2>();
        serviceCollection.AddScoped<IScoped3, Scoped3>();
        var provider = serviceCollection.BuildServiceProvider();
        var _ = provider.GetService<IServiceProvider>();
    }

}

[ServiceProvider]
[Scoped(typeof(IScoped1), typeof(Scoped1))]
[Scoped(typeof(IScoped2), typeof(Scoped2))]
[Scoped(typeof(IScoped3), typeof(Scoped3))]
internal partial class ContainerStartupScoped
{
}