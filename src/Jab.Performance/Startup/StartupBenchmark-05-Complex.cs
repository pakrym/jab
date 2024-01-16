namespace Jab.Performance.Startup;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Jab;
using Jab.Performance.Basic.Complex;
using Jab.Performance.Basic.Mixed;
using Jab.Performance.Basic.Singleton;
using Jab.Performance.Basic.Transient;
using Microsoft.Extensions.DependencyInjection;

public partial class StartupBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("05", "Complex", "Jab")]
    public IServiceProvider Jab_Complex()
    {
        var provider = new ContainerStartupComplex();
        return provider.GetService<IServiceProvider>();
    }

    [Benchmark, BenchmarkCategory("05", "Complex", "MEDI")]
    public IServiceProvider MEDI_Complex()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IComplex1, Complex1>();
        serviceCollection.AddScoped<IComplex2, Complex2>();
        serviceCollection.AddScoped<IComplex3, Complex3>();
        serviceCollection.AddTransient<IService1, Service1>();
        serviceCollection.AddTransient<IService2, Service2>();
        serviceCollection.AddTransient<IService3, Service3>();
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
        return provider.GetService<IServiceProvider>()!;
    }
}

[ServiceProvider]
[Scoped(typeof(IComplex1), typeof(Complex1))]
[Scoped(typeof(IComplex2), typeof(Complex2))]
[Scoped(typeof(IComplex3), typeof(Complex3))]
[Transient(typeof(IService1), typeof(Service1))]
[Transient(typeof(IService2), typeof(Service2))]
[Transient(typeof(IService3), typeof(Service3))]
[Transient(typeof(IMix1), typeof(Mix1))]
[Transient(typeof(IMix2), typeof(Mix2))]
[Transient(typeof(IMix3), typeof(Mix3))]
[Transient(typeof(ITransient1), typeof(Transient1))]
[Transient(typeof(ITransient2), typeof(Transient2))]
[Transient(typeof(ITransient3), typeof(Transient3))]
[Singleton(typeof(ISingleton1), typeof(Singleton1))]
[Singleton(typeof(ISingleton2), typeof(Singleton2))]
[Singleton(typeof(ISingleton3), typeof(Singleton3))]
internal partial class ContainerStartupComplex
{
}