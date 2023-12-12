namespace Jab.Performance.Deep.Transient; 

using BenchmarkDotNet.Attributes;
using Jab.Performance.Deep.Singleton;
using Microsoft.Extensions.DependencyInjection;
using MEDI = Microsoft.Extensions.DependencyInjection;

[MemoryDiagnoser]
public class DeepTransientBenchmark
{
    private readonly MEDI.ServiceProvider _provider;
    private readonly DeepContainerTransient _container = new();

    public DeepTransientBenchmark()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ITransient1, Transient1>();
        serviceCollection.AddTransient<ITransient2, Transient2>();
        serviceCollection.AddTransient<ITransient3, Transient3>();
        serviceCollection.AddTransient<ITransient4, Transient4>();
        serviceCollection.AddTransient<ITransient5, Transient5>();
        serviceCollection.AddTransient<ITransient6, Transient6>();
        serviceCollection.AddTransient<ITransient7, Transient7>();
        serviceCollection.AddTransient<ITransient8, Transient8>();
        serviceCollection.AddTransient<ITransient9, Transient9>();
        serviceCollection.AddTransient<ITransient10, Transient10>();
        serviceCollection.AddTransient<ITransient11, Transient11>();
        serviceCollection.AddTransient<ITransient12, Transient12>();
        serviceCollection.AddTransient<ITransient13, Transient13>();
        serviceCollection.AddTransient<ITransient14, Transient14>();
        serviceCollection.AddTransient<ITransient15, Transient15>();
        serviceCollection.AddTransient<ITransient16, Transient16>();
        serviceCollection.AddTransient<ITransient17, Transient17>();
        serviceCollection.AddTransient<ITransient18, Transient18>();
        serviceCollection.AddTransient<ITransient19, Transient19>();
        serviceCollection.AddTransient<ITransient20, Transient20>();
        serviceCollection.AddTransient<ITransient21, Transient21>();
        serviceCollection.AddTransient<ITransient22, Transient22>();
        serviceCollection.AddTransient<ITransient23, Transient23>();
        serviceCollection.AddTransient<ITransient24, Transient24>();
        serviceCollection.AddTransient<ITransient25, Transient25>();
        serviceCollection.AddTransient<ITransient26, Transient26>();
        serviceCollection.AddTransient<ITransient27, Transient27>();
        serviceCollection.AddTransient<ITransient28, Transient28>();
        serviceCollection.AddTransient<ITransient29, Transient29>();
        serviceCollection.AddTransient<ITransient30, Transient30>();
        serviceCollection.AddTransient<ITransient31, Transient31>();
        serviceCollection.AddTransient<ITransient32, Transient32>();
        serviceCollection.AddTransient<ITransient33, Transient33>();
        serviceCollection.AddTransient<ITransient34, Transient34>();
        serviceCollection.AddTransient<ITransient35, Transient35>();
        serviceCollection.AddTransient<ITransient36, Transient36>();
        serviceCollection.AddTransient<ITransient37, Transient37>();
        serviceCollection.AddTransient<ITransient38, Transient38>();
        serviceCollection.AddTransient<ITransient39, Transient39>();
        serviceCollection.AddTransient<ITransient40, Transient40>();
        serviceCollection.AddTransient<ITransient41, Transient41>();
        serviceCollection.AddTransient<ITransient42, Transient42>();
        serviceCollection.AddTransient<ITransient43, Transient43>();
        serviceCollection.AddTransient<ITransient44, Transient44>();
        serviceCollection.AddTransient<ITransient45, Transient45>();
        serviceCollection.AddTransient<ITransient46, Transient46>();
        serviceCollection.AddTransient<ITransient47, Transient47>();
        serviceCollection.AddTransient<ITransient48, Transient48>();
        serviceCollection.AddTransient<ITransient49, Transient49>();
        serviceCollection.AddTransient<ITransient50, Transient50>();
        serviceCollection.AddTransient<ITransient51, Transient51>();
        serviceCollection.AddTransient<ITransient52, Transient52>();
        serviceCollection.AddTransient<ITransient53, Transient53>();
        serviceCollection.AddTransient<ITransient54, Transient54>();
        serviceCollection.AddTransient<ITransient55, Transient55>();
        serviceCollection.AddTransient<ITransient56, Transient56>();
        serviceCollection.AddTransient<ITransient57, Transient57>();
        serviceCollection.AddTransient<ITransient58, Transient58>();
        serviceCollection.AddTransient<ITransient59, Transient59>();
        serviceCollection.AddTransient<ITransient60, Transient60>();
        serviceCollection.AddTransient<ITransient61, Transient61>();
        serviceCollection.AddTransient<ITransient62, Transient62>();
        serviceCollection.AddTransient<ITransient63, Transient63>();
        serviceCollection.AddTransient<ITransient64, Transient64>();
        serviceCollection.AddTransient<ITransient65, Transient65>();
        serviceCollection.AddTransient<ITransient66, Transient66>();
        serviceCollection.AddTransient<ITransient67, Transient67>();
        serviceCollection.AddTransient<ITransient68, Transient68>();
        serviceCollection.AddTransient<ITransient69, Transient69>();
        serviceCollection.AddTransient<ITransient70, Transient70>();
        serviceCollection.AddTransient<ITransient71, Transient71>();
        serviceCollection.AddTransient<ITransient72, Transient72>();
        serviceCollection.AddTransient<ITransient73, Transient73>();
        serviceCollection.AddTransient<ITransient74, Transient74>();
        serviceCollection.AddTransient<ITransient75, Transient75>();
        serviceCollection.AddTransient<ITransient76, Transient76>();
        serviceCollection.AddTransient<ITransient77, Transient77>();
        serviceCollection.AddTransient<ITransient78, Transient78>();
        serviceCollection.AddTransient<ITransient79, Transient79>();
        serviceCollection.AddTransient<ITransient80, Transient80>();
        serviceCollection.AddTransient<ITransient81, Transient81>();
        serviceCollection.AddTransient<ITransient82, Transient82>();
        serviceCollection.AddTransient<ITransient83, Transient83>();
        serviceCollection.AddTransient<ITransient84, Transient84>();
        serviceCollection.AddTransient<ITransient85, Transient85>();
        serviceCollection.AddTransient<ITransient86, Transient86>();
        serviceCollection.AddTransient<ITransient87, Transient87>();
        serviceCollection.AddTransient<ITransient88, Transient88>();
        serviceCollection.AddTransient<ITransient89, Transient89>();
        serviceCollection.AddTransient<ITransient90, Transient90>();
        serviceCollection.AddTransient<ITransient91, Transient91>();
        serviceCollection.AddTransient<ITransient92, Transient92>();
        serviceCollection.AddTransient<ITransient93, Transient93>();
        serviceCollection.AddTransient<ITransient94, Transient94>();
        serviceCollection.AddTransient<ITransient95, Transient95>();
        serviceCollection.AddTransient<ITransient96, Transient96>();
        serviceCollection.AddTransient<ITransient97, Transient97>();
        serviceCollection.AddTransient<ITransient98, Transient98>();
        serviceCollection.AddTransient<ITransient99, Transient99>();
        serviceCollection.AddTransient<ITransient100, Transient100>();

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
            if (Deep == 1)
                _container.GetService<ITransient100>();
            if (Deep == 10)
                _container.GetService<ITransient90>();
            if (Deep == 100)
                _container.GetService<ITransient1>();
        }
    }

    [Benchmark]
    public void MEDI()
    {
        for (var i = 0; i < NumbersOfCalls; i++)
        {
            if (Deep == 1)
                _provider.GetService<ITransient100>();
            if (Deep == 10)
                _provider.GetService<ITransient90>();
            if (Deep == 100)
                _provider.GetService<ITransient1>();
        }
    }
}

[ServiceProvider]
[Transient(typeof(ITransient1), typeof(Transient1))]
[Transient(typeof(ITransient2), typeof(Transient2))]
[Transient(typeof(ITransient3), typeof(Transient3))]
[Transient(typeof(ITransient4), typeof(Transient4))]
[Transient(typeof(ITransient5), typeof(Transient5))]
[Transient(typeof(ITransient6), typeof(Transient6))]
[Transient(typeof(ITransient7), typeof(Transient7))]
[Transient(typeof(ITransient8), typeof(Transient8))]
[Transient(typeof(ITransient9), typeof(Transient9))]
[Transient(typeof(ITransient10), typeof(Transient10))]
[Transient(typeof(ITransient11), typeof(Transient11))]
[Transient(typeof(ITransient12), typeof(Transient12))]
[Transient(typeof(ITransient13), typeof(Transient13))]
[Transient(typeof(ITransient14), typeof(Transient14))]
[Transient(typeof(ITransient15), typeof(Transient15))]
[Transient(typeof(ITransient16), typeof(Transient16))]
[Transient(typeof(ITransient17), typeof(Transient17))]
[Transient(typeof(ITransient18), typeof(Transient18))]
[Transient(typeof(ITransient19), typeof(Transient19))]
[Transient(typeof(ITransient20), typeof(Transient20))]
[Transient(typeof(ITransient21), typeof(Transient21))]
[Transient(typeof(ITransient22), typeof(Transient22))]
[Transient(typeof(ITransient23), typeof(Transient23))]
[Transient(typeof(ITransient24), typeof(Transient24))]
[Transient(typeof(ITransient25), typeof(Transient25))]
[Transient(typeof(ITransient26), typeof(Transient26))]
[Transient(typeof(ITransient27), typeof(Transient27))]
[Transient(typeof(ITransient28), typeof(Transient28))]
[Transient(typeof(ITransient29), typeof(Transient29))]
[Transient(typeof(ITransient30), typeof(Transient30))]
[Transient(typeof(ITransient31), typeof(Transient31))]
[Transient(typeof(ITransient32), typeof(Transient32))]
[Transient(typeof(ITransient33), typeof(Transient33))]
[Transient(typeof(ITransient34), typeof(Transient34))]
[Transient(typeof(ITransient35), typeof(Transient35))]
[Transient(typeof(ITransient36), typeof(Transient36))]
[Transient(typeof(ITransient37), typeof(Transient37))]
[Transient(typeof(ITransient38), typeof(Transient38))]
[Transient(typeof(ITransient39), typeof(Transient39))]
[Transient(typeof(ITransient40), typeof(Transient40))]
[Transient(typeof(ITransient41), typeof(Transient41))]
[Transient(typeof(ITransient42), typeof(Transient42))]
[Transient(typeof(ITransient43), typeof(Transient43))]
[Transient(typeof(ITransient44), typeof(Transient44))]
[Transient(typeof(ITransient45), typeof(Transient45))]
[Transient(typeof(ITransient46), typeof(Transient46))]
[Transient(typeof(ITransient47), typeof(Transient47))]
[Transient(typeof(ITransient48), typeof(Transient48))]
[Transient(typeof(ITransient49), typeof(Transient49))]
[Transient(typeof(ITransient50), typeof(Transient50))]
[Transient(typeof(ITransient51), typeof(Transient51))]
[Transient(typeof(ITransient52), typeof(Transient52))]
[Transient(typeof(ITransient53), typeof(Transient53))]
[Transient(typeof(ITransient54), typeof(Transient54))]
[Transient(typeof(ITransient55), typeof(Transient55))]
[Transient(typeof(ITransient56), typeof(Transient56))]
[Transient(typeof(ITransient57), typeof(Transient57))]
[Transient(typeof(ITransient58), typeof(Transient58))]
[Transient(typeof(ITransient59), typeof(Transient59))]
[Transient(typeof(ITransient60), typeof(Transient60))]
[Transient(typeof(ITransient61), typeof(Transient61))]
[Transient(typeof(ITransient62), typeof(Transient62))]
[Transient(typeof(ITransient63), typeof(Transient63))]
[Transient(typeof(ITransient64), typeof(Transient64))]
[Transient(typeof(ITransient65), typeof(Transient65))]
[Transient(typeof(ITransient66), typeof(Transient66))]
[Transient(typeof(ITransient67), typeof(Transient67))]
[Transient(typeof(ITransient68), typeof(Transient68))]
[Transient(typeof(ITransient69), typeof(Transient69))]
[Transient(typeof(ITransient70), typeof(Transient70))]
[Transient(typeof(ITransient71), typeof(Transient71))]
[Transient(typeof(ITransient72), typeof(Transient72))]
[Transient(typeof(ITransient73), typeof(Transient73))]
[Transient(typeof(ITransient74), typeof(Transient74))]
[Transient(typeof(ITransient75), typeof(Transient75))]
[Transient(typeof(ITransient76), typeof(Transient76))]
[Transient(typeof(ITransient77), typeof(Transient77))]
[Transient(typeof(ITransient78), typeof(Transient78))]
[Transient(typeof(ITransient79), typeof(Transient79))]
[Transient(typeof(ITransient80), typeof(Transient80))]
[Transient(typeof(ITransient81), typeof(Transient81))]
[Transient(typeof(ITransient82), typeof(Transient82))]
[Transient(typeof(ITransient83), typeof(Transient83))]
[Transient(typeof(ITransient84), typeof(Transient84))]
[Transient(typeof(ITransient85), typeof(Transient85))]
[Transient(typeof(ITransient86), typeof(Transient86))]
[Transient(typeof(ITransient87), typeof(Transient87))]
[Transient(typeof(ITransient88), typeof(Transient88))]
[Transient(typeof(ITransient89), typeof(Transient89))]
[Transient(typeof(ITransient90), typeof(Transient90))]
[Transient(typeof(ITransient91), typeof(Transient91))]
[Transient(typeof(ITransient92), typeof(Transient92))]
[Transient(typeof(ITransient93), typeof(Transient93))]
[Transient(typeof(ITransient94), typeof(Transient94))]
[Transient(typeof(ITransient95), typeof(Transient95))]
[Transient(typeof(ITransient96), typeof(Transient96))]
[Transient(typeof(ITransient97), typeof(Transient97))]
[Transient(typeof(ITransient98), typeof(Transient98))]
[Transient(typeof(ITransient99), typeof(Transient99))]
[Transient(typeof(ITransient100), typeof(Transient100))]
internal partial class DeepContainerTransient
{
}