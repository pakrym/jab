namespace Jab.Performance.Deep.Singleton;

using BenchmarkDotNet.Attributes;
using Jab.Performance.Basic.Mixed;
using Microsoft.Extensions.DependencyInjection;
using static System.Formats.Asn1.AsnWriter;
using MEDI = Microsoft.Extensions.DependencyInjection;


[MemoryDiagnoser]
public class DeepSingletonBenchmark
{
    private readonly MEDI.ServiceProvider _provider;
    private readonly DeepContainerSingleton _container = new();

    public DeepSingletonBenchmark()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ISingleton1, Singleton1>();
        serviceCollection.AddSingleton<ISingleton2, Singleton2>();
        serviceCollection.AddSingleton<ISingleton3, Singleton3>();
        serviceCollection.AddSingleton<ISingleton4, Singleton4>();
        serviceCollection.AddSingleton<ISingleton5, Singleton5>();
        serviceCollection.AddSingleton<ISingleton6, Singleton6>();
        serviceCollection.AddSingleton<ISingleton7, Singleton7>();
        serviceCollection.AddSingleton<ISingleton8, Singleton8>();
        serviceCollection.AddSingleton<ISingleton9, Singleton9>();
        serviceCollection.AddSingleton<ISingleton10, Singleton10>();
        serviceCollection.AddSingleton<ISingleton11, Singleton11>();
        serviceCollection.AddSingleton<ISingleton12, Singleton12>();
        serviceCollection.AddSingleton<ISingleton13, Singleton13>();
        serviceCollection.AddSingleton<ISingleton14, Singleton14>();
        serviceCollection.AddSingleton<ISingleton15, Singleton15>();
        serviceCollection.AddSingleton<ISingleton16, Singleton16>();
        serviceCollection.AddSingleton<ISingleton17, Singleton17>();
        serviceCollection.AddSingleton<ISingleton18, Singleton18>();
        serviceCollection.AddSingleton<ISingleton19, Singleton19>();
        serviceCollection.AddSingleton<ISingleton20, Singleton20>();
        serviceCollection.AddSingleton<ISingleton21, Singleton21>();
        serviceCollection.AddSingleton<ISingleton22, Singleton22>();
        serviceCollection.AddSingleton<ISingleton23, Singleton23>();
        serviceCollection.AddSingleton<ISingleton24, Singleton24>();
        serviceCollection.AddSingleton<ISingleton25, Singleton25>();
        serviceCollection.AddSingleton<ISingleton26, Singleton26>();
        serviceCollection.AddSingleton<ISingleton27, Singleton27>();
        serviceCollection.AddSingleton<ISingleton28, Singleton28>();
        serviceCollection.AddSingleton<ISingleton29, Singleton29>();
        serviceCollection.AddSingleton<ISingleton30, Singleton30>();
        serviceCollection.AddSingleton<ISingleton31, Singleton31>();
        serviceCollection.AddSingleton<ISingleton32, Singleton32>();
        serviceCollection.AddSingleton<ISingleton33, Singleton33>();
        serviceCollection.AddSingleton<ISingleton34, Singleton34>();
        serviceCollection.AddSingleton<ISingleton35, Singleton35>();
        serviceCollection.AddSingleton<ISingleton36, Singleton36>();
        serviceCollection.AddSingleton<ISingleton37, Singleton37>();
        serviceCollection.AddSingleton<ISingleton38, Singleton38>();
        serviceCollection.AddSingleton<ISingleton39, Singleton39>();
        serviceCollection.AddSingleton<ISingleton40, Singleton40>();
        serviceCollection.AddSingleton<ISingleton41, Singleton41>();
        serviceCollection.AddSingleton<ISingleton42, Singleton42>();
        serviceCollection.AddSingleton<ISingleton43, Singleton43>();
        serviceCollection.AddSingleton<ISingleton44, Singleton44>();
        serviceCollection.AddSingleton<ISingleton45, Singleton45>();
        serviceCollection.AddSingleton<ISingleton46, Singleton46>();
        serviceCollection.AddSingleton<ISingleton47, Singleton47>();
        serviceCollection.AddSingleton<ISingleton48, Singleton48>();
        serviceCollection.AddSingleton<ISingleton49, Singleton49>();
        serviceCollection.AddSingleton<ISingleton50, Singleton50>();
        serviceCollection.AddSingleton<ISingleton51, Singleton51>();
        serviceCollection.AddSingleton<ISingleton52, Singleton52>();
        serviceCollection.AddSingleton<ISingleton53, Singleton53>();
        serviceCollection.AddSingleton<ISingleton54, Singleton54>();
        serviceCollection.AddSingleton<ISingleton55, Singleton55>();
        serviceCollection.AddSingleton<ISingleton56, Singleton56>();
        serviceCollection.AddSingleton<ISingleton57, Singleton57>();
        serviceCollection.AddSingleton<ISingleton58, Singleton58>();
        serviceCollection.AddSingleton<ISingleton59, Singleton59>();
        serviceCollection.AddSingleton<ISingleton60, Singleton60>();
        serviceCollection.AddSingleton<ISingleton61, Singleton61>();
        serviceCollection.AddSingleton<ISingleton62, Singleton62>();
        serviceCollection.AddSingleton<ISingleton63, Singleton63>();
        serviceCollection.AddSingleton<ISingleton64, Singleton64>();
        serviceCollection.AddSingleton<ISingleton65, Singleton65>();
        serviceCollection.AddSingleton<ISingleton66, Singleton66>();
        serviceCollection.AddSingleton<ISingleton67, Singleton67>();
        serviceCollection.AddSingleton<ISingleton68, Singleton68>();
        serviceCollection.AddSingleton<ISingleton69, Singleton69>();
        serviceCollection.AddSingleton<ISingleton70, Singleton70>();
        serviceCollection.AddSingleton<ISingleton71, Singleton71>();
        serviceCollection.AddSingleton<ISingleton72, Singleton72>();
        serviceCollection.AddSingleton<ISingleton73, Singleton73>();
        serviceCollection.AddSingleton<ISingleton74, Singleton74>();
        serviceCollection.AddSingleton<ISingleton75, Singleton75>();
        serviceCollection.AddSingleton<ISingleton76, Singleton76>();
        serviceCollection.AddSingleton<ISingleton77, Singleton77>();
        serviceCollection.AddSingleton<ISingleton78, Singleton78>();
        serviceCollection.AddSingleton<ISingleton79, Singleton79>();
        serviceCollection.AddSingleton<ISingleton80, Singleton80>();
        serviceCollection.AddSingleton<ISingleton81, Singleton81>();
        serviceCollection.AddSingleton<ISingleton82, Singleton82>();
        serviceCollection.AddSingleton<ISingleton83, Singleton83>();
        serviceCollection.AddSingleton<ISingleton84, Singleton84>();
        serviceCollection.AddSingleton<ISingleton85, Singleton85>();
        serviceCollection.AddSingleton<ISingleton86, Singleton86>();
        serviceCollection.AddSingleton<ISingleton87, Singleton87>();
        serviceCollection.AddSingleton<ISingleton88, Singleton88>();
        serviceCollection.AddSingleton<ISingleton89, Singleton89>();
        serviceCollection.AddSingleton<ISingleton90, Singleton90>();
        serviceCollection.AddSingleton<ISingleton91, Singleton91>();
        serviceCollection.AddSingleton<ISingleton92, Singleton92>();
        serviceCollection.AddSingleton<ISingleton93, Singleton93>();
        serviceCollection.AddSingleton<ISingleton94, Singleton94>();
        serviceCollection.AddSingleton<ISingleton95, Singleton95>();
        serviceCollection.AddSingleton<ISingleton96, Singleton96>();
        serviceCollection.AddSingleton<ISingleton97, Singleton97>();
        serviceCollection.AddSingleton<ISingleton98, Singleton98>();
        serviceCollection.AddSingleton<ISingleton99, Singleton99>();
        serviceCollection.AddSingleton<ISingleton100, Singleton100>();

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
            if (Deep == 1 )
                _container.GetService<ISingleton100>();
            if (Deep == 10)
                _container.GetService<ISingleton90>();
            if (Deep == 100)
                _container.GetService<ISingleton1>();
        }
    }

    [Benchmark]
    public void MEDI()
    {
        for (var i = 0; i < NumbersOfCalls; i++)
        {
            if (Deep == 1)
                _provider.GetService<ISingleton100>();
            if (Deep == 10)
                _provider.GetService<ISingleton90>();
            if (Deep == 100)
                _provider.GetService<ISingleton1>();
        }
    }
}

[ServiceProvider]
[Singleton(typeof(ISingleton1), typeof(Singleton1))]
[Singleton(typeof(ISingleton2), typeof(Singleton2))]
[Singleton(typeof(ISingleton3), typeof(Singleton3))]
[Singleton(typeof(ISingleton4), typeof(Singleton4))]
[Singleton(typeof(ISingleton5), typeof(Singleton5))]
[Singleton(typeof(ISingleton6), typeof(Singleton6))]
[Singleton(typeof(ISingleton7), typeof(Singleton7))]
[Singleton(typeof(ISingleton8), typeof(Singleton8))]
[Singleton(typeof(ISingleton9), typeof(Singleton9))]
[Singleton(typeof(ISingleton10), typeof(Singleton10))]
[Singleton(typeof(ISingleton11), typeof(Singleton11))]
[Singleton(typeof(ISingleton12), typeof(Singleton12))]
[Singleton(typeof(ISingleton13), typeof(Singleton13))]
[Singleton(typeof(ISingleton14), typeof(Singleton14))]
[Singleton(typeof(ISingleton15), typeof(Singleton15))]
[Singleton(typeof(ISingleton16), typeof(Singleton16))]
[Singleton(typeof(ISingleton17), typeof(Singleton17))]
[Singleton(typeof(ISingleton18), typeof(Singleton18))]
[Singleton(typeof(ISingleton19), typeof(Singleton19))]
[Singleton(typeof(ISingleton20), typeof(Singleton20))]
[Singleton(typeof(ISingleton21), typeof(Singleton21))]
[Singleton(typeof(ISingleton22), typeof(Singleton22))]
[Singleton(typeof(ISingleton23), typeof(Singleton23))]
[Singleton(typeof(ISingleton24), typeof(Singleton24))]
[Singleton(typeof(ISingleton25), typeof(Singleton25))]
[Singleton(typeof(ISingleton26), typeof(Singleton26))]
[Singleton(typeof(ISingleton27), typeof(Singleton27))]
[Singleton(typeof(ISingleton28), typeof(Singleton28))]
[Singleton(typeof(ISingleton29), typeof(Singleton29))]
[Singleton(typeof(ISingleton30), typeof(Singleton30))]
[Singleton(typeof(ISingleton31), typeof(Singleton31))]
[Singleton(typeof(ISingleton32), typeof(Singleton32))]
[Singleton(typeof(ISingleton33), typeof(Singleton33))]
[Singleton(typeof(ISingleton34), typeof(Singleton34))]
[Singleton(typeof(ISingleton35), typeof(Singleton35))]
[Singleton(typeof(ISingleton36), typeof(Singleton36))]
[Singleton(typeof(ISingleton37), typeof(Singleton37))]
[Singleton(typeof(ISingleton38), typeof(Singleton38))]
[Singleton(typeof(ISingleton39), typeof(Singleton39))]
[Singleton(typeof(ISingleton40), typeof(Singleton40))]
[Singleton(typeof(ISingleton41), typeof(Singleton41))]
[Singleton(typeof(ISingleton42), typeof(Singleton42))]
[Singleton(typeof(ISingleton43), typeof(Singleton43))]
[Singleton(typeof(ISingleton44), typeof(Singleton44))]
[Singleton(typeof(ISingleton45), typeof(Singleton45))]
[Singleton(typeof(ISingleton46), typeof(Singleton46))]
[Singleton(typeof(ISingleton47), typeof(Singleton47))]
[Singleton(typeof(ISingleton48), typeof(Singleton48))]
[Singleton(typeof(ISingleton49), typeof(Singleton49))]
[Singleton(typeof(ISingleton50), typeof(Singleton50))]
[Singleton(typeof(ISingleton51), typeof(Singleton51))]
[Singleton(typeof(ISingleton52), typeof(Singleton52))]
[Singleton(typeof(ISingleton53), typeof(Singleton53))]
[Singleton(typeof(ISingleton54), typeof(Singleton54))]
[Singleton(typeof(ISingleton55), typeof(Singleton55))]
[Singleton(typeof(ISingleton56), typeof(Singleton56))]
[Singleton(typeof(ISingleton57), typeof(Singleton57))]
[Singleton(typeof(ISingleton58), typeof(Singleton58))]
[Singleton(typeof(ISingleton59), typeof(Singleton59))]
[Singleton(typeof(ISingleton60), typeof(Singleton60))]
[Singleton(typeof(ISingleton61), typeof(Singleton61))]
[Singleton(typeof(ISingleton62), typeof(Singleton62))]
[Singleton(typeof(ISingleton63), typeof(Singleton63))]
[Singleton(typeof(ISingleton64), typeof(Singleton64))]
[Singleton(typeof(ISingleton65), typeof(Singleton65))]
[Singleton(typeof(ISingleton66), typeof(Singleton66))]
[Singleton(typeof(ISingleton67), typeof(Singleton67))]
[Singleton(typeof(ISingleton68), typeof(Singleton68))]
[Singleton(typeof(ISingleton69), typeof(Singleton69))]
[Singleton(typeof(ISingleton70), typeof(Singleton70))]
[Singleton(typeof(ISingleton71), typeof(Singleton71))]
[Singleton(typeof(ISingleton72), typeof(Singleton72))]
[Singleton(typeof(ISingleton73), typeof(Singleton73))]
[Singleton(typeof(ISingleton74), typeof(Singleton74))]
[Singleton(typeof(ISingleton75), typeof(Singleton75))]
[Singleton(typeof(ISingleton76), typeof(Singleton76))]
[Singleton(typeof(ISingleton77), typeof(Singleton77))]
[Singleton(typeof(ISingleton78), typeof(Singleton78))]
[Singleton(typeof(ISingleton79), typeof(Singleton79))]
[Singleton(typeof(ISingleton80), typeof(Singleton80))]
[Singleton(typeof(ISingleton81), typeof(Singleton81))]
[Singleton(typeof(ISingleton82), typeof(Singleton82))]
[Singleton(typeof(ISingleton83), typeof(Singleton83))]
[Singleton(typeof(ISingleton84), typeof(Singleton84))]
[Singleton(typeof(ISingleton85), typeof(Singleton85))]
[Singleton(typeof(ISingleton86), typeof(Singleton86))]
[Singleton(typeof(ISingleton87), typeof(Singleton87))]
[Singleton(typeof(ISingleton88), typeof(Singleton88))]
[Singleton(typeof(ISingleton89), typeof(Singleton89))]
[Singleton(typeof(ISingleton90), typeof(Singleton90))]
[Singleton(typeof(ISingleton91), typeof(Singleton91))]
[Singleton(typeof(ISingleton92), typeof(Singleton92))]
[Singleton(typeof(ISingleton93), typeof(Singleton93))]
[Singleton(typeof(ISingleton94), typeof(Singleton94))]
[Singleton(typeof(ISingleton95), typeof(Singleton95))]
[Singleton(typeof(ISingleton96), typeof(Singleton96))]
[Singleton(typeof(ISingleton97), typeof(Singleton97))]
[Singleton(typeof(ISingleton98), typeof(Singleton98))]
[Singleton(typeof(ISingleton99), typeof(Singleton99))]
[Singleton(typeof(ISingleton100), typeof(Singleton100))]
internal partial class DeepContainerSingleton
{
}