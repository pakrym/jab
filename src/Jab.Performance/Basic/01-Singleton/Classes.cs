namespace Jab.Performance.Basic.Singleton;

public interface ISingleton1
{
    void SayHi();
}

public interface ISingleton2
{
    void SayHi();
}

public interface ISingleton3
{
    void SayHi();
}

public class Singleton1() : ISingleton1
{
    public void SayHi() => Console.WriteLine("Hello from Singleton 1");
}
public class Singleton2() : ISingleton2
{
    public void SayHi() => Console.WriteLine("Hello from Singleton 2");
}
public class Singleton3() : ISingleton3
{
    public void SayHi() => Console.WriteLine("Hello from Singleton 3");
}
