namespace Jab.Performance.Basic.Mixed;

using Jab.Performance.Basic.Singleton;
using Jab.Performance.Basic.Transient;

public interface IMix1
{
    void SayHi();
}
public interface IMix2
{
    void SayHi();
}
public interface IMix3
{
    void SayHi();
}

public class Mix1 : IMix1
{
    private readonly ISingleton1 _singleton1;
    private readonly ITransient1 _transient1;
    public Mix1(ISingleton1 singleton1, ITransient1 transient1)
    {
        ArgumentNullException.ThrowIfNull(singleton1);
        ArgumentNullException.ThrowIfNull(transient1);
        this._singleton1 = singleton1;
        this._transient1 = transient1;
    }

    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 1");
        this._singleton1.SayHi();
        this._transient1.SayHi();
    }
}
public class Mix2 : IMix2
{
    private readonly ISingleton2 _singleton2;
    private readonly ITransient2 _transient2;
    public Mix2(ISingleton2 singleton2, ITransient2 transient2)
    {
        ArgumentNullException.ThrowIfNull(singleton2);
        ArgumentNullException.ThrowIfNull(transient2);
        this._singleton2 = singleton2;
        this._transient2 = transient2;
    }

    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 2");
        this._singleton2.SayHi();
        this._transient2.SayHi();
    }
}

public class Mix3 : IMix3
{
    private readonly ISingleton3 _singleton3;
    private readonly ITransient3 _transient3;
    public Mix3(ISingleton3 singleton3, ITransient3 transient3)
    {
        ArgumentNullException.ThrowIfNull(singleton3);
        ArgumentNullException.ThrowIfNull(transient3);
        this._singleton3 = singleton3;
        this._transient3 = transient3;
    }

    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 3");
        this._singleton3.SayHi();
        this._transient3.SayHi();
    }
}

