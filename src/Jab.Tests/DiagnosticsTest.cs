using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verify = Jab.Tests.GeneratorAnalyzerVerifier<Jab.DependencyInjectionContainerGenerator>;

namespace Jab.Tests
{
    public class DiagnosticsTest
    {
        [Fact]
        public async Task ProducesDiagnosticWhenInstanceMemberNotFound()
        {
            string testCode = @"
[CompositionRoot]
[{|#1:Singleton(typeof(ICloneable), Instance = ""CreateCloneable"")|}]
public class Container {}
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
[CompositionRoot]
[{{|#1:{attribute}(typeof(ICloneable), Factory = ""CreateCloneable"")|}}]
public class Container {{}}
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
[CompositionRoot]
[{{|#1:{attribute}(typeof(ICloneable), Factory = ""CreateCloneable"")|}}]
public class Container {{
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