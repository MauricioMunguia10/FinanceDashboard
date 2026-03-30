using FinanceDashboard.Core.Entities;

namespace FinanceDashboard.Application.Common.Interfaces;

public interface ICsvImportService
{
    Task<List<Transaction>> ParseTrackWalletCsvAsync(Stream fileStream);
}