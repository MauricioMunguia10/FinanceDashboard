using FinanceDashboard.Application.Common.Interfaces;
using FinanceDashboard.Core.Services;
using MediatR;

namespace FinanceDashboard.Application.Transactions.Commands;

public class ImportTrackWalletCsvHandler(IApplicationDbContext context, ICsvImportService csvService)
    : IRequestHandler<ImportTrackWalletCsvCommand, int>
{
    private readonly TransferReconciler _reconciler = new();

    public async Task<int> Handle(ImportTrackWalletCsvCommand request, CancellationToken cancellationToken)
    {
        var transactions = await csvService.ParseTrackWalletCsvAsync(request.FileStream);
        
        _reconciler.LinkTransfers(transactions);
        
        context.Transactions.AddRange(transactions);
        return await context.SaveChangesAsync(cancellationToken);
    }
}