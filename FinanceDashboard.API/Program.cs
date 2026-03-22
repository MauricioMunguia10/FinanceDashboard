using FinanceDashboard.Infrastructure;
using FinanceDashboard.API.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var endpointGroups = typeof(Program).Assembly.GetTypes()
    .Where(t => t.IsSubclassOf(typeof(EndpointGroupBase)) && !t.IsAbstract);

foreach (var type in endpointGroups)
{
    ((EndpointGroupBase)Activator.CreateInstance(type)!).Map(app);
}

app.Run();