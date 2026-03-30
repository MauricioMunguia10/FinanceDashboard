using FinanceDashboard.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Transaction> Transactions { get; }
    DbSet<Account> Accounts { get; }
    DbSet<Category> Categories { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}