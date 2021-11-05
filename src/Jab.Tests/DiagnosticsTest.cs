﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verify = JabTests.GeneratorAnalyzerVerifier<Jab.DependencyInjectionContainerGenerator>;

namespace JabTests
{
    public class DiagnosticsTest
    {
        [Fact]
        public async Task ProducesDiagnosticWhenModuleNotMarkedWithAttribute()
        {
            string testCode = @"

public interface IModule {}

[ServiceProvider]
[{|#1:Import(typeof(IModule))|}]
public partial class Container {}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0006")
                    .WithLocation(1)
                    .WithArguments("IModule", "ServiceProviderModuleAttribute"));
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
                    .CompilerError("JAB0005")
                    .WithLocation(1)
                    .WithArguments("Container"));
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
                    .CompilerError("JAB0003")
                    .WithLocation(1)
                    .WithArguments("CreateCloneable", "Instance"));
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
                    .CompilerError("JAB0003")
                    .WithLocation(1)
                    .WithArguments("CreateCloneable", "Factory"));
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
                    .CompilerError("JAB0004")
                    .WithLocation(1)
                    .WithArguments("CreateCloneable", "Factory"));
        }

        [Fact]
        public async Task ProducesJAB0002WhenRequiredDependencyNotFound()
        {
            string testCode = $@"
interface IDependency {{ }}
class Service {{ public Service(IDependency dep) {{}} }}
[ServiceProvider]
[{{|#1:Transient(typeof(Service))|}}]
public partial class Container {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0002")
                    .WithLocation(1)
                    .WithArguments("IDependency", "Service"));
        }

        [Fact]
        public async Task ProducesJAB0002WhenRequiredDependenciesNotFound()
        {
            string testCode = $@"
interface IDependency {{ }}
interface IDependency2 {{ }}
class Service {{ public Service(IDependency dep, IDependency2 dep2) {{}} }}
[ServiceProvider]
[{{|#1:Transient(typeof(Service))|}}]
public partial class Container {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0002")
                    .WithLocation(1)
                    .WithArguments("IDependency", "Service"),
                DiagnosticResult
                    .CompilerError("JAB0002")
                    .WithLocation(1)
                    .WithArguments("IDependency2", "Service"));
        }

        [Fact]
        public async Task ProducesJAB0007WhenRequiredImplementationHasNoPublicConstructors()
        {
            string testCode = $@"
class Service {{ private Service() {{}} }}
[ServiceProvider]
[{{|#1:Transient(typeof(Service))|}}]
public partial class Container {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0007")
                    .WithLocation(1)
                    .WithArguments("Service"));
        }

        [Fact]
        public async Task ProducesJAB0008WhenCircularChainDetected()
        {
            string testCode = $@"
interface IService {{}}
class FirstService {{ public FirstService(IService s) {{}} }}
class Service : IService {{ public Service(AnotherService s) {{}} }}
class AnotherService {{ public AnotherService(IService s) {{}} }}
[ServiceProvider]
[{{|#1:Transient(typeof(FirstService))|}}]
[Transient(typeof(IService), typeof(Service))]
[Transient(typeof(AnotherService))]
public partial class Container {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0008")
                    .WithLocation(1)
                    .WithArguments("FirstService", "IService", "FirstService -> IService -> Service -> AnotherService -> IService"));
        }

        [Fact]
        public async Task ProducesJAB0009ForRegistrationsWithoutServiceProvider()
        {
            string testCode = $@"
interface IService {{}}
[Transient(typeof(IService))]
public partial class {{|#1:Container|}} {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0009")
                    .WithLocation(1)
                    .WithArguments("Container"));
        }

        [Fact]
        public async Task ProducesJAB0009ForInterafaceWithRegistrationsWithoutServiceProvider()
        {
            string testCode = $@"
interface IService {{}}
[Transient(typeof(IService))]
interface {{|#1:Container|}} {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0009")
                    .WithLocation(1)
                    .WithArguments("Container"));
        }
    }
}