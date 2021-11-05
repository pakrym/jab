using System;
using Jab;
using ModuleSample;

namespace ConsoleSample
{
    [ServiceProvider]
    [Import(typeof(IModule))]
    [Singleton(typeof(Program))]
    [Singleton(typeof(Logger))]
    partial class ServiceProvider
    {
    }

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

    class Program
    {
        private readonly Logger _logger;
        private readonly ServiceDefinedInAModule _serviceDefinedInAModule;

        public Program(Logger logger, ServiceDefinedInAModule serviceDefinedInAModule)
        {
            _logger = logger;
            _serviceDefinedInAModule = serviceDefinedInAModule;
        }

        static void Main(string[] args) => new ServiceProvider().GetService<Program>().Run(args);

        public void Run(string[] args)
        {
            _logger.Log("Starting");
            _logger.LogError("Error happened");
        }
    }
}
