using System.Collections.Generic;

namespace JabTests;

#nullable enable

internal class ServiceImplementationWithNullableOptional : IService
{
    public IService1 Parameter1 { get; }
    public IService2? Parameter2 { get; }
    public IEnumerable<IService3>? Parameter3 { get; }

    public ServiceImplementationWithNullableOptional(
        IService1 parameter1,
        IService2? parameter2 = null,
        IEnumerable<IService3>? parameter3 = null)
    {
        Parameter1 = parameter1;
        Parameter2 = parameter2;
        Parameter3 = parameter3;
    }
}