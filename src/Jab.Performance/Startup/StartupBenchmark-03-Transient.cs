namespace Jab.Performance.Startup;

using BenchmarkDotNet.Attributes;
using Jab;
using Jab.Performance.Basic.Transient;
using Microsoft.Extensions.DependencyInjection;

public partial class StartupBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("03", "Transient", "Jab")]
    public IServiceProvider Jab_Transient()
    {
        var provider = new ContainerStartupTransient();
        return provider.GetService<IServiceProvider>();
    }

    [Benchmark, BenchmarkCategory("03", "Transient", "MEDI")]
    public IServiceProvider MEDI_Transient()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ITransient1, Transient1>();
        serviceCollection.AddTransient<ITransient2, Transient2>();
        serviceCollection.AddTransient<ITransient3, Transient3>();
        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetService<IServiceProvider>()!;
    }

}

[ServiceProvider]
[Transient(typeof(ITransient1), typeof(Transient1))]
[Transient(typeof(ITransient2), typeof(Transient2))]
[Transient(typeof(ITransient3), typeof(Transient3))]
internal partial class ContainerStartupTransient
{
}