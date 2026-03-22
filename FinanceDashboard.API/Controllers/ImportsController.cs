using FinanceDashboard.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceDashboard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportsController(CsvImportService importService) : ControllerBase
{
    [HttpPost("track-wallet")]
    public async Task<IActionResult> UploadTrackWalletCsv(IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest("No file uploaded.");

        await using var stream = file.OpenReadStream();
        
        var result = await importService.ImportTrackWalletCsvAsync(stream);

        return Ok(new { 
            Message = "Import completed successfully.", 
            RecordsInserted = result 
        });
    }
}