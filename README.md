# Jab Compile Time Dependency Injection

**NOTE:** this is an extremely early prototype not intended for any production use.

Jab provides a [C# Source Generator](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) based dependency injection container implementation.

Jab brings has no runtime dependencies.

Jab is AOT and linker friendly, all code is generated at compile time.

Jab allows easy debugging of service resolution process.

## Example

Define a service and implementation:

``` C#
internal interface IService
{
    void M();
}

internal class ServiceImplementation : IService
{
    public void M()
    {
    }
}
```

Define a composition root and register services:

```C#
[CompositionRoot]
[Transient(typeof(IService), typeof(ServiceImplementation))]
internal partial class MyContainer { }
```

Use the container:

``` C#
MyContainer c = new();
IService service = c.GetIService();
```

## Features

The plan is to support the minimum feature set Microsoft.Extensions.DependencyInjection.Abstraction requires but *NOT* the `IServiceCollection`-based registration syntax as it is runtime based.