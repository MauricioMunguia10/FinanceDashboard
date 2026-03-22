using CsvHelper.Configuration;
using FinanceDashboard.Core.Entities;

namespace FinanceDashboard.Infrastructure.Data.Mappings;

public class TrackWalletMap : ClassMap<Transaction>
{
    public TrackWalletMap()
    {
        Map(m => m.Date).Name("Date").Convert(args => 
        {
            var rawDate = args.Row.GetField("Date");
            return DateTime.SpecifyKind(DateTime.Parse(rawDate!), DateTimeKind.Utc);
        });

        Map(m => m.Amount).Name("Amount").Convert(args => 
        {
            var rawAmount = args.Row.GetField<decimal>("Amount");
            return Math.Abs(rawAmount);
        });
        
        Map(m => m.Description).Name("Note");
    }
}