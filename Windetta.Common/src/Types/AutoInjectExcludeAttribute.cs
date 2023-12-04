namespace Windetta.Common.Types;

/// <summary>
/// Marker fro exclude from auto injecting dependency
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AutoInjectExcludeAttribute : Attribute
{
}


/// <summary>
/// Specifies the service type for the implementation
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AutoInjectServiceTypeAttribute<T> : Attribute where T : class
{
    public Type TypeOfService => typeof(T);
    public static string NameOfProperty => nameof(TypeOfService); // dont touch
}