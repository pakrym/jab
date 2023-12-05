namespace Jab.Performance.Basic.Scoped;

public interface IScoped1
{
    void SayHi();
}

public interface IScoped2
{
    void SayHi();
}

public interface IScoped3
{
    void SayHi();
}

public class Scoped1() : IScoped1
{
    public void SayHi() => Console.WriteLine("Hello from Scoped 1");
}
public class Scoped2() : IScoped2
{
    public void SayHi() => Console.WriteLine("Hello from Scoped 2");
}
public class Scoped3() : IScoped3
{
    public void SayHi() => Console.WriteLine("Hello from Scoped 3");
}
