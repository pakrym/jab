# Jab Compile Time Dependency Injection

[![Nuget](https://img.shields.io/nuget/v/Jab)](https://www.nuget.org/packages/Jab)

Jab provides a [C# Source Generator](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) based dependency injection container implementation.

- Fast startup (200x faster than Microsoft.Extensions.DependencyInjection). [Details](#Startup-Time).
- Fast resolution (7x faster than Microsoft.Extensions.DependencyInjection). [Details](#GetService).
- No runtime dependencies.
- AOT and linker friendly, all code is generated during project compilation.
- Clean stack traces: <br> ![stacktrace](https://raw.githubusercontent.com/pakrym/jab/main/doc/stacktrace.png)
- Readable generated code: <br> ![generated code](https://raw.githubusercontent.com/pakrym/jab/main/doc/generatedcode.png)
- Registration validation. Container configuration issues become compiler errors: <br> ![generated code](https://raw.githubusercontent.com/pakrym/jab/main/doc/errors.png)
- Incremental generation, .NET 5/6/7/8 SDK support, .NET Standard 2.0 support, [Unity support](README.md#Unity-installation)

## Example

Add Jab package reference:
```xml
<ItemGroup>
    <PackageReference Include="Jab" Version="0.10.2" PrivateAssets="all" />
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
- Named registrations
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

### Named services

Use the `Name` property to assign a name to your service registrations and `[FromNamedServices("...")]` attribute to resolve a service using its name.

```C#
[ServiceProvider]
[Singleton(typeof(INotificationService), typeof(EmailNotificationService), Name="email")]
[Singleton(typeof(INotificationService), typeof(SmsNotificationService), Name="sms")]
[Singleton(typeof(Notifier))]
internal partial class MyServiceProvider {}

class Notifier
{
    public Notifier(
        [FromNamedServices("email")] INotificationService email,
        [FromNamedServices("sms")] INotificationService sms)
    {}
}
```

NOTE: Jab also recognizes the `[FromKeyedServices]` attribute from `Microsoft.Extensions.DependencyInjection`.

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

Similar to constructors, factories support parameter injection:

```
[ServiceProvider]
[Transient(typeof(IService), Factory = nameof(MyServiceFactory))]
[Transient(typeof(SomeOtherService))]
internal partial class MyServiceProvider {
    public IService MyServiceFactory(SomeOtherService other) => new ServiceImplementation(other);
}
```

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

| Method         | Mean         | Error         | StdDev      | Gen0   | Gen1   | Allocated |
|--------------- |-------------:|--------------:|------------:|-------:|-------:|----------:|
| Jab_Singleton  |     8.763 ns |     0.5496 ns |   0.0301 ns | 0.0067 |      - |      56 B |
| MEDI_Singleton | 1,883.539 ns | 1,976.8621 ns | 108.3585 ns | 0.8640 | 0.2155 |    7232 B |
|                |              |               |             |        |        |           |
| Jab_Scoped     |     8.445 ns |     6.0425 ns |   0.3312 ns | 0.0038 |      - |      32 B |
| MEDI_Scoped    | 1,894.341 ns | 1,854.7252 ns | 101.6637 ns | 0.8640 | 0.2155 |    7232 B |
|                |              |               |             |        |        |           |
| Jab_Transient  |     8.695 ns |    11.2536 ns |   0.6168 ns | 0.0038 |      - |      32 B |
| MEDI_Transient | 1,894.840 ns | 2,000.5040 ns | 109.6544 ns | 0.8640 | 0.2155 |    7232 B |
|                |              |               |             |        |        |           |
| Jab_Mixed      |     9.722 ns |     2.2865 ns |   0.1253 ns | 0.0067 |      - |      56 B |
| MEDI_Mixed     | 2,454.882 ns | 2,626.4526 ns | 143.9647 ns | 1.0834 | 0.2689 |    9064 B |
|                |              |               |             |        |        |           |
| Jab_Complex    |    13.402 ns |     4.5401 ns |   0.2489 ns | 0.0067 |      - |      56 B |
| MEDI_Complex   | 2,350.227 ns | 2,335.5694 ns | 128.0204 ns | 1.1330 | 0.2823 |    9496 B |

### GetService

The `GetService` benchmark measures the `provider.GetService<IService>()` call.

### Singleton

| Method  | Mean         | Error         | StdDev        | Ratio    | RatioSD  |
|-------- |-------------:|--------------:|--------------:|---------:|---------:|
| **Jab** | **3.325 ns** | **2.1053 ns** | **0.1154 ns** | **1.00** | **0.00** |
| MEDI    |     9.241 ns |     0.2836 ns |     0.0155 ns |     2.78 |     0.09 |

### Transient

| Method  | Mean         | Error        | StdDev       | Ratio    | RatioSD  |
|-------- |-------------:|-------------:|-------------:|---------:|---------:|
| **Jab** | **11.29 ns** | **3.599 ns** | **0.197 ns** | **1.00** | **0.00** |
| MEDI    |     13.46 ns |     1.805 ns |     0.099 ns |     1.19 |     0.03 |


### Complex

| Method  | Mean         | Error         | StdDev      | Ratio     | RatioSD  |
|-------- |-------------:|--------------:|------------:|----------:|---------:|
| **Jab** | **279.6 ns** | **154.26 ns** | **8.46 ns** |  **1.00** | **0.00** |
| MEDI    |     149.2 ns |      20.02 ns |     1.10 ns |      0.53 |     0.02 |



## Unity installation
1. Navigate to the Packages directory of your project.
2. Adjust the [project manifest file](https://docs.unity3d.com/Manual/upm-manifestPrj.html) manifest.json in a text editor.
3. Ensure `https://registry.npmjs.org/` is part of `scopedRegistries`.
4. Ensure `com.pakrym` is part of `scopes`.
5. Add `com.pakrym.jab` to the dependencies, stating the latest version.

A minimal example ends up looking like this:

```
{
  "scopedRegistries": [
    {
      "name": "npmjs",
      "url": "https://registry.npmjs.org/",
      "scopes": [
        "com.pakrym"
      ]
    }
  ],
  "dependencies": {
    "com.pakrym.jab": "0.10.2",
    ...
  }
}
```


## Debugging locally

Run `dotnet build /t:CreateLaunchSettings` in the `Jab.Tests` directory would update the `Jab\Properties\launchSettings.json` file to include `csc` invocation that allows F5 debugging of the generator targeting the `Jab.Tests` project.
