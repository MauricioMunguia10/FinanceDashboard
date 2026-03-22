using FinanceDashboard.API.Common;
using FinanceDashboard.Infrastructure.Services;

namespace FinanceDashboard.API.Endpoints;

public class ImportEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup("api/imports")
            .WithTags("Imports")
            .MapPost("/track-wallet", UploadTrackWalletCsv);
    }

    private static async Task<IResult> UploadTrackWalletCsv(
        IFormFile file, 
        CsvImportService importService)
    {
        if (file.Length == 0) return Results.BadRequest("No file.");

        await using var stream = file.OpenReadStream();
        var result = await importService.ImportTrackWalletCsvAsync(stream);

        return Results.Ok(new { RecordsInserted = result });
    }
}