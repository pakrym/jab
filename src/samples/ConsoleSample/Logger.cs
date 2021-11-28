using System;

public class Logger
{
    public void Log(string message)
    {
        Console.Error.WriteLine(message);
    }

    public void LogError(string message)
    {
        Console.Error.WriteLine(message);
    }
}