namespace Jab.Performance.Deep.Mixed;

using Jab.Performance.Deep.Singleton;
using Jab.Performance.Deep.Transient;

public interface IMix1
{
    void SayHi();
}

public class Mix1(ISingleton1 singleton1, ITransient1 transient1) : IMix1
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 1");
        singleton1.SayHi();
        transient1.SayHi();
    }
}

public interface IMix2
{
    void SayHi();
}

public class Mix2(ISingleton2 singleton2, ITransient2 transient2) : IMix2
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 2");
        singleton2.SayHi();
        transient2.SayHi();
    }
}

public interface IMix3
{
    void SayHi();
}

public class Mix3(ISingleton3 singleton3, ITransient3 transient3) : IMix3
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 3");
        singleton3.SayHi();
        transient3.SayHi();
    }
}

public interface IMix4
{
    void SayHi();
}

public class Mix4(ISingleton4 singleton4, ITransient4 transient4) : IMix4
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 4");
        singleton4.SayHi();
        transient4.SayHi();
    }
}

public interface IMix5
{
    void SayHi();
}

public class Mix5(ISingleton5 singleton5, ITransient5 transient5) : IMix5
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 5");
        singleton5.SayHi();
        transient5.SayHi();
    }
}

public interface IMix6
{
    void SayHi();
}

public class Mix6(ISingleton6 singleton6, ITransient6 transient6) : IMix6
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 6");
        singleton6.SayHi();
        transient6.SayHi();
    }
}

public interface IMix7
{
    void SayHi();
}

public class Mix7(ISingleton7 singleton7, ITransient7 transient7) : IMix7
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 7");
        singleton7.SayHi();
        transient7.SayHi();
    }
}

public interface IMix8
{
    void SayHi();
}

public class Mix8(ISingleton8 singleton8, ITransient8 transient8) : IMix8
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 8");
        singleton8.SayHi();
        transient8.SayHi();
    }
}

public interface IMix9
{
    void SayHi();
}

public class Mix9(ISingleton9 singleton9, ITransient9 transient9) : IMix9
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 9");
        singleton9.SayHi();
        transient9.SayHi();
    }
}

public interface IMix10
{
    void SayHi();
}

public class Mix10(ISingleton10 singleton10, ITransient10 transient10) : IMix10
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 10");
        singleton10.SayHi();
        transient10.SayHi();
    }
}

public interface IMix11
{
    void SayHi();
}

public class Mix11(ISingleton11 singleton11, ITransient11 transient11) : IMix11
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 11");
        singleton11.SayHi();
        transient11.SayHi();
    }
}

public interface IMix12
{
    void SayHi();
}

public class Mix12(ISingleton12 singleton12, ITransient12 transient12) : IMix12
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 12");
        singleton12.SayHi();
        transient12.SayHi();
    }
}

public interface IMix13
{
    void SayHi();
}

public class Mix13(ISingleton13 singleton13, ITransient13 transient13) : IMix13
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 13");
        singleton13.SayHi();
        transient13.SayHi();
    }
}

public interface IMix14
{
    void SayHi();
}

public class Mix14(ISingleton14 singleton14, ITransient14 transient14) : IMix14
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 14");
        singleton14.SayHi();
        transient14.SayHi();
    }
}

public interface IMix15
{
    void SayHi();
}

public class Mix15(ISingleton15 singleton15, ITransient15 transient15) : IMix15
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 15");
        singleton15.SayHi();
        transient15.SayHi();
    }
}

public interface IMix16
{
    void SayHi();
}

public class Mix16(ISingleton16 singleton16, ITransient16 transient16) : IMix16
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 16");
        singleton16.SayHi();
        transient16.SayHi();
    }
}

public interface IMix17
{
    void SayHi();
}

public class Mix17(ISingleton17 singleton17, ITransient17 transient17) : IMix17
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 17");
        singleton17.SayHi();
        transient17.SayHi();
    }
}

public interface IMix18
{
    void SayHi();
}

public class Mix18(ISingleton18 singleton18, ITransient18 transient18) : IMix18
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 18");
        singleton18.SayHi();
        transient18.SayHi();
    }
}

public interface IMix19
{
    void SayHi();
}

public class Mix19(ISingleton19 singleton19, ITransient19 transient19) : IMix19
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 19");
        singleton19.SayHi();
        transient19.SayHi();
    }
}

public interface IMix20
{
    void SayHi();
}

public class Mix20(ISingleton20 singleton20, ITransient20 transient20) : IMix20
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 20");
        singleton20.SayHi();
        transient20.SayHi();
    }
}

public interface IMix21
{
    void SayHi();
}

public class Mix21(ISingleton21 singleton21, ITransient21 transient21) : IMix21
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 21");
        singleton21.SayHi();
        transient21.SayHi();
    }
}

public interface IMix22
{
    void SayHi();
}

public class Mix22(ISingleton22 singleton22, ITransient22 transient22) : IMix22
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 22");
        singleton22.SayHi();
        transient22.SayHi();
    }
}

public interface IMix23
{
    void SayHi();
}

public class Mix23(ISingleton23 singleton23, ITransient23 transient23) : IMix23
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 23");
        singleton23.SayHi();
        transient23.SayHi();
    }
}

public interface IMix24
{
    void SayHi();
}

public class Mix24(ISingleton24 singleton24, ITransient24 transient24) : IMix24
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 24");
        singleton24.SayHi();
        transient24.SayHi();
    }
}

public interface IMix25
{
    void SayHi();
}

public class Mix25(ISingleton25 singleton25, ITransient25 transient25) : IMix25
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 25");
        singleton25.SayHi();
        transient25.SayHi();
    }
}

public interface IMix26
{
    void SayHi();
}

public class Mix26(ISingleton26 singleton26, ITransient26 transient26) : IMix26
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 26");
        singleton26.SayHi();
        transient26.SayHi();
    }
}

public interface IMix27
{
    void SayHi();
}

public class Mix27(ISingleton27 singleton27, ITransient27 transient27) : IMix27
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 27");
        singleton27.SayHi();
        transient27.SayHi();
    }
}

public interface IMix28
{
    void SayHi();
}

public class Mix28(ISingleton28 singleton28, ITransient28 transient28) : IMix28
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 28");
        singleton28.SayHi();
        transient28.SayHi();
    }
}

public interface IMix29
{
    void SayHi();
}

public class Mix29(ISingleton29 singleton29, ITransient29 transient29) : IMix29
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 29");
        singleton29.SayHi();
        transient29.SayHi();
    }
}

public interface IMix30
{
    void SayHi();
}

public class Mix30(ISingleton30 singleton30, ITransient30 transient30) : IMix30
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 30");
        singleton30.SayHi();
        transient30.SayHi();
    }
}

public interface IMix31
{
    void SayHi();
}

public class Mix31(ISingleton31 singleton31, ITransient31 transient31) : IMix31
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 31");
        singleton31.SayHi();
        transient31.SayHi();
    }
}

public interface IMix32
{
    void SayHi();
}

public class Mix32(ISingleton32 singleton32, ITransient32 transient32) : IMix32
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 32");
        singleton32.SayHi();
        transient32.SayHi();
    }
}

public interface IMix33
{
    void SayHi();
}

public class Mix33(ISingleton33 singleton33, ITransient33 transient33) : IMix33
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 33");
        singleton33.SayHi();
        transient33.SayHi();
    }
}

public interface IMix34
{
    void SayHi();
}

public class Mix34(ISingleton34 singleton34, ITransient34 transient34) : IMix34
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 34");
        singleton34.SayHi();
        transient34.SayHi();
    }
}

public interface IMix35
{
    void SayHi();
}

public class Mix35(ISingleton35 singleton35, ITransient35 transient35) : IMix35
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 35");
        singleton35.SayHi();
        transient35.SayHi();
    }
}

public interface IMix36
{
    void SayHi();
}

public class Mix36(ISingleton36 singleton36, ITransient36 transient36) : IMix36
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 36");
        singleton36.SayHi();
        transient36.SayHi();
    }
}

public interface IMix37
{
    void SayHi();
}

public class Mix37(ISingleton37 singleton37, ITransient37 transient37) : IMix37
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 37");
        singleton37.SayHi();
        transient37.SayHi();
    }
}

public interface IMix38
{
    void SayHi();
}

public class Mix38(ISingleton38 singleton38, ITransient38 transient38) : IMix38
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 38");
        singleton38.SayHi();
        transient38.SayHi();
    }
}

public interface IMix39
{
    void SayHi();
}

public class Mix39(ISingleton39 singleton39, ITransient39 transient39) : IMix39
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 39");
        singleton39.SayHi();
        transient39.SayHi();
    }
}

public interface IMix40
{
    void SayHi();
}

public class Mix40(ISingleton40 singleton40, ITransient40 transient40) : IMix40
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 40");
        singleton40.SayHi();
        transient40.SayHi();
    }
}

public interface IMix41
{
    void SayHi();
}

public class Mix41(ISingleton41 singleton41, ITransient41 transient41) : IMix41
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 41");
        singleton41.SayHi();
        transient41.SayHi();
    }
}

public interface IMix42
{
    void SayHi();
}

public class Mix42(ISingleton42 singleton42, ITransient42 transient42) : IMix42
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 42");
        singleton42.SayHi();
        transient42.SayHi();
    }
}

public interface IMix43
{
    void SayHi();
}

public class Mix43(ISingleton43 singleton43, ITransient43 transient43) : IMix43
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 43");
        singleton43.SayHi();
        transient43.SayHi();
    }
}

public interface IMix44
{
    void SayHi();
}

public class Mix44(ISingleton44 singleton44, ITransient44 transient44) : IMix44
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 44");
        singleton44.SayHi();
        transient44.SayHi();
    }
}

public interface IMix45
{
    void SayHi();
}

public class Mix45(ISingleton45 singleton45, ITransient45 transient45) : IMix45
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 45");
        singleton45.SayHi();
        transient45.SayHi();
    }
}

public interface IMix46
{
    void SayHi();
}

public class Mix46(ISingleton46 singleton46, ITransient46 transient46) : IMix46
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 46");
        singleton46.SayHi();
        transient46.SayHi();
    }
}

public interface IMix47
{
    void SayHi();
}

public class Mix47(ISingleton47 singleton47, ITransient47 transient47) : IMix47
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 47");
        singleton47.SayHi();
        transient47.SayHi();
    }
}

public interface IMix48
{
    void SayHi();
}

public class Mix48(ISingleton48 singleton48, ITransient48 transient48) : IMix48
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 48");
        singleton48.SayHi();
        transient48.SayHi();
    }
}

public interface IMix49
{
    void SayHi();
}

public class Mix49(ISingleton49 singleton49, ITransient49 transient49) : IMix49
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 49");
        singleton49.SayHi();
        transient49.SayHi();
    }
}

public interface IMix50
{
    void SayHi();
}

public class Mix50(ISingleton50 singleton50, ITransient50 transient50) : IMix50
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 50");
        singleton50.SayHi();
        transient50.SayHi();
    }
}

public interface IMix51
{
    void SayHi();
}

public class Mix51(ISingleton51 singleton51, ITransient51 transient51) : IMix51
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 51");
        singleton51.SayHi();
        transient51.SayHi();
    }
}

public interface IMix52
{
    void SayHi();
}

public class Mix52(ISingleton52 singleton52, ITransient52 transient52) : IMix52
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 52");
        singleton52.SayHi();
        transient52.SayHi();
    }
}

public interface IMix53
{
    void SayHi();
}

public class Mix53(ISingleton53 singleton53, ITransient53 transient53) : IMix53
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 53");
        singleton53.SayHi();
        transient53.SayHi();
    }
}

public interface IMix54
{
    void SayHi();
}

public class Mix54(ISingleton54 singleton54, ITransient54 transient54) : IMix54
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 54");
        singleton54.SayHi();
        transient54.SayHi();
    }
}

public interface IMix55
{
    void SayHi();
}

public class Mix55(ISingleton55 singleton55, ITransient55 transient55) : IMix55
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 55");
        singleton55.SayHi();
        transient55.SayHi();
    }
}

public interface IMix56
{
    void SayHi();
}

public class Mix56(ISingleton56 singleton56, ITransient56 transient56) : IMix56
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 56");
        singleton56.SayHi();
        transient56.SayHi();
    }
}

public interface IMix57
{
    void SayHi();
}

public class Mix57(ISingleton57 singleton57, ITransient57 transient57) : IMix57
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 57");
        singleton57.SayHi();
        transient57.SayHi();
    }
}

public interface IMix58
{
    void SayHi();
}

public class Mix58(ISingleton58 singleton58, ITransient58 transient58) : IMix58
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 58");
        singleton58.SayHi();
        transient58.SayHi();
    }
}

public interface IMix59
{
    void SayHi();
}

public class Mix59(ISingleton59 singleton59, ITransient59 transient59) : IMix59
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 59");
        singleton59.SayHi();
        transient59.SayHi();
    }
}

public interface IMix60
{
    void SayHi();
}

public class Mix60(ISingleton60 singleton60, ITransient60 transient60) : IMix60
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 60");
        singleton60.SayHi();
        transient60.SayHi();
    }
}

public interface IMix61
{
    void SayHi();
}

public class Mix61(ISingleton61 singleton61, ITransient61 transient61) : IMix61
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 61");
        singleton61.SayHi();
        transient61.SayHi();
    }
}

public interface IMix62
{
    void SayHi();
}

public class Mix62(ISingleton62 singleton62, ITransient62 transient62) : IMix62
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 62");
        singleton62.SayHi();
        transient62.SayHi();
    }
}

public interface IMix63
{
    void SayHi();
}

public class Mix63(ISingleton63 singleton63, ITransient63 transient63) : IMix63
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 63");
        singleton63.SayHi();
        transient63.SayHi();
    }
}

public interface IMix64
{
    void SayHi();
}

public class Mix64(ISingleton64 singleton64, ITransient64 transient64) : IMix64
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 64");
        singleton64.SayHi();
        transient64.SayHi();
    }
}

public interface IMix65
{
    void SayHi();
}

public class Mix65(ISingleton65 singleton65, ITransient65 transient65) : IMix65
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 65");
        singleton65.SayHi();
        transient65.SayHi();
    }
}

public interface IMix66
{
    void SayHi();
}

public class Mix66(ISingleton66 singleton66, ITransient66 transient66) : IMix66
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 66");
        singleton66.SayHi();
        transient66.SayHi();
    }
}

public interface IMix67
{
    void SayHi();
}

public class Mix67(ISingleton67 singleton67, ITransient67 transient67) : IMix67
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 67");
        singleton67.SayHi();
        transient67.SayHi();
    }
}

public interface IMix68
{
    void SayHi();
}

public class Mix68(ISingleton68 singleton68, ITransient68 transient68) : IMix68
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 68");
        singleton68.SayHi();
        transient68.SayHi();
    }
}

public interface IMix69
{
    void SayHi();
}

public class Mix69(ISingleton69 singleton69, ITransient69 transient69) : IMix69
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 69");
        singleton69.SayHi();
        transient69.SayHi();
    }
}

public interface IMix70
{
    void SayHi();
}

public class Mix70(ISingleton70 singleton70, ITransient70 transient70) : IMix70
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 70");
        singleton70.SayHi();
        transient70.SayHi();
    }
}

public interface IMix71
{
    void SayHi();
}

public class Mix71(ISingleton71 singleton71, ITransient71 transient71) : IMix71
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 71");
        singleton71.SayHi();
        transient71.SayHi();
    }
}

public interface IMix72
{
    void SayHi();
}

public class Mix72(ISingleton72 singleton72, ITransient72 transient72) : IMix72
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 72");
        singleton72.SayHi();
        transient72.SayHi();
    }
}

public interface IMix73
{
    void SayHi();
}

public class Mix73(ISingleton73 singleton73, ITransient73 transient73) : IMix73
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 73");
        singleton73.SayHi();
        transient73.SayHi();
    }
}

public interface IMix74
{
    void SayHi();
}

public class Mix74(ISingleton74 singleton74, ITransient74 transient74) : IMix74
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 74");
        singleton74.SayHi();
        transient74.SayHi();
    }
}

public interface IMix75
{
    void SayHi();
}

public class Mix75(ISingleton75 singleton75, ITransient75 transient75) : IMix75
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 75");
        singleton75.SayHi();
        transient75.SayHi();
    }
}

public interface IMix76
{
    void SayHi();
}

public class Mix76(ISingleton76 singleton76, ITransient76 transient76) : IMix76
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 76");
        singleton76.SayHi();
        transient76.SayHi();
    }
}

public interface IMix77
{
    void SayHi();
}

public class Mix77(ISingleton77 singleton77, ITransient77 transient77) : IMix77
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 77");
        singleton77.SayHi();
        transient77.SayHi();
    }
}

public interface IMix78
{
    void SayHi();
}

public class Mix78(ISingleton78 singleton78, ITransient78 transient78) : IMix78
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 78");
        singleton78.SayHi();
        transient78.SayHi();
    }
}

public interface IMix79
{
    void SayHi();
}

public class Mix79(ISingleton79 singleton79, ITransient79 transient79) : IMix79
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 79");
        singleton79.SayHi();
        transient79.SayHi();
    }
}

public interface IMix80
{
    void SayHi();
}

public class Mix80(ISingleton80 singleton80, ITransient80 transient80) : IMix80
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 80");
        singleton80.SayHi();
        transient80.SayHi();
    }
}

public interface IMix81
{
    void SayHi();
}

public class Mix81(ISingleton81 singleton81, ITransient81 transient81) : IMix81
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 81");
        singleton81.SayHi();
        transient81.SayHi();
    }
}

public interface IMix82
{
    void SayHi();
}

public class Mix82(ISingleton82 singleton82, ITransient82 transient82) : IMix82
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 82");
        singleton82.SayHi();
        transient82.SayHi();
    }
}

public interface IMix83
{
    void SayHi();
}

public class Mix83(ISingleton83 singleton83, ITransient83 transient83) : IMix83
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 83");
        singleton83.SayHi();
        transient83.SayHi();
    }
}

public interface IMix84
{
    void SayHi();
}

public class Mix84(ISingleton84 singleton84, ITransient84 transient84) : IMix84
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 84");
        singleton84.SayHi();
        transient84.SayHi();
    }
}

public interface IMix85
{
    void SayHi();
}

public class Mix85(ISingleton85 singleton85, ITransient85 transient85) : IMix85
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 85");
        singleton85.SayHi();
        transient85.SayHi();
    }
}

public interface IMix86
{
    void SayHi();
}

public class Mix86(ISingleton86 singleton86, ITransient86 transient86) : IMix86
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 86");
        singleton86.SayHi();
        transient86.SayHi();
    }
}

public interface IMix87
{
    void SayHi();
}

public class Mix87(ISingleton87 singleton87, ITransient87 transient87) : IMix87
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 87");
        singleton87.SayHi();
        transient87.SayHi();
    }
}

public interface IMix88
{
    void SayHi();
}

public class Mix88(ISingleton88 singleton88, ITransient88 transient88) : IMix88
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 88");
        singleton88.SayHi();
        transient88.SayHi();
    }
}

public interface IMix89
{
    void SayHi();
}

public class Mix89(ISingleton89 singleton89, ITransient89 transient89) : IMix89
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 89");
        singleton89.SayHi();
        transient89.SayHi();
    }
}

public interface IMix90
{
    void SayHi();
}

public class Mix90(ISingleton90 singleton90, ITransient90 transient90) : IMix90
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 90");
        singleton90.SayHi();
        transient90.SayHi();
    }
}

public interface IMix91
{
    void SayHi();
}

public class Mix91(ISingleton91 singleton91, ITransient91 transient91) : IMix91
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 91");
        singleton91.SayHi();
        transient91.SayHi();
    }
}

public interface IMix92
{
    void SayHi();
}

public class Mix92(ISingleton92 singleton92, ITransient92 transient92) : IMix92
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 92");
        singleton92.SayHi();
        transient92.SayHi();
    }
}

public interface IMix93
{
    void SayHi();
}

public class Mix93(ISingleton93 singleton93, ITransient93 transient93) : IMix93
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 93");
        singleton93.SayHi();
        transient93.SayHi();
    }
}

public interface IMix94
{
    void SayHi();
}

public class Mix94(ISingleton94 singleton94, ITransient94 transient94) : IMix94
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 94");
        singleton94.SayHi();
        transient94.SayHi();
    }
}

public interface IMix95
{
    void SayHi();
}

public class Mix95(ISingleton95 singleton95, ITransient95 transient95) : IMix95
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 95");
        singleton95.SayHi();
        transient95.SayHi();
    }
}

public interface IMix96
{
    void SayHi();
}

public class Mix96(ISingleton96 singleton96, ITransient96 transient96) : IMix96
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 96");
        singleton96.SayHi();
        transient96.SayHi();
    }
}

public interface IMix97
{
    void SayHi();
}

public class Mix97(ISingleton97 singleton97, ITransient97 transient97) : IMix97
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 97");
        singleton97.SayHi();
        transient97.SayHi();
    }
}

public interface IMix98
{
    void SayHi();
}

public class Mix98(ISingleton98 singleton98, ITransient98 transient98) : IMix98
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 98");
        singleton98.SayHi();
        transient98.SayHi();
    }
}

public interface IMix99
{
    void SayHi();
}

public class Mix99(ISingleton99 singleton99, ITransient99 transient99) : IMix99
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 99");
        singleton99.SayHi();
        transient99.SayHi();
    }
}

public interface IMix100
{
    void SayHi();
}

public class Mix100(ISingleton100 singleton100, ITransient100 transient100) : IMix100
{
    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 100");
        singleton100.SayHi();
        transient100.SayHi();
    }
}

