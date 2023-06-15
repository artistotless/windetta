namespace Windetta.Common.Types;

public class AuditableEntity : BaseEntity
{
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
