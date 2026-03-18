using FinanceDashboard.Core.Common;
using FinanceDashboard.Core.Enums;

namespace FinanceDashboard.Core.Entities;

public class Transaction : BaseAuditableEntity
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    
    // The "Anti-Duplicate" Shield
    public string DeduplicationHash { get; set; } = string.Empty; 
    
    public TransactionType Type { get; set; }
    
    // Foreign Keys
    public Guid AccountId { get; set; }
    public Account? Account { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }

    // For Transfers: Link the "Out" to the "In"
    public Guid? RelatedTransactionId { get; set; }
}