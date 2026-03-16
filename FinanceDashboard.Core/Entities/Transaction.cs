using FinanceDashboard.Core.Common;
using FinanceDashboard.Core.Enums;

namespace FinanceDashboard.Core.Entities;

public class Transaction : BaseAuditableEntity
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public string Source { get; set; } = string.Empty; 
    public string? ExternalId { get; set; } 
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
}