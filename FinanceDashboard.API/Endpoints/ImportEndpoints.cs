using FinanceDashboard.API.Common;
using FinanceDashboard.Application.Transactions.Commands;
using MediatR;

namespace FinanceDashboard.API.Endpoints;

public class ImportEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup("api/imports")
            .WithTags("Imports")
            .DisableAntiforgery()
            .MapPost("/track-wallet", UploadTrackWalletCsv);
    }
    
    private static async Task<IResult> UploadTrackWalletCsv(
        IFormFile file, 
        ISender sender)
    {
        if (file.Length == 0) 
            return Results.BadRequest("No file uploaded.");

        await using var stream = file.OpenReadStream();
        
        var command = new ImportTrackWalletCsvCommand(stream);
        var result = await sender.Send(command);

        return Results.Ok(new { 
            Message = "Import completed successfully.", 
            RecordsInserted = result 
        });
    }
}