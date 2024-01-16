namespace Jab.Performance.Basic.Transient;

public interface ITransient1
{
    void SayHi();
}

public interface ITransient2
{
    void SayHi();
}

public interface ITransient3
{
    void SayHi();
}

public class Transient1() : ITransient1
{
    public void SayHi() => Console.WriteLine("Hello from Transient 1");
}
public class Transient2() : ITransient2
{
    public void SayHi() => Console.WriteLine("Hello from Transient 2");
}
public class Transient3() : ITransient3
{
    public void SayHi() => Console.WriteLine("Hello from Transient 3");
}
