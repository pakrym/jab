namespace Jab.Performance.Basic.Scoped;

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
    private readonly ISingleton1 _first;
    private readonly ITransient1 _second;
    public Mix1(ISingleton1 first, ITransient1 second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        this._first = first;
        this._second = second;
    }

    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 1");
        this._first.SayHi();
        this._second.SayHi();
    }
}

public class Mix2 : IMix2
{
    private readonly ISingleton2 _first;
    private readonly ITransient2 _second;
    public Mix2(ISingleton2 first, ITransient2 second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        this._first = first;
        this._second = second;
    }

    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 2");
        this._first.SayHi();
        this._second.SayHi();
    }
}


public class Mix3 : IMix3
{
    private readonly ISingleton3 _first;
    private readonly ITransient3 _second;
    public Mix3(ISingleton3 first, ITransient3 second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        this._first = first;
        this._second = second;
    }

    public void SayHi()
    {
        Console.WriteLine($"Hello from Combined 3");
        this._first.SayHi();
        this._second.SayHi();
    }
}

