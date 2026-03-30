using FinanceDashboard.Infrastructure;
using FinanceDashboard.API.Common;
using FinanceDashboard.Application;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddAntiforgery();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAntiforgery();
app.UseAuthorization();

app.MapControllers(); 

var endpointGroups = typeof(Program).Assembly.GetTypes()
    .Where(t => t.IsSubclassOf(typeof(EndpointGroupBase)) && !t.IsAbstract);

foreach (var type in endpointGroups)
{
    ((EndpointGroupBase)Activator.CreateInstance(type)!).Map(app);
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FinanceDashboard.Infrastructure.Data.ApplicationDbContext>();
        await context.Database.MigrateAsync(); 
        Console.WriteLine("Database migrated successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
    }
}

app.Run();