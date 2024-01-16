# Jab Compile Time Dependency Injection

[![Nuget (with prereleases)](https://img.shields.io/nuget/v/Jab)](https://www.nuget.org/packages/Jab)

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Jab)](https://www.nuget.org/packages/Jab)

Jab provides a [C# Source Generator](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) based dependency injection container implementation.

- Fast startup (200x faster than Microsoft.Extensions.DependencyInjection). [Details](#Startup-Time).
- Fast resolution (7x faster than Microsoft.Extensions.DependencyInjection). [Details](#GetService).
- No runtime dependencies.
- AOT and linker friendly, all code is generated during project compilation.
- Clean stack traces:

    ![stacktrace](https://raw.githubusercontent.com/pakrym/jab/main/doc/stacktrace.png)
    
- Readable generated code:

    ![generated code](https://raw.githubusercontent.com/pakrym/jab/main/doc/generatedcode.png)

- Registration validation. Container configuration issues become compiler errors:

    ![generated code](https://raw.githubusercontent.com/pakrym/jab/main/doc/errors.png)
- Incremental generation, .NET 5/6 SDK support, .NET Standard 2.0 support

## Example

Add Jab package reference:
```xml
<ItemGroup>
    <PackageReference Include="Jab" Version="0.8.6" PrivateAssets="all" />
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
internal partial class MyServiceProvider { }
```

Use the service provider:

``` C#
MyServiceProvider c = new MyServiceProvider();
IService service = c.GetService<IService>();
```

## Features

- No runtime dependency, safe to use in libraries
- Transient, Singleton, Scoped service registration
- Factory registration
- Instance registration
- `IEnumerable` resolution
- `IDisposable` and `IAsyncDisposable` support
- `IServiceProvider` support

The plan is to support the minimum feature set Microsoft.Extensions.DependencyInjection.Abstraction requires but *NOT* the `IServiceCollection`-based registration syntax as it is runtime based.

### Singleton services

Singleton services are created once per container lifetime in a thread-safe manner and cached.
To register a singleton service use the `SingletonAttribute`:

```C#
[ServiceProvider]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
internal partial class MyServiceProvider { }
```

### Singleton Instances

If you want to use an existing object as a service define a property in the container declaration and use the `Instance` property of the `SingletonAttribute` to register the service:

```C#
[ServiceProvider]
[Singleton(typeof(IService), Instance = nameof(MyServiceInstance))]
internal partial class MyServiceProvider {
    public IService MyServiceInstance { get;set; }
}
```

Then initialize the property during the container creation:

```C#
MyServiceProvider c = new MyServiceProvider();
c.MyServiceInstance = new ServiceImplementation();

IService service = c.GetService<IService>();
```

### Factories

Sometimes it's useful to provide a custom way to create a service instance without using the automatic construction selection.
To do this define a method in the container declaration and use the `Factory` property of the `SingletonAttribute` or `TransientAttribute` to register the service:

```C#
[ServiceProvider]
[Transient(typeof(IService), Factory = nameof(MyServiceFactory))]
internal partial class MyServiceProvider {
    public IService MyServiceFactory() => new ServiceImplementation();
}

MyServiceProvider c = new MyServiceProvider();
IService service = c.GetService<IService>();
```

When using with `TransientAttribute` the factory method would be invoked for every service resolution.
When used with `SingletonAttribute` it would only be invoked the first time the service is requested.

### Scoped Services

Scoped services are created once per service provider scope. To create a scope use the `CreateScope()` method of the service provider.
Service are resolved from the scope using the `GetService<IService>()` call.

```C#
[ServiceProvider]
[Scoped(typeof(IService), typeof(ServiceImplementation))]
internal partial class MyServiceProvider { }

MyServiceProvider c = new MyServiceProvider();
using MyServiceProvider.Scope scope = c.CreateScope();
IService service = scope.GetService<IService>();
```

When the scope is disposed all `IDisposable` and `IAsyncDisposable` services that were resolved from it are disposed as well.

### Generic registration attributes 

You can use generic attributes to register services if your project targets `net7.0` or `net6.0` and has `LangVersion` set to preview.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net7.0</TargetFrameworks>
  </PropertyGroup>

</Project>

```

Generic attributes allow declaration to be more compact by avoiding the `typeof` calls:

``` C#
[ServiceProvider]
[Scoped<IService, ServiceImplementation>]
[Import<IMyModule>]
internal partial class MyServiceProvider { }
```

### Modules

Often, a set of service registrations would represent a distinct set of functionality that can be included into arbitrary 
service provider. Modules are used to implement registration sharing. To define a module create an interface and mark it with `ServiceProviderModuleAttribute`. Service registrations can be listed in module the same way they are in the service provider.

```C#
[ServiceProviderModule]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
public interface IMyModule
{
}
```

To use the module apply the `Import` attribute to the service provider type:

```C#
[ServiceProvider]
[Import(typeof(IMyModule))]
internal partial class MyServiceProvider
{
}

MyServiceProvider c = new MyServiceProvider();
IService service = c.GetService<IEnumerable<IService>>();
```

Modules can import other modules as well.

**NOTE**: module service and implementation types have to be accessible from the project where service provider is generated.

## Root services

By default, `IEnumerable<...>` service accessors are only generated when requested by other service constructors. If you would like to have a root `IEnumerable<..>` accessor generated use the `RootService` parameter of the `ServiceProvider` attribute. The generator also scans all the `GetService<...>` usages and tries to all collected type arguments as the root service.

``` C#
[ServiceProvider(RootServices = new [] {typeof(IEnumerable<IService>)})]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
internal partial class MyServiceProvider
{
}

MyServiceProvider c = new MyServiceProvider();
IService service = c.GetService<IEnumerable<IService>>();
```

## Samples

### Console application

Sample Jab usage in console application can be found in [src/samples/ConsoleSample](src/samples/ConsoleSample)

## Performance

The performance benchmark project is available in [src/Jab.Performance/](src/Jab.Performance/).

And the results in [docs/benchmark/](docs/benchmark/)

### Startup time

The startup time benchmark measures time between application startup and the first service being resolved.

| Method         | Mean         | Error         | StdDev      | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------- |-------------:|--------------:|------------:|-------:|-------:|----------:|------------:|
| Jab_Singleton  |     8.541 ns |     3.4715 ns |   0.1903 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Singleton | 2,162.589 ns | 2,026.5482 ns | 111.0819 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Scoped     |     9.227 ns |     4.8011 ns |   0.2632 ns | 0.0038 |      - |      32 B |        1.00 |
| MEDI_Scoped    | 1,895.721 ns | 2,150.1331 ns | 117.8560 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Transient  |     8.397 ns |     1.7967 ns |   0.0985 ns | 0.0038 |      - |      32 B |        1.00 |
| MEDI_Transient | 2,116.654 ns | 4,289.4329 ns | 235.1183 ns | 0.8640 | 0.2155 |    7232 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Mixed      |     9.697 ns |     0.1884 ns |   0.0103 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Mixed     | 2,519.538 ns | 1,988.6074 ns | 109.0023 ns | 1.0834 | 0.2689 |    9064 B |           ? |
|                |              |               |             |        |        |           |             |
| Jab_Complex    |    13.230 ns |     9.9845 ns |   0.5473 ns | 0.0067 |      - |      56 B |        1.00 |
| MEDI_Complex   | 2,429.244 ns | 1,541.6226 ns |  84.5015 ns | 1.1330 | 0.2823 |    9496 B |           ? |

### GetService

The `GetService` benchmark measures the `provider.GetService<IService>()` call.

### Singleton


| Method  | Mean         | Error         | StdDev        | Ratio    | RatioSD  |
|-------- |-------------:|--------------:|--------------:|---------:|---------:|
| **Jab** | **3.305 ns** | **2.1067 ns** | **0.1155 ns** | **1.00** | **0.00** |
| MEDI    |     9.419 ns |     6.5332 ns |     0.3581 ns |     2.85 |     0.12 |

### Transient

| Method  | Mean         | Error        | StdDev       | Ratio    | RatioSD  |
|-------- |-------------:|-------------:|-------------:|---------:|---------:|
| **Jab** | **11.33 ns** | **5.879 ns** | **0.322 ns** | **1.00** | **0.00** |
| MEDI    |     14.12 ns |     3.393 ns |     0.186 ns |     1.25 |     0.02 |

### Complex

| Method  | Mean         | Error        | StdDev      | Ratio    | RatioSD  |
|-------- |-------------:|-------------:|------------:|---------:|---------:|
| **Jab** | **277.0 ns** | **62.11 ns** | **3.40 ns** | **1.00** | **0.00** | 
| MEDI    |     162.0 ns |     47.78 ns |     2.62 ns |     0.59 |     0.02 |


## Debugging locally

Run `dotnet build /t:CreateLaunchSettings` in the `Jab.Tests` directory would update the `Jab\Properties\launchSettings.json` file to include `csc` invocation that allows F5 debugging of the generator targeting the `Jab.Tests` project.
