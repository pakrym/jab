namespace Jab;

public readonly record struct ServiceIdentity
{
    public ITypeSymbol Type { get; }
    public string? Name { get; }
    public int? ReverseIndex { get; }

    public ServiceIdentity(ITypeSymbol serviceType, string? name, int? reverseIndex)
    {
        Type = serviceType;
        Name = name;
        ReverseIndex = reverseIndex == 0 ? null : reverseIndex;
    }
}