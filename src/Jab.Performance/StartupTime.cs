using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Jab.Performance
{
    [MemoryDiagnoser]
    public class StartupTime
    {
        [Benchmark]
        public void MEDI()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IService, Service>();
            var provider = serviceCollection.BuildServiceProvider();
            provider.GetService<IService>();
        }

        [Benchmark(Baseline = true)]
        public void Jab()
        {
            var provider = new Container();
            provider.GetService<IService>();
        }
    }
}