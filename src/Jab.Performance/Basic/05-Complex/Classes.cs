namespace Jab.Performance.Basic.Complex;

using Jab.Performance.Basic.Mixed;
using Jab.Performance.Basic.Singleton;
using Jab.Performance.Basic.Transient;

public interface IComplex1
{
    void SayHi();
}
public interface IComplex2
{
    void SayHi();
}
public interface IComplex3
{
    void SayHi();
}

public interface IService1
{
    void SayHi();
}
public interface IService2
{
    void SayHi();
}
public interface IService3
{
    void SayHi();
}

public class Service1(ITransient1 Transient1) : IService1
{
    public void SayHi()
    {
        Transient1.SayHi();
        Console.WriteLine("Hello from Service 1");
    }
}
public class Service2(ITransient2 Transient2) : IService2
{
    public void SayHi()
    {
        Transient2.SayHi();
        Console.WriteLine("Hello from Service 2");
    }
}
public class Service3(ITransient3 Transient3) : IService3
{
    public void SayHi()
    {
        Transient3.SayHi();
        Console.WriteLine("Hello from Service 3");
    }
}

public class Complex1 : IComplex1
{
    private readonly IService1 _service1;
    private readonly IService2 _service2;
    private readonly IService3 _service3;
    private readonly IMix1 _mix1;
    private readonly IMix2 _mix2;
    private readonly IMix3 _mix3;
    private readonly ISingleton1 _singleton1;
    private readonly ITransient1 _transient1;

    public Complex1(IService1 service1, IService2 service2, IService3 service3, IMix1 mix1, IMix2 mix2, IMix3 mix3, ISingleton1 singleton1, ITransient1 transient1)
    {
        ArgumentNullException.ThrowIfNull(service1);
        ArgumentNullException.ThrowIfNull(service2);
        ArgumentNullException.ThrowIfNull(service3);
        ArgumentNullException.ThrowIfNull(mix1);
        ArgumentNullException.ThrowIfNull(mix2);
        ArgumentNullException.ThrowIfNull(mix3);
        ArgumentNullException.ThrowIfNull(singleton1);
        ArgumentNullException.ThrowIfNull(transient1);

        _service1 = service1;
        _service2 = service2;
        _service3 = service3;
        _mix1 = mix1;
        _mix2 = mix2;
        _mix3 = mix3;
        _singleton1 = singleton1;
        _transient1 = transient1;
    }

    public void SayHi()
    {
        Console.WriteLine($"Hello from Complex 1");
        this._service1.SayHi();
        this._service2.SayHi();
        this._service3.SayHi();
        this._mix1.SayHi();
        this._mix2.SayHi();
        this._mix3.SayHi();
        this._singleton1.SayHi();
        this._transient1.SayHi();
    }
}

public class Complex2 : IComplex2
{
    private readonly IService1 _service1;
    private readonly IService2 _service2;
    private readonly IService3 _service3;
    private readonly IMix1 _mix1;
    private readonly IMix2 _mix2;
    private readonly IMix3 _mix3;
    private readonly ISingleton2 _singleton2;
    private readonly ITransient2 _transient2;

    public Complex2(IService1 service1, IService2 service2, IService3 service3, IMix1 mix1, IMix2 mix2, IMix3 mix3, ISingleton2 singleton2, ITransient2 transient2)
    {
        ArgumentNullException.ThrowIfNull(service1);
        ArgumentNullException.ThrowIfNull(service2);
        ArgumentNullException.ThrowIfNull(service3);
        ArgumentNullException.ThrowIfNull(mix1);
        ArgumentNullException.ThrowIfNull(mix2);
        ArgumentNullException.ThrowIfNull(mix3);
        ArgumentNullException.ThrowIfNull(singleton2);
        ArgumentNullException.ThrowIfNull(transient2);

        _service1 = service1;
        _service2 = service2;
        _service3 = service3;
        _mix1 = mix1;
        _mix2 = mix2;
        _mix3 = mix3;
        _singleton2 = singleton2;
        _transient2 = transient2;
    }

    public void SayHi()
    {
        Console.WriteLine($"Hello from Complex 2");
        this._service1.SayHi();
        this._service2.SayHi();
        this._service3.SayHi();
        this._mix1.SayHi();
        this._mix2.SayHi();
        this._mix3.SayHi();
        this._singleton2.SayHi();
        this._transient2.SayHi();
    }
}

public class Complex3 : IComplex3
{
    private readonly IService1 _service1;
    private readonly IService2 _service2;
    private readonly IService3 _service3;
    private readonly IMix1 _mix1;
    private readonly IMix2 _mix2;
    private readonly IMix3 _mix3;
    private readonly ISingleton3 _singleton3;
    private readonly ITransient3 _transient3;

    public Complex3(IService1 service1, IService2 service2, IService3 service3, IMix1 mix1, IMix2 mix2, IMix3 mix3, ISingleton3 singleton3, ITransient3 transient3)
    {
        ArgumentNullException.ThrowIfNull(service1);
        ArgumentNullException.ThrowIfNull(service2);
        ArgumentNullException.ThrowIfNull(service3);
        ArgumentNullException.ThrowIfNull(mix1);
        ArgumentNullException.ThrowIfNull(mix2);
        ArgumentNullException.ThrowIfNull(mix3);
        ArgumentNullException.ThrowIfNull(singleton3);
        ArgumentNullException.ThrowIfNull(transient3);

        _service1 = service1;
        _service2 = service2;
        _service3 = service3;
        _mix1 = mix1;
        _mix2 = mix2;
        _mix3 = mix3;
        _singleton3 = singleton3;
        _transient3 = transient3;
    }

    public void SayHi()
    {
        Console.WriteLine($"Hello from Complex 3");
        this._service1.SayHi();
        this._service2.SayHi();
        this._service3.SayHi();
        this._mix1.SayHi();
        this._mix2.SayHi();
        this._mix3.SayHi();
        this._singleton3.SayHi();
        this._transient3.SayHi();
    }
}
