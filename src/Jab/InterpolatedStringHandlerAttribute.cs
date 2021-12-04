namespace System.Runtime.CompilerServices;

public sealed class InterpolatedStringHandlerAttribute : Attribute
{
}

[System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class InterpolatedStringHandlerArgumentAttribute : Attribute
{
    public InterpolatedStringHandlerArgumentAttribute (params string[] arguments){}
}


public sealed class IsExternalInit : Attribute
{
}
