using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using FinanceDashboard.Application.Common.Interfaces;
using FinanceDashboard.Core.Entities;
using FinanceDashboard.Core.Enums;
using FinanceDashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Infrastructure.Services;

public class CsvImportService(ApplicationDbContext context) : ICsvImportService
{
    public async Task<List<Transaction>> ParseTrackWalletCsvAsync(Stream fileStream)
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
            
            var account = await context.Accounts.FirstOrDefaultAsync(a => a.Name == accountName) 
                          ?? await CreateAccountAsync(accountName!);
            
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName)
                           ?? await CreateCategoryAsync(categoryName);

            var transaction = new Transaction
            {
                Date = DateTime.SpecifyKind(DateTime.Parse(dateStr!), DateTimeKind.Utc),
                Amount = Math.Abs(decimal.Parse(amountStr!, CultureInfo.InvariantCulture)),
                DeduplicationHash = hash,
                Type = typeStr!.Equals("Transfer", StringComparison.OrdinalIgnoreCase) ? TransactionType.Transfer : TransactionType.Expense,
                Description = csv.GetField("Note") ?? $"Imported {categoryName}",
                AccountId = account.Id,
                Account = account,
                CategoryId = category.Id,
                Category = category
            };

            records.Add(transaction);
        }

        return records;
    }

    private async Task<Account> CreateAccountAsync(string name)
    {
        var account = context.Accounts.Local.FirstOrDefault(a => a.Name == name);
        if (account != null) return account;

        account = new Account { Name = name, Type = AccountType.Asset, Currency = "MXN" };
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return account;
    }

    private async Task<Category> CreateCategoryAsync(string name)
    {
        var category = context.Categories.Local.FirstOrDefault(c => c.Name == name);
        if (category != null) return category;

        category = new Category { Name = name, Icon = "bi-question", Color = "#808080" };
        context.Categories.Add(category);
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