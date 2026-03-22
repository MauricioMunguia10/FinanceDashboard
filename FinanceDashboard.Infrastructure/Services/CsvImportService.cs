using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using FinanceDashboard.Core.Entities;
using FinanceDashboard.Core.Enums;
using FinanceDashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Infrastructure.Services;

public class CsvImportService(ApplicationDbContext context)
{
    public async Task<int> ImportTrackWalletCsvAsync(Stream fileStream)
    {
        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });

        var records = new List<Transaction>();
        await csv.ReadAsync();
        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            var dateStr = csv.GetField("Date");
            var amountStr = csv.GetField("Amount");
            var accountName = csv.GetField("Account");
            var typeStr = csv.GetField("Transaction");
            var categoryName = csv.GetField("Category") ?? "Uncategorized";

            var hash = GenerateDeduplicationHash(dateStr!, accountName!, amountStr!, typeStr!);
            if (await context.Transactions.AnyAsync(t => t.DeduplicationHash == hash)) continue;
            
            var account = await context.Set<Account>().FirstOrDefaultAsync(a => a.Name == accountName) 
                          ?? await CreateAccount(accountName!);
            
            var category = await context.Set<Category>().FirstOrDefaultAsync(c => c.Name == categoryName)
                           ?? await CreateCategory(categoryName);

            var transaction = new Transaction
            {
                Date = DateTime.SpecifyKind(DateTime.Parse(dateStr!), DateTimeKind.Utc),
                Amount = Math.Abs(decimal.Parse(amountStr!, CultureInfo.InvariantCulture)),
                DeduplicationHash = hash,
                Type = typeStr!.Equals("Transfer", StringComparison.OrdinalIgnoreCase) ? TransactionType.Transfer : TransactionType.Expense,
                Description = csv.GetField("Note") ?? $"Imported {categoryName}",
                AccountId = account.Id,
                CategoryId = category.Id
            };

            records.Add(transaction);
        }

        if (records.Any())
        {
            context.Transactions.AddRange(records);
            return await context.SaveChangesAsync();
        }

        return 0;
    }

    private async Task<Account> CreateAccount(string name)
    {
        var account = new Account { Name = name, Type = AccountType.Asset, Currency = "MXN" };
        context.Set<Account>().Add(account);
        await context.SaveChangesAsync();
        return account;
    }

    private async Task<Category> CreateCategory(string name)
    {
        var category = new Category { Name = name, Icon = "bi-question", Color = "#808080" };
        context.Set<Category>().Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    private string GenerateDeduplicationHash(string date, string account, string amount, string type)
    {
        var rawData = $"{date}|{account}|{amount}|{type}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
        return Convert.ToHexString(bytes);
    }
}