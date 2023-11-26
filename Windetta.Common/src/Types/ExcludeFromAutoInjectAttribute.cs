namespace Windetta.Common.Types;

[AttributeUsage(AttributeTargets.Class)]
public class ExcludeFromAutoInjectAttribute : Attribute // marker for autoinjecting
{
}
