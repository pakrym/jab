using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verify = JabTests.GeneratorAnalyzerVerifier<Jab.ContainerGenerator>;

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
        public async Task ProducesJAB0019WhenRequiredNamedDependencyNotFound()
        {
            string testCode = $@"
class Dependency {{ }}
class Service {{ public Service([FromNamedServices(""Named"")] Dependency dep) {{}} }}
[ServiceProvider]
[{{|#1:Transient(typeof(Service))|}}]
[Transient(typeof(Dependency))]
public partial class Container {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0019")
                    .WithLocation(1)
                    .WithArguments("Dependency", "Named", "Service"));
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
        public async Task ProducesJAB0011WhenBothImplementationAndFactory()
        {
            string testCode = $@"
public class Service {{ private Service() {{}} }}
[ServiceProvider]
[{{|#1:Transient(typeof(Service), typeof(Service), Factory=nameof(F))|}}]
[{{|#2:Singleton(typeof(Service), typeof(Service), Instance=nameof(F))|}}]
public partial class Container {{
    public Service F() => null;
}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0011")
                    .WithLocation(1)
                    .WithArguments("Service"),
                DiagnosticResult
                    .CompilerError("JAB0011")
                    .WithLocation(2)
                    .WithArguments("Service"));
        }


        [Fact]
        public async Task ProducesJAB0012WhenFactoryIsNotCallable()
        {
            string testCode = $@"
public class Service {{ private Service() {{}} }}
public class Service2 {{ private Service2() {{}} }}
[ServiceProvider]
[Transient(typeof(Service), Factory=nameof(F))]
[Transient(typeof(Service2), Factory=nameof(F1))]
public partial class Container {{
    public {{|#1:object|}} F = null;
    public {{|#2:object|}} F1 {{get;}} = null;
}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0012")
                    .WithLocation(1)
                    .WithArguments("F", "Service"),
                DiagnosticResult
                    .CompilerError("JAB0012")
                    .WithLocation(2)
                    .WithArguments("F1", "Service2"));
        }

        [Fact]
        public async Task ProducesJAB0008WhenCircularChainDetected()
        {
            string testCode = $@"
interface IService {{}}
class FirstService {{ public FirstService(IService s) {{}} }}
class Service : IService {{ public Service(AnotherService s) {{}} }}
class AnotherService {{ public AnotherService({{|#1:IService|}} s) {{}} }}
[ServiceProvider]
[Transient(typeof(FirstService))]
[Transient(typeof(IService), typeof(Service))]
[Transient(typeof(AnotherService))]
public partial class Container {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0008")
                    .WithLocation(1)
                    .WithArguments("IService", "FirstService -> IService -> Service -> AnotherService -> IService"));
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

        [Fact]
        public async Task ProducesJAB0010OrJAB0018IfGetServiceCallTypeUnregistered()
        {
            string testCode = $@"
interface IService {{}}
[ServiceProvider]
public partial class Container {{
    public T GetService<T>() => default;
    public T GetService<T>(string name) => default;
    public static void Main() {{ 
        var container = new Container(); 
        {{|#1:container.GetService<IService>()|}}; 
        {{|#2:container.GetService<IService>(""Named"")|}}; 
    }}
}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0010")
                    .WithLocation(1)
                    .WithArguments("IService"),

                DiagnosticResult
                    .CompilerError("JAB0018")
                    .WithLocation(2)
                    .WithArguments("IService", "Named"));
        }

        [Fact]
        public async Task ProducesJAB0010IfRootServicesTypeUnregistered()
        {
            string testCode = $@"
interface IService {{}}
[{{|#1:ServiceProvider(RootServices=new[] {{ typeof(IService) }})|}}]
public partial class Container {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0010")
                    .WithLocation(1)
                    .WithArguments("IService"));
        }

        [Fact]
        public async Task ProducesDiagnosticWhenServiceNameNotAlphanumeric()
        {
            string testCode = @"
public class Service {}
[ServiceProvider]
[{|#1:Singleton(typeof(Service), Name = """")|}]
[{|#2:Singleton(typeof(Service), Name = ""'"")|}]
[{|#3:Singleton(typeof(Service), Name = ""1a"")|}]
[Singleton(typeof(Service), Name = ""aA10"")]
public partial class Container {}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0015")
                    .WithLocation(1)
                    .WithArguments(""),

                DiagnosticResult
                    .CompilerError("JAB0015")
                    .WithLocation(2)
                    .WithArguments("'"),

                DiagnosticResult
                    .CompilerError("JAB0015")
                    .WithLocation(3)
                    .WithArguments("1a"));
        }

        [Fact]
        public async Task ProducesDiagnosticWhenBuiltInServicesRequestedAsNamed()
        {
            string testCode = @"
public class Service {
    public Service(
        [FromNamedServices(""A"")] {|#1:IServiceProvider|} sp
    ) {}
}

[ServiceProvider]
[Singleton(typeof(Service))]
public partial class Container {}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0016")
                    .WithLocation(1)
                    .WithArguments("System.IServiceProvider"));
        }


        [Fact]
        public async Task ProducesDiagnosticWhenImplicitIEnumerableRequestedAsNamed()
        {
            string testCode = @"
public class Service1 {}
public class Service {
    public Service(
        [FromNamedServices(""A"")] {|#1:IEnumerable<Service1>|} s,
        IEnumerable<Service1> ss
    ) {}
}

[ServiceProvider]
[Singleton(typeof(Service))]
[Singleton(typeof(Service1))]
public partial class Container {}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0017")
                    .WithLocation(1)
                    .WithArguments("System.Collections.Generic.IEnumerable<Service1>"));
        }

        [Fact]
        public async Task ProducesJAB0013WhenNullableNonOptionalDependencyNotFound()
        {
            string testCode = $@"
#nullable enable
interface IDependency {{ }}
class Service {{ public Service(IDependency? dep) {{}} }}
[ServiceProvider]
[{{|#1:Transient(typeof(Service))|}}]
public partial class Container {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0013")
                    .WithSeverity(DiagnosticSeverity.Error)
                    .WithLocation(1)
                    .WithArguments("IDependency?", "Service"));
        }

        [Fact]
        public async Task ProducesJAB0014WhenNullableNonOptionalDependencyFound()
        {
            string testCode = $@"
#nullable enable
interface IDependency {{ }}
class Dependency : IDependency {{ }}
class Service {{ public Service(IDependency? dep) {{}} }}
[ServiceProvider]
[{{|#1:Transient(typeof(Service))|}}]
[{{|#2:Transient(typeof(IDependency), typeof(Dependency))|}}]
public partial class Container {{}}
";
            await Verify.VerifyAnalyzerAsync(testCode,
                DiagnosticResult
                    .CompilerError("JAB0014")
                    .WithSeverity(DiagnosticSeverity.Info)
                    .WithLocation(1)
                    .WithArguments("IDependency?", "Service"));
        }
    }
}