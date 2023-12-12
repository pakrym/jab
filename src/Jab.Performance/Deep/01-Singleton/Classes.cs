namespace Jab.Performance.Deep.Singleton;

public interface ISingleton1
{
    void SayHi();
}
public class Singleton1(ISingleton2 _singleton2) : ISingleton1
{
    public void SayHi()
    {
        _singleton2.SayHi();
        Console.WriteLine("Hello from Singleton 1");
    }
}

public interface ISingleton2
{
    void SayHi();
}
public class Singleton2(ISingleton3 _singleton3) : ISingleton2
{
    public void SayHi()
    {
        _singleton3.SayHi();
        Console.WriteLine("Hello from Singleton 2");
    }
}

public interface ISingleton3
{
    void SayHi();
}
public class Singleton3(ISingleton4 _singleton4) : ISingleton3
{
    public void SayHi()
    {
        _singleton4.SayHi();
        Console.WriteLine("Hello from Singleton 3");
    }
}

public interface ISingleton4
{
    void SayHi();
}
public class Singleton4(ISingleton5 _singleton5) : ISingleton4
{
    public void SayHi()
    {
        _singleton5.SayHi();
        Console.WriteLine("Hello from Singleton 4");
    }
}

public interface ISingleton5
{
    void SayHi();
}
public class Singleton5(ISingleton6 _singleton6) : ISingleton5
{
    public void SayHi()
    {
        _singleton6.SayHi();
        Console.WriteLine("Hello from Singleton 5");
    }
}

public interface ISingleton6
{
    void SayHi();
}
public class Singleton6(ISingleton7 _singleton7) : ISingleton6
{
    public void SayHi()
    {
        _singleton7.SayHi();
        Console.WriteLine("Hello from Singleton 6");
    }
}

public interface ISingleton7
{
    void SayHi();
}
public class Singleton7(ISingleton8 _singleton8) : ISingleton7
{
    public void SayHi()
    {
        _singleton8.SayHi();
        Console.WriteLine("Hello from Singleton 7");
    }
}

public interface ISingleton8
{
    void SayHi();
}
public class Singleton8(ISingleton9 _singleton9) : ISingleton8
{
    public void SayHi()
    {
        _singleton9.SayHi();
        Console.WriteLine("Hello from Singleton 8");
    }
}

public interface ISingleton9
{
    void SayHi();
}
public class Singleton9(ISingleton10 _singleton10) : ISingleton9
{
    public void SayHi()
    {
        _singleton10.SayHi();
        Console.WriteLine("Hello from Singleton 9");
    }
}

public interface ISingleton10
{
    void SayHi();
}
public class Singleton10(ISingleton11 _singleton11) : ISingleton10
{
    public void SayHi()
    {
        _singleton11.SayHi();
        Console.WriteLine("Hello from Singleton 10");
    }
}

public interface ISingleton11
{
    void SayHi();
}
public class Singleton11(ISingleton12 _singleton12) : ISingleton11
{
    public void SayHi()
    {
        _singleton12.SayHi();
        Console.WriteLine("Hello from Singleton 11");
    }
}

public interface ISingleton12
{
    void SayHi();
}
public class Singleton12(ISingleton13 _singleton13) : ISingleton12
{
    public void SayHi()
    {
        _singleton13.SayHi();
        Console.WriteLine("Hello from Singleton 12");
    }
}

public interface ISingleton13
{
    void SayHi();
}
public class Singleton13(ISingleton14 _singleton14) : ISingleton13
{
    public void SayHi()
    {
        _singleton14.SayHi();
        Console.WriteLine("Hello from Singleton 13");
    }
}

public interface ISingleton14
{
    void SayHi();
}
public class Singleton14(ISingleton15 _singleton15) : ISingleton14
{
    public void SayHi()
    {
        _singleton15.SayHi();
        Console.WriteLine("Hello from Singleton 14");
    }
}

public interface ISingleton15
{
    void SayHi();
}
public class Singleton15(ISingleton16 _singleton16) : ISingleton15
{
    public void SayHi()
    {
        _singleton16.SayHi();
        Console.WriteLine("Hello from Singleton 15");
    }
}

public interface ISingleton16
{
    void SayHi();
}
public class Singleton16(ISingleton17 _singleton17) : ISingleton16
{
    public void SayHi()
    {
        _singleton17.SayHi();
        Console.WriteLine("Hello from Singleton 16");
    }
}

public interface ISingleton17
{
    void SayHi();
}
public class Singleton17(ISingleton18 _singleton18) : ISingleton17
{
    public void SayHi()
    {
        _singleton18.SayHi();
        Console.WriteLine("Hello from Singleton 17");
    }
}

public interface ISingleton18
{
    void SayHi();
}
public class Singleton18(ISingleton19 _singleton19) : ISingleton18
{
    public void SayHi()
    {
        _singleton19.SayHi();
        Console.WriteLine("Hello from Singleton 18");
    }
}

public interface ISingleton19
{
    void SayHi();
}
public class Singleton19(ISingleton20 _singleton20) : ISingleton19
{
    public void SayHi()
    {
        _singleton20.SayHi();
        Console.WriteLine("Hello from Singleton 19");
    }
}

public interface ISingleton20
{
    void SayHi();
}
public class Singleton20(ISingleton21 _singleton21) : ISingleton20
{
    public void SayHi()
    {
        _singleton21.SayHi();
        Console.WriteLine("Hello from Singleton 20");
    }
}

public interface ISingleton21
{
    void SayHi();
}
public class Singleton21(ISingleton22 _singleton22) : ISingleton21
{
    public void SayHi()
    {
        _singleton22.SayHi();
        Console.WriteLine("Hello from Singleton 21");
    }
}

public interface ISingleton22
{
    void SayHi();
}
public class Singleton22(ISingleton23 _singleton23) : ISingleton22
{
    public void SayHi()
    {
        _singleton23.SayHi();
        Console.WriteLine("Hello from Singleton 22");
    }
}

public interface ISingleton23
{
    void SayHi();
}
public class Singleton23(ISingleton24 _singleton24) : ISingleton23
{
    public void SayHi()
    {
        _singleton24.SayHi();
        Console.WriteLine("Hello from Singleton 23");
    }
}

public interface ISingleton24
{
    void SayHi();
}
public class Singleton24(ISingleton25 _singleton25) : ISingleton24
{
    public void SayHi()
    {
        _singleton25.SayHi();
        Console.WriteLine("Hello from Singleton 24");
    }
}

public interface ISingleton25
{
    void SayHi();
}
public class Singleton25(ISingleton26 _singleton26) : ISingleton25
{
    public void SayHi()
    {
        _singleton26.SayHi();
        Console.WriteLine("Hello from Singleton 25");
    }
}

public interface ISingleton26
{
    void SayHi();
}
public class Singleton26(ISingleton27 _singleton27) : ISingleton26
{
    public void SayHi()
    {
        _singleton27.SayHi();
        Console.WriteLine("Hello from Singleton 26");
    }
}

public interface ISingleton27
{
    void SayHi();
}
public class Singleton27(ISingleton28 _singleton28) : ISingleton27
{
    public void SayHi()
    {
        _singleton28.SayHi();
        Console.WriteLine("Hello from Singleton 27");
    }
}

public interface ISingleton28
{
    void SayHi();
}
public class Singleton28(ISingleton29 _singleton29) : ISingleton28
{
    public void SayHi()
    {
        _singleton29.SayHi();
        Console.WriteLine("Hello from Singleton 28");
    }
}

public interface ISingleton29
{
    void SayHi();
}
public class Singleton29(ISingleton30 _singleton30) : ISingleton29
{
    public void SayHi()
    {
        _singleton30.SayHi();
        Console.WriteLine("Hello from Singleton 29");
    }
}

public interface ISingleton30
{
    void SayHi();
}
public class Singleton30(ISingleton31 _singleton31) : ISingleton30
{
    public void SayHi()
    {
        _singleton31.SayHi();
        Console.WriteLine("Hello from Singleton 30");
    }
}

public interface ISingleton31
{
    void SayHi();
}
public class Singleton31(ISingleton32 _singleton32) : ISingleton31
{
    public void SayHi()
    {
        _singleton32.SayHi();
        Console.WriteLine("Hello from Singleton 31");
    }
}

public interface ISingleton32
{
    void SayHi();
}
public class Singleton32(ISingleton33 _singleton33) : ISingleton32
{
    public void SayHi()
    {
        _singleton33.SayHi();
        Console.WriteLine("Hello from Singleton 32");
    }
}

public interface ISingleton33
{
    void SayHi();
}
public class Singleton33(ISingleton34 _singleton34) : ISingleton33
{
    public void SayHi()
    {
        _singleton34.SayHi();
        Console.WriteLine("Hello from Singleton 33");
    }
}

public interface ISingleton34
{
    void SayHi();
}
public class Singleton34(ISingleton35 _singleton35) : ISingleton34
{
    public void SayHi()
    {
        _singleton35.SayHi();
        Console.WriteLine("Hello from Singleton 34");
    }
}

public interface ISingleton35
{
    void SayHi();
}
public class Singleton35(ISingleton36 _singleton36) : ISingleton35
{
    public void SayHi()
    {
        _singleton36.SayHi();
        Console.WriteLine("Hello from Singleton 35");
    }
}

public interface ISingleton36
{
    void SayHi();
}
public class Singleton36(ISingleton37 _singleton37) : ISingleton36
{
    public void SayHi()
    {
        _singleton37.SayHi();
        Console.WriteLine("Hello from Singleton 36");
    }
}

public interface ISingleton37
{
    void SayHi();
}
public class Singleton37(ISingleton38 _singleton38) : ISingleton37
{
    public void SayHi()
    {
        _singleton38.SayHi();
        Console.WriteLine("Hello from Singleton 37");
    }
}

public interface ISingleton38
{
    void SayHi();
}
public class Singleton38(ISingleton39 _singleton39) : ISingleton38
{
    public void SayHi()
    {
        _singleton39.SayHi();
        Console.WriteLine("Hello from Singleton 38");
    }
}

public interface ISingleton39
{
    void SayHi();
}
public class Singleton39(ISingleton40 _singleton40) : ISingleton39
{
    public void SayHi()
    {
        _singleton40.SayHi();
        Console.WriteLine("Hello from Singleton 39");
    }
}

public interface ISingleton40
{
    void SayHi();
}
public class Singleton40(ISingleton41 _singleton41) : ISingleton40
{
    public void SayHi()
    {
        _singleton41.SayHi();
        Console.WriteLine("Hello from Singleton 40");
    }
}

public interface ISingleton41
{
    void SayHi();
}
public class Singleton41(ISingleton42 _singleton42) : ISingleton41
{
    public void SayHi()
    {
        _singleton42.SayHi();
        Console.WriteLine("Hello from Singleton 41");
    }
}

public interface ISingleton42
{
    void SayHi();
}
public class Singleton42(ISingleton43 _singleton43) : ISingleton42
{
    public void SayHi()
    {
        _singleton43.SayHi();
        Console.WriteLine("Hello from Singleton 42");
    }
}

public interface ISingleton43
{
    void SayHi();
}
public class Singleton43(ISingleton44 _singleton44) : ISingleton43
{
    public void SayHi()
    {
        _singleton44.SayHi();
        Console.WriteLine("Hello from Singleton 43");
    }
}

public interface ISingleton44
{
    void SayHi();
}
public class Singleton44(ISingleton45 _singleton45) : ISingleton44
{
    public void SayHi()
    {
        _singleton45.SayHi();
        Console.WriteLine("Hello from Singleton 44");
    }
}

public interface ISingleton45
{
    void SayHi();
}
public class Singleton45(ISingleton46 _singleton46) : ISingleton45
{
    public void SayHi()
    {
        _singleton46.SayHi();
        Console.WriteLine("Hello from Singleton 45");
    }
}

public interface ISingleton46
{
    void SayHi();
}
public class Singleton46(ISingleton47 _singleton47) : ISingleton46
{
    public void SayHi()
    {
        _singleton47.SayHi();
        Console.WriteLine("Hello from Singleton 46");
    }
}

public interface ISingleton47
{
    void SayHi();
}
public class Singleton47(ISingleton48 _singleton48) : ISingleton47
{
    public void SayHi()
    {
        _singleton48.SayHi();
        Console.WriteLine("Hello from Singleton 47");
    }
}

public interface ISingleton48
{
    void SayHi();
}
public class Singleton48(ISingleton49 _singleton49) : ISingleton48
{
    public void SayHi()
    {
        _singleton49.SayHi();
        Console.WriteLine("Hello from Singleton 48");
    }
}

public interface ISingleton49
{
    void SayHi();
}
public class Singleton49(ISingleton50 _singleton50) : ISingleton49
{
    public void SayHi()
    {
        _singleton50.SayHi();
        Console.WriteLine("Hello from Singleton 49");
    }
}

public interface ISingleton50
{
    void SayHi();
}
public class Singleton50(ISingleton51 _singleton51) : ISingleton50
{
    public void SayHi()
    {
        _singleton51.SayHi();
        Console.WriteLine("Hello from Singleton 50");
    }
}

public interface ISingleton51
{
    void SayHi();
}
public class Singleton51(ISingleton52 _singleton52) : ISingleton51
{
    public void SayHi()
    {
        _singleton52.SayHi();
        Console.WriteLine("Hello from Singleton 51");
    }
}

public interface ISingleton52
{
    void SayHi();
}
public class Singleton52(ISingleton53 _singleton53) : ISingleton52
{
    public void SayHi()
    {
        _singleton53.SayHi();
        Console.WriteLine("Hello from Singleton 52");
    }
}

public interface ISingleton53
{
    void SayHi();
}
public class Singleton53(ISingleton54 _singleton54) : ISingleton53
{
    public void SayHi()
    {
        _singleton54.SayHi();
        Console.WriteLine("Hello from Singleton 53");
    }
}

public interface ISingleton54
{
    void SayHi();
}
public class Singleton54(ISingleton55 _singleton55) : ISingleton54
{
    public void SayHi()
    {
        _singleton55.SayHi();
        Console.WriteLine("Hello from Singleton 54");
    }
}

public interface ISingleton55
{
    void SayHi();
}
public class Singleton55(ISingleton56 _singleton56) : ISingleton55
{
    public void SayHi()
    {
        _singleton56.SayHi();
        Console.WriteLine("Hello from Singleton 55");
    }
}

public interface ISingleton56
{
    void SayHi();
}
public class Singleton56(ISingleton57 _singleton57) : ISingleton56
{
    public void SayHi()
    {
        _singleton57.SayHi();
        Console.WriteLine("Hello from Singleton 56");
    }
}

public interface ISingleton57
{
    void SayHi();
}
public class Singleton57(ISingleton58 _singleton58) : ISingleton57
{
    public void SayHi()
    {
        _singleton58.SayHi();
        Console.WriteLine("Hello from Singleton 57");
    }
}

public interface ISingleton58
{
    void SayHi();
}
public class Singleton58(ISingleton59 _singleton59) : ISingleton58
{
    public void SayHi()
    {
        _singleton59.SayHi();
        Console.WriteLine("Hello from Singleton 58");
    }
}

public interface ISingleton59
{
    void SayHi();
}
public class Singleton59(ISingleton60 _singleton60) : ISingleton59
{
    public void SayHi()
    {
        _singleton60.SayHi();
        Console.WriteLine("Hello from Singleton 59");
    }
}

public interface ISingleton60
{
    void SayHi();
}
public class Singleton60(ISingleton61 _singleton61) : ISingleton60
{
    public void SayHi()
    {
        _singleton61.SayHi();
        Console.WriteLine("Hello from Singleton 60");
    }
}

public interface ISingleton61
{
    void SayHi();
}
public class Singleton61(ISingleton62 _singleton62) : ISingleton61
{
    public void SayHi()
    {
        _singleton62.SayHi();
        Console.WriteLine("Hello from Singleton 61");
    }
}

public interface ISingleton62
{
    void SayHi();
}
public class Singleton62(ISingleton63 _singleton63) : ISingleton62
{
    public void SayHi()
    {
        _singleton63.SayHi();
        Console.WriteLine("Hello from Singleton 62");
    }
}

public interface ISingleton63
{
    void SayHi();
}
public class Singleton63(ISingleton64 _singleton64) : ISingleton63
{
    public void SayHi()
    {
        _singleton64.SayHi();
        Console.WriteLine("Hello from Singleton 63");
    }
}

public interface ISingleton64
{
    void SayHi();
}
public class Singleton64(ISingleton65 _singleton65) : ISingleton64
{
    public void SayHi()
    {
        _singleton65.SayHi();
        Console.WriteLine("Hello from Singleton 64");
    }
}

public interface ISingleton65
{
    void SayHi();
}
public class Singleton65(ISingleton66 _singleton66) : ISingleton65
{
    public void SayHi()
    {
        _singleton66.SayHi();
        Console.WriteLine("Hello from Singleton 65");
    }
}

public interface ISingleton66
{
    void SayHi();
}
public class Singleton66(ISingleton67 _singleton67) : ISingleton66
{
    public void SayHi()
    {
        _singleton67.SayHi();
        Console.WriteLine("Hello from Singleton 66");
    }
}

public interface ISingleton67
{
    void SayHi();
}
public class Singleton67(ISingleton68 _singleton68) : ISingleton67
{
    public void SayHi()
    {
        _singleton68.SayHi();
        Console.WriteLine("Hello from Singleton 67");
    }
}

public interface ISingleton68
{
    void SayHi();
}
public class Singleton68(ISingleton69 _singleton69) : ISingleton68
{
    public void SayHi()
    {
        _singleton69.SayHi();
        Console.WriteLine("Hello from Singleton 68");
    }
}

public interface ISingleton69
{
    void SayHi();
}
public class Singleton69(ISingleton70 _singleton70) : ISingleton69
{
    public void SayHi()
    {
        _singleton70.SayHi();
        Console.WriteLine("Hello from Singleton 69");
    }
}

public interface ISingleton70
{
    void SayHi();
}
public class Singleton70(ISingleton71 _singleton71) : ISingleton70
{
    public void SayHi()
    {
        _singleton71.SayHi();
        Console.WriteLine("Hello from Singleton 70");
    }
}

public interface ISingleton71
{
    void SayHi();
}
public class Singleton71(ISingleton72 _singleton72) : ISingleton71
{
    public void SayHi()
    {
        _singleton72.SayHi();
        Console.WriteLine("Hello from Singleton 71");
    }
}

public interface ISingleton72
{
    void SayHi();
}
public class Singleton72(ISingleton73 _singleton73) : ISingleton72
{
    public void SayHi()
    {
        _singleton73.SayHi();
        Console.WriteLine("Hello from Singleton 72");
    }
}

public interface ISingleton73
{
    void SayHi();
}
public class Singleton73(ISingleton74 _singleton74) : ISingleton73
{
    public void SayHi()
    {
        _singleton74.SayHi();
        Console.WriteLine("Hello from Singleton 73");
    }
}

public interface ISingleton74
{
    void SayHi();
}
public class Singleton74(ISingleton75 _singleton75) : ISingleton74
{
    public void SayHi()
    {
        _singleton75.SayHi();
        Console.WriteLine("Hello from Singleton 74");
    }
}

public interface ISingleton75
{
    void SayHi();
}
public class Singleton75(ISingleton76 _singleton76) : ISingleton75
{
    public void SayHi()
    {
        _singleton76.SayHi();
        Console.WriteLine("Hello from Singleton 75");
    }
}

public interface ISingleton76
{
    void SayHi();
}
public class Singleton76(ISingleton77 _singleton77) : ISingleton76
{
    public void SayHi()
    {
        _singleton77.SayHi();
        Console.WriteLine("Hello from Singleton 76");
    }
}

public interface ISingleton77
{
    void SayHi();
}
public class Singleton77(ISingleton78 _singleton78) : ISingleton77
{
    public void SayHi()
    {
        _singleton78.SayHi();
        Console.WriteLine("Hello from Singleton 77");
    }
}

public interface ISingleton78
{
    void SayHi();
}
public class Singleton78(ISingleton79 _singleton79) : ISingleton78
{
    public void SayHi()
    {
        _singleton79.SayHi();
        Console.WriteLine("Hello from Singleton 78");
    }
}

public interface ISingleton79
{
    void SayHi();
}
public class Singleton79(ISingleton80 _singleton80) : ISingleton79
{
    public void SayHi()
    {
        _singleton80.SayHi();
        Console.WriteLine("Hello from Singleton 79");
    }
}

public interface ISingleton80
{
    void SayHi();
}
public class Singleton80(ISingleton81 _singleton81) : ISingleton80
{
    public void SayHi()
    {
        _singleton81.SayHi();
        Console.WriteLine("Hello from Singleton 80");
    }
}

public interface ISingleton81
{
    void SayHi();
}
public class Singleton81(ISingleton82 _singleton82) : ISingleton81
{
    public void SayHi()
    {
        _singleton82.SayHi();
        Console.WriteLine("Hello from Singleton 81");
    }
}

public interface ISingleton82
{
    void SayHi();
}
public class Singleton82(ISingleton83 _singleton83) : ISingleton82
{
    public void SayHi()
    {
        _singleton83.SayHi();
        Console.WriteLine("Hello from Singleton 82");
    }
}

public interface ISingleton83
{
    void SayHi();
}
public class Singleton83(ISingleton84 _singleton84) : ISingleton83
{
    public void SayHi()
    {
        _singleton84.SayHi();
        Console.WriteLine("Hello from Singleton 83");
    }
}

public interface ISingleton84
{
    void SayHi();
}
public class Singleton84(ISingleton85 _singleton85) : ISingleton84
{
    public void SayHi()
    {
        _singleton85.SayHi();
        Console.WriteLine("Hello from Singleton 84");
    }
}

public interface ISingleton85
{
    void SayHi();
}
public class Singleton85(ISingleton86 _singleton86) : ISingleton85
{
    public void SayHi()
    {
        _singleton86.SayHi();
        Console.WriteLine("Hello from Singleton 85");
    }
}

public interface ISingleton86
{
    void SayHi();
}
public class Singleton86(ISingleton87 _singleton87) : ISingleton86
{
    public void SayHi()
    {
        _singleton87.SayHi();
        Console.WriteLine("Hello from Singleton 86");
    }
}

public interface ISingleton87
{
    void SayHi();
}
public class Singleton87(ISingleton88 _singleton88) : ISingleton87
{
    public void SayHi()
    {
        _singleton88.SayHi();
        Console.WriteLine("Hello from Singleton 87");
    }
}

public interface ISingleton88
{
    void SayHi();
}
public class Singleton88(ISingleton89 _singleton89) : ISingleton88
{
    public void SayHi()
    {
        _singleton89.SayHi();
        Console.WriteLine("Hello from Singleton 88");
    }
}

public interface ISingleton89
{
    void SayHi();
}
public class Singleton89(ISingleton90 _singleton90) : ISingleton89
{
    public void SayHi()
    {
        _singleton90.SayHi();
        Console.WriteLine("Hello from Singleton 89");
    }
}

public interface ISingleton90
{
    void SayHi();
}
public class Singleton90(ISingleton91 _singleton91) : ISingleton90
{
    public void SayHi()
    {
        _singleton91.SayHi();
        Console.WriteLine("Hello from Singleton 90");
    }
}

public interface ISingleton91
{
    void SayHi();
}
public class Singleton91(ISingleton92 _singleton92) : ISingleton91
{
    public void SayHi()
    {
        _singleton92.SayHi();
        Console.WriteLine("Hello from Singleton 91");
    }
}

public interface ISingleton92
{
    void SayHi();
}
public class Singleton92(ISingleton93 _singleton93) : ISingleton92
{
    public void SayHi()
    {
        _singleton93.SayHi();
        Console.WriteLine("Hello from Singleton 92");
    }
}

public interface ISingleton93
{
    void SayHi();
}
public class Singleton93(ISingleton94 _singleton94) : ISingleton93
{
    public void SayHi()
    {
        _singleton94.SayHi();
        Console.WriteLine("Hello from Singleton 93");
    }
}

public interface ISingleton94
{
    void SayHi();
}
public class Singleton94(ISingleton95 _singleton95) : ISingleton94
{
    public void SayHi()
    {
        _singleton95.SayHi();
        Console.WriteLine("Hello from Singleton 94");
    }
}

public interface ISingleton95
{
    void SayHi();
}
public class Singleton95(ISingleton96 _singleton96) : ISingleton95
{
    public void SayHi()
    {
        _singleton96.SayHi();
        Console.WriteLine("Hello from Singleton 95");
    }
}

public interface ISingleton96
{
    void SayHi();
}
public class Singleton96(ISingleton97 _singleton97) : ISingleton96
{
    public void SayHi()
    {
        _singleton97.SayHi();
        Console.WriteLine("Hello from Singleton 96");
    }
}

public interface ISingleton97
{
    void SayHi();
}
public class Singleton97(ISingleton98 _singleton98) : ISingleton97
{
    public void SayHi()
    {
        _singleton98.SayHi();
        Console.WriteLine("Hello from Singleton 97");
    }
}

public interface ISingleton98
{
    void SayHi();
}
public class Singleton98(ISingleton99 _singleton99) : ISingleton98
{
    public void SayHi()
    {
        _singleton99.SayHi();
        Console.WriteLine("Hello from Singleton 98");
    }
}

public interface ISingleton99
{
    void SayHi();
}
public class Singleton99(ISingleton100 _singleton100) : ISingleton99
{
    public void SayHi()
    {
        _singleton100.SayHi();
        Console.WriteLine("Hello from Singleton 99");
    }
}

public interface ISingleton100
{
    void SayHi();
}
public class Singleton100() : ISingleton100
{
    public void SayHi()
    {
        Console.WriteLine("Hello from Singleton 100");
    }
}

