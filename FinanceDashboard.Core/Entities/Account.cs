using FinanceDashboard.Core.Common;
using FinanceDashboard.Core.Enums;

namespace FinanceDashboard.Core.Entities;

public class Account : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "MXN"; // Default for now
    public AccountType Type { get; set; }
    public decimal InitialBalance { get; set; }
}