namespace Jab.Performance.Startup;

using BenchmarkDotNet.Attributes;
using Jab;
using Jab.Performance.Basic.Scoped;
using Microsoft.Extensions.DependencyInjection;

public partial class StartupBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("02", "Scoped", "Jab")]
    public IServiceProvider Jab_Scoped()
    {
        var provider = new ContainerStartupScoped();
        return provider.GetService<IServiceProvider>();
    }

    [Benchmark, BenchmarkCategory("02", "Scoped", "MEDI")]
    public IServiceProvider MEDI_Scoped()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IScoped1, Scoped1>();
        serviceCollection.AddScoped<IScoped2, Scoped2>();
        serviceCollection.AddScoped<IScoped3, Scoped3>();
        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetService<IServiceProvider>()!;
    }

}

[ServiceProvider]
[Scoped(typeof(IScoped1), typeof(Scoped1))]
[Scoped(typeof(IScoped2), typeof(Scoped2))]
[Scoped(typeof(IScoped3), typeof(Scoped3))]
internal partial class ContainerStartupScoped
{
}