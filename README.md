# Jab Compile Time Dependency Injection

**NOTE:** this is an extremely early prototype not intended for any production use.

Jab provides a [C# Source Generator](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) based dependency injection container implementation.

Jab brings has no runtime dependencies.

Jab is AOT and linker friendly, all code is generated at compile time.

Jab allows easy debugging of the service resolution process.

## Example

Add Jab package reference:
```xml
<ItemGroup>
    <PackageReference Include="Jab" Version="0.0.1-beta.1" PrivateAssets="all" />
</ItemGroup>
```

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
[ServiceProvider]
[Transient(typeof(IService), typeof(ServiceImplementation))]
internal partial class MyContainer { }
```

Use the container:

``` C#
MyContainer c = new();
IService service = c.GetService<IService>();
```

## Features

- No runtime dependency, safe to use in libraries
- Transient, Singleton service registration
- Factory registration
- Instance registration
- IEnumerable resolution

The plan is to support the minimum feature set Microsoft.Extensions.DependencyInjection.Abstraction requires but *NOT* the `IServiceCollection`-based registration syntax as it is runtime based.

### Singleton services

Singleton services are created once per container lifetime in a thread-safe manner and cached.
To register a singleton service use the `SingletonAttribute`:

```C#
[ServiceProvider]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
internal partial class MyContainer { }
```

### Singleton Instances

If you want to use an existing object as a service define a property in the container declaration and use the `Instance` property of the `SingletonAttribute` to register the service:

```C#
[ServiceProvider]
[Singleton(typeof(IService), Instance = nameof(MyServiceInstance))]
internal partial class MyContainer {
    public IService MyServiceInstance { get;set; }
}
```

Then initialize the property during the container creation:

```C#
MyContainer c = new();
c.MyServiceInstance = new ServiceImplementation();

IService service = c.GetService<IService>();
```

### Factories

Sometimes it's useful to provide a custom way to create a service instance without using the automatic construction selection.
To do this define a method in the container declaration and use the `Factory` property of the `SingletonAttribute` or `TransientAttribute` to register the service:

```C#
[ServiceProvider]
[Transient(typeof(IService), Factory = nameof(MyServiceFactory))]
internal partial class MyContainer {
    public IService MyServiceFactory() => new ServiceImplementation();
}

MyContainer c = new();
IService service = c.GetService<IService>();
```

When using with `TransientAttribute` the factory method would be invoked for every service resolution.
When used with `SingletonAttribute` it would only be invoked the first time the service is requested.

## Root services

By default, `IEnumerable<...>` service accessors are only generated when requested by other service constructors. If you would like to have a root `IEnumerable<..>` accessor generated use the `RootService` parameter of the `ServiceProvider` attribute. The generator also scans all the `GetService<...>` usages and tries to all collected type arguments as the root service.

``` C#
[ServiceProvider(RootServices = new [] {typeof(IEnumerable<IService>)})]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
internal partial class MyContainer
{
}

MyContainer c = new();
IService service = c.GetService<IEnumerable<IService>>();
```

## Samples

### Console application

Sample Jab usage in console application can be found in [src/samples/ConsoleSample](src/samples/ConsoleSample)

## Debugging locally

Run `dotnet build /t:CreateLaunchSettings` in the `Jab.Tests` directory would update the `Jab\Properties\launchSettings.json` file to include `csc` invocation that allows F5 debugging of the generator targeting the `Jab.Tests` project.
