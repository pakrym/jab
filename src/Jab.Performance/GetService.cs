using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Jab.Performance
{
    [MemoryDiagnoser]
    public class GetService
    {
        private ServiceProvider _provider;
        private Container _container;

        public GetService()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IService, Service>();
            _provider = serviceCollection.BuildServiceProvider();
            _container = new Container();
        }

        [Benchmark]
        public void MEDI() => _provider.GetService<IService>();

        [Benchmark(Baseline = true)]
        public void Jab() => _container.GetService<IService>();
    }
}