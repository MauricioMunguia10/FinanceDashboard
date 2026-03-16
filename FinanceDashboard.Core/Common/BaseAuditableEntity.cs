namespace FinanceDashboard.Core.Common;

public abstract class BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastModifiedAt { get; set; }
}