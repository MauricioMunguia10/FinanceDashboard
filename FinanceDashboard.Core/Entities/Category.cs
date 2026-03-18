using FinanceDashboard.Core.Common;

namespace FinanceDashboard.Core.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "bi-tag";
    public string Color { get; set; } = "#808080";
    public bool IsSystem { get; set; } = false;
}