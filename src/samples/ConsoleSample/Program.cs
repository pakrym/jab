using Jab;
using ModuleSample;

new ServiceProvider().GetService<Program>().Run(args);

[ServiceProvider]
[Import(typeof(IModule))]
[Singleton(typeof(Program))]
[Singleton(typeof(Logger))]
partial class ServiceProvider
{
}

partial class Program
{
    private readonly Logger _logger;
    private readonly ServiceDefinedInAModule _serviceDefinedInAModule;

    public Program(Logger logger, ServiceDefinedInAModule serviceDefinedInAModule)
    {
        _logger = logger;
        _serviceDefinedInAModule = serviceDefinedInAModule;
    }

    public void Run(string[] args)
    {
        _logger.Log("Starting");
        _logger.LogError("Error happened");
    }
}