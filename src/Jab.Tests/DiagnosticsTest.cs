using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verify = Jab.Tests.GeneratorAnalyzerVerifier<Jab.DependencyInjectionContainerGenerator>;

namespace Jab.Tests
{
    public class DiagnosticsTest
    {
        [Fact]
        public async Task ProducesDiagnosticWhenModuleNotMarkedWithAttribute()
        {
            string testCode = @"

[Singleton(typeof(Object))]
public interface IModule {}

[ServiceProvider]
[{|#1:Import(typeof(IModule))|}]
public partial class Container {}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0001")
                    .WithLocation(1)
                    .WithArguments("The imported type 'IModule' is not marked with the 'Jab.ServiceProviderModuleAttribute'."));
        }

        [Fact]
        public async Task ProducesDiagnosticWhenServiceProviderIsNotPartial()
        {
            string testCode = @"
[ServiceProvider]
[Singleton(typeof(Object))]
public class {|#1:Container|} {}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0001")
                    .WithLocation(1)
                    .WithArguments("The type marked with the ServiceProvider attribute has to be marked partial."));
        }

        [Fact]
        public async Task ProducesDiagnosticWhenInstanceMemberNotFound()
        {
            string testCode = @"
[ServiceProvider]
[{|#1:Singleton(typeof(ICloneable), Instance = ""CreateCloneable"")|}]
public partial class Container {}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0001")
                    .WithLocation(1)
                    .WithArguments("Unable to find a member 'CreateCloneable' referenced in the 'Instance' attribute parameter."));
        }

        [Theory]
        [InlineData("Singleton")]
        [InlineData("Transient")]
        public async Task ProducesDiagnosticWhenFactoryMemberNotFound(string attribute)
        {
            string testCode = $@"
[ServiceProvider]
[{{|#1:{attribute}(typeof(ICloneable), Factory = ""CreateCloneable"")|}}]
public partial class Container {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0001")
                    .WithLocation(1)
                    .WithArguments("Unable to find a member 'CreateCloneable' referenced in the 'Factory' attribute parameter."));
        }

        [Theory]
        [InlineData("Singleton")]
        [InlineData("Transient")]
        public async Task ProducesDiagnosticWhenFactoryMemberIsAmbiguous(string attribute)
        {
            string testCode = $@"
[ServiceProvider]
[{{|#1:{attribute}(typeof(ICloneable), Factory = ""CreateCloneable"")|}}]
public partial class Container {{
ICloneable CreateCloneable() => null;
ICloneable CreateCloneable(int why) => null;
}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0001")
                    .WithLocation(1)
                    .WithArguments("Found multiple members with the 'CreateCloneable' name, referenced in the 'Factory' attribute parameter."));
        }
    }
}