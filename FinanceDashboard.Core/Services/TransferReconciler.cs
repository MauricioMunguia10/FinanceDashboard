using FinanceDashboard.Core.Entities;
using FinanceDashboard.Core.Enums;

namespace FinanceDashboard.Core.Services;

public class TransferReconciler
{
    public void LinkTransfers(List<Transaction> transactions)
    {
        var transfers = transactions
            .Where(t => t is { Type: TransactionType.Transfer, RelatedTransactionId: null })
            .ToList();

        foreach (var outbound in transfers.Where(t => t.Amount < 0))
        {
            var inbound = transfers.FirstOrDefault(t => 
                t.Date == outbound.Date && 
                t.Amount == Math.Abs(outbound.Amount) && 
                t.Id != outbound.Id);

            if (inbound == null) continue;
            outbound.RelatedTransactionId = inbound.Id;
            inbound.RelatedTransactionId = outbound.Id;
                
            outbound.Description = $"Transfer to {inbound.Account?.Name}";
            inbound.Description = $"Transfer from {outbound.Account?.Name}";
        }
    }
}