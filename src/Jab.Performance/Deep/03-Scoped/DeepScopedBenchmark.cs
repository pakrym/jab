namespace Jab.Performance.Deep.Scoped; 

using BenchmarkDotNet.Attributes;
using Jab.Performance.Deep.Transient;
using Microsoft.Extensions.DependencyInjection;
using MEDI = Microsoft.Extensions.DependencyInjection;

[MemoryDiagnoser]
public class DeepScopedBenchmark
{
    private readonly MEDI.ServiceProvider _provider;
    private readonly DeepContainerScoped _container = new();

    public DeepScopedBenchmark()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IScoped1, Scoped1>();
        serviceCollection.AddScoped<IScoped2, Scoped2>();
        serviceCollection.AddScoped<IScoped3, Scoped3>();
        serviceCollection.AddScoped<IScoped4, Scoped4>();
        serviceCollection.AddScoped<IScoped5, Scoped5>();
        serviceCollection.AddScoped<IScoped6, Scoped6>();
        serviceCollection.AddScoped<IScoped7, Scoped7>();
        serviceCollection.AddScoped<IScoped8, Scoped8>();
        serviceCollection.AddScoped<IScoped9, Scoped9>();
        serviceCollection.AddScoped<IScoped10, Scoped10>();
        serviceCollection.AddScoped<IScoped11, Scoped11>();
        serviceCollection.AddScoped<IScoped12, Scoped12>();
        serviceCollection.AddScoped<IScoped13, Scoped13>();
        serviceCollection.AddScoped<IScoped14, Scoped14>();
        serviceCollection.AddScoped<IScoped15, Scoped15>();
        serviceCollection.AddScoped<IScoped16, Scoped16>();
        serviceCollection.AddScoped<IScoped17, Scoped17>();
        serviceCollection.AddScoped<IScoped18, Scoped18>();
        serviceCollection.AddScoped<IScoped19, Scoped19>();
        serviceCollection.AddScoped<IScoped20, Scoped20>();
        serviceCollection.AddScoped<IScoped21, Scoped21>();
        serviceCollection.AddScoped<IScoped22, Scoped22>();
        serviceCollection.AddScoped<IScoped23, Scoped23>();
        serviceCollection.AddScoped<IScoped24, Scoped24>();
        serviceCollection.AddScoped<IScoped25, Scoped25>();
        serviceCollection.AddScoped<IScoped26, Scoped26>();
        serviceCollection.AddScoped<IScoped27, Scoped27>();
        serviceCollection.AddScoped<IScoped28, Scoped28>();
        serviceCollection.AddScoped<IScoped29, Scoped29>();
        serviceCollection.AddScoped<IScoped30, Scoped30>();
        serviceCollection.AddScoped<IScoped31, Scoped31>();
        serviceCollection.AddScoped<IScoped32, Scoped32>();
        serviceCollection.AddScoped<IScoped33, Scoped33>();
        serviceCollection.AddScoped<IScoped34, Scoped34>();
        serviceCollection.AddScoped<IScoped35, Scoped35>();
        serviceCollection.AddScoped<IScoped36, Scoped36>();
        serviceCollection.AddScoped<IScoped37, Scoped37>();
        serviceCollection.AddScoped<IScoped38, Scoped38>();
        serviceCollection.AddScoped<IScoped39, Scoped39>();
        serviceCollection.AddScoped<IScoped40, Scoped40>();
        serviceCollection.AddScoped<IScoped41, Scoped41>();
        serviceCollection.AddScoped<IScoped42, Scoped42>();
        serviceCollection.AddScoped<IScoped43, Scoped43>();
        serviceCollection.AddScoped<IScoped44, Scoped44>();
        serviceCollection.AddScoped<IScoped45, Scoped45>();
        serviceCollection.AddScoped<IScoped46, Scoped46>();
        serviceCollection.AddScoped<IScoped47, Scoped47>();
        serviceCollection.AddScoped<IScoped48, Scoped48>();
        serviceCollection.AddScoped<IScoped49, Scoped49>();
        serviceCollection.AddScoped<IScoped50, Scoped50>();
        serviceCollection.AddScoped<IScoped51, Scoped51>();
        serviceCollection.AddScoped<IScoped52, Scoped52>();
        serviceCollection.AddScoped<IScoped53, Scoped53>();
        serviceCollection.AddScoped<IScoped54, Scoped54>();
        serviceCollection.AddScoped<IScoped55, Scoped55>();
        serviceCollection.AddScoped<IScoped56, Scoped56>();
        serviceCollection.AddScoped<IScoped57, Scoped57>();
        serviceCollection.AddScoped<IScoped58, Scoped58>();
        serviceCollection.AddScoped<IScoped59, Scoped59>();
        serviceCollection.AddScoped<IScoped60, Scoped60>();
        serviceCollection.AddScoped<IScoped61, Scoped61>();
        serviceCollection.AddScoped<IScoped62, Scoped62>();
        serviceCollection.AddScoped<IScoped63, Scoped63>();
        serviceCollection.AddScoped<IScoped64, Scoped64>();
        serviceCollection.AddScoped<IScoped65, Scoped65>();
        serviceCollection.AddScoped<IScoped66, Scoped66>();
        serviceCollection.AddScoped<IScoped67, Scoped67>();
        serviceCollection.AddScoped<IScoped68, Scoped68>();
        serviceCollection.AddScoped<IScoped69, Scoped69>();
        serviceCollection.AddScoped<IScoped70, Scoped70>();
        serviceCollection.AddScoped<IScoped71, Scoped71>();
        serviceCollection.AddScoped<IScoped72, Scoped72>();
        serviceCollection.AddScoped<IScoped73, Scoped73>();
        serviceCollection.AddScoped<IScoped74, Scoped74>();
        serviceCollection.AddScoped<IScoped75, Scoped75>();
        serviceCollection.AddScoped<IScoped76, Scoped76>();
        serviceCollection.AddScoped<IScoped77, Scoped77>();
        serviceCollection.AddScoped<IScoped78, Scoped78>();
        serviceCollection.AddScoped<IScoped79, Scoped79>();
        serviceCollection.AddScoped<IScoped80, Scoped80>();
        serviceCollection.AddScoped<IScoped81, Scoped81>();
        serviceCollection.AddScoped<IScoped82, Scoped82>();
        serviceCollection.AddScoped<IScoped83, Scoped83>();
        serviceCollection.AddScoped<IScoped84, Scoped84>();
        serviceCollection.AddScoped<IScoped85, Scoped85>();
        serviceCollection.AddScoped<IScoped86, Scoped86>();
        serviceCollection.AddScoped<IScoped87, Scoped87>();
        serviceCollection.AddScoped<IScoped88, Scoped88>();
        serviceCollection.AddScoped<IScoped89, Scoped89>();
        serviceCollection.AddScoped<IScoped90, Scoped90>();
        serviceCollection.AddScoped<IScoped91, Scoped91>();
        serviceCollection.AddScoped<IScoped92, Scoped92>();
        serviceCollection.AddScoped<IScoped93, Scoped93>();
        serviceCollection.AddScoped<IScoped94, Scoped94>();
        serviceCollection.AddScoped<IScoped95, Scoped95>();
        serviceCollection.AddScoped<IScoped96, Scoped96>();
        serviceCollection.AddScoped<IScoped97, Scoped97>();
        serviceCollection.AddScoped<IScoped98, Scoped98>();
        serviceCollection.AddScoped<IScoped99, Scoped99>();
        serviceCollection.AddScoped<IScoped100, Scoped100>();

        _provider = serviceCollection.BuildServiceProvider();
    }

    [Params(1, 10, 100)]
    public int NumbersOfCalls { get; set; }

    [Params(1, 10, 100)]
    public int Deep { get; set; }

    [Benchmark(Baseline = true)]
    public void Jab()
    {
        for (var i = 0; i < NumbersOfCalls; i++)
        {
            using var scope = _container.CreateScope();

            if (Deep == 1)
                scope.GetService<IScoped100>();
            if (Deep == 10)
                scope.GetService<IScoped90>();
            if (Deep == 100)
                scope.GetService<IScoped1>();
        }
    }

    [Benchmark]
    public void MEDI()
    {
        for (var i = 0; i < NumbersOfCalls; i++)
        {
            using var scope = _provider.CreateScope();

            if (Deep == 1)
                scope.ServiceProvider.GetService<IScoped100>();
            if (Deep == 10)
                scope.ServiceProvider.GetService<IScoped90>();
            if (Deep == 100)
                scope.ServiceProvider.GetService<IScoped1>();
        }
    }
}

[ServiceProvider]
[Scoped(typeof(IScoped1), typeof(Scoped1))]
[Scoped(typeof(IScoped2), typeof(Scoped2))]
[Scoped(typeof(IScoped3), typeof(Scoped3))]
[Scoped(typeof(IScoped4), typeof(Scoped4))]
[Scoped(typeof(IScoped5), typeof(Scoped5))]
[Scoped(typeof(IScoped6), typeof(Scoped6))]
[Scoped(typeof(IScoped7), typeof(Scoped7))]
[Scoped(typeof(IScoped8), typeof(Scoped8))]
[Scoped(typeof(IScoped9), typeof(Scoped9))]
[Scoped(typeof(IScoped10), typeof(Scoped10))]
[Scoped(typeof(IScoped11), typeof(Scoped11))]
[Scoped(typeof(IScoped12), typeof(Scoped12))]
[Scoped(typeof(IScoped13), typeof(Scoped13))]
[Scoped(typeof(IScoped14), typeof(Scoped14))]
[Scoped(typeof(IScoped15), typeof(Scoped15))]
[Scoped(typeof(IScoped16), typeof(Scoped16))]
[Scoped(typeof(IScoped17), typeof(Scoped17))]
[Scoped(typeof(IScoped18), typeof(Scoped18))]
[Scoped(typeof(IScoped19), typeof(Scoped19))]
[Scoped(typeof(IScoped20), typeof(Scoped20))]
[Scoped(typeof(IScoped21), typeof(Scoped21))]
[Scoped(typeof(IScoped22), typeof(Scoped22))]
[Scoped(typeof(IScoped23), typeof(Scoped23))]
[Scoped(typeof(IScoped24), typeof(Scoped24))]
[Scoped(typeof(IScoped25), typeof(Scoped25))]
[Scoped(typeof(IScoped26), typeof(Scoped26))]
[Scoped(typeof(IScoped27), typeof(Scoped27))]
[Scoped(typeof(IScoped28), typeof(Scoped28))]
[Scoped(typeof(IScoped29), typeof(Scoped29))]
[Scoped(typeof(IScoped30), typeof(Scoped30))]
[Scoped(typeof(IScoped31), typeof(Scoped31))]
[Scoped(typeof(IScoped32), typeof(Scoped32))]
[Scoped(typeof(IScoped33), typeof(Scoped33))]
[Scoped(typeof(IScoped34), typeof(Scoped34))]
[Scoped(typeof(IScoped35), typeof(Scoped35))]
[Scoped(typeof(IScoped36), typeof(Scoped36))]
[Scoped(typeof(IScoped37), typeof(Scoped37))]
[Scoped(typeof(IScoped38), typeof(Scoped38))]
[Scoped(typeof(IScoped39), typeof(Scoped39))]
[Scoped(typeof(IScoped40), typeof(Scoped40))]
[Scoped(typeof(IScoped41), typeof(Scoped41))]
[Scoped(typeof(IScoped42), typeof(Scoped42))]
[Scoped(typeof(IScoped43), typeof(Scoped43))]
[Scoped(typeof(IScoped44), typeof(Scoped44))]
[Scoped(typeof(IScoped45), typeof(Scoped45))]
[Scoped(typeof(IScoped46), typeof(Scoped46))]
[Scoped(typeof(IScoped47), typeof(Scoped47))]
[Scoped(typeof(IScoped48), typeof(Scoped48))]
[Scoped(typeof(IScoped49), typeof(Scoped49))]
[Scoped(typeof(IScoped50), typeof(Scoped50))]
[Scoped(typeof(IScoped51), typeof(Scoped51))]
[Scoped(typeof(IScoped52), typeof(Scoped52))]
[Scoped(typeof(IScoped53), typeof(Scoped53))]
[Scoped(typeof(IScoped54), typeof(Scoped54))]
[Scoped(typeof(IScoped55), typeof(Scoped55))]
[Scoped(typeof(IScoped56), typeof(Scoped56))]
[Scoped(typeof(IScoped57), typeof(Scoped57))]
[Scoped(typeof(IScoped58), typeof(Scoped58))]
[Scoped(typeof(IScoped59), typeof(Scoped59))]
[Scoped(typeof(IScoped60), typeof(Scoped60))]
[Scoped(typeof(IScoped61), typeof(Scoped61))]
[Scoped(typeof(IScoped62), typeof(Scoped62))]
[Scoped(typeof(IScoped63), typeof(Scoped63))]
[Scoped(typeof(IScoped64), typeof(Scoped64))]
[Scoped(typeof(IScoped65), typeof(Scoped65))]
[Scoped(typeof(IScoped66), typeof(Scoped66))]
[Scoped(typeof(IScoped67), typeof(Scoped67))]
[Scoped(typeof(IScoped68), typeof(Scoped68))]
[Scoped(typeof(IScoped69), typeof(Scoped69))]
[Scoped(typeof(IScoped70), typeof(Scoped70))]
[Scoped(typeof(IScoped71), typeof(Scoped71))]
[Scoped(typeof(IScoped72), typeof(Scoped72))]
[Scoped(typeof(IScoped73), typeof(Scoped73))]
[Scoped(typeof(IScoped74), typeof(Scoped74))]
[Scoped(typeof(IScoped75), typeof(Scoped75))]
[Scoped(typeof(IScoped76), typeof(Scoped76))]
[Scoped(typeof(IScoped77), typeof(Scoped77))]
[Scoped(typeof(IScoped78), typeof(Scoped78))]
[Scoped(typeof(IScoped79), typeof(Scoped79))]
[Scoped(typeof(IScoped80), typeof(Scoped80))]
[Scoped(typeof(IScoped81), typeof(Scoped81))]
[Scoped(typeof(IScoped82), typeof(Scoped82))]
[Scoped(typeof(IScoped83), typeof(Scoped83))]
[Scoped(typeof(IScoped84), typeof(Scoped84))]
[Scoped(typeof(IScoped85), typeof(Scoped85))]
[Scoped(typeof(IScoped86), typeof(Scoped86))]
[Scoped(typeof(IScoped87), typeof(Scoped87))]
[Scoped(typeof(IScoped88), typeof(Scoped88))]
[Scoped(typeof(IScoped89), typeof(Scoped89))]
[Scoped(typeof(IScoped90), typeof(Scoped90))]
[Scoped(typeof(IScoped91), typeof(Scoped91))]
[Scoped(typeof(IScoped92), typeof(Scoped92))]
[Scoped(typeof(IScoped93), typeof(Scoped93))]
[Scoped(typeof(IScoped94), typeof(Scoped94))]
[Scoped(typeof(IScoped95), typeof(Scoped95))]
[Scoped(typeof(IScoped96), typeof(Scoped96))]
[Scoped(typeof(IScoped97), typeof(Scoped97))]
[Scoped(typeof(IScoped98), typeof(Scoped98))]
[Scoped(typeof(IScoped99), typeof(Scoped99))]
[Scoped(typeof(IScoped100), typeof(Scoped100))]
internal partial class DeepContainerScoped
{
}