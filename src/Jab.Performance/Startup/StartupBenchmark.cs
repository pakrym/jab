namespace Jab.Performance.Startup;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

[ShortRunJob]
[Config(typeof(Config))]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByJob, BenchmarkLogicalGroupRule.ByCategory, BenchmarkLogicalGroupRule.ByMethod, BenchmarkLogicalGroupRule.ByParams)]
[MemoryDiagnoser]
public partial class StartupBenchmark
{
    private class Config : ManualConfig
    {
        public Config()
        {
            this.HideColumns("Ratio", "RatioSD");
        }
    }

}
