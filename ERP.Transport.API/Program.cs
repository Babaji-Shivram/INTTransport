using ERP.Transport.API.Extensions;
using ERP.Transport.API.Middleware;
using ERP.Transport.Infrastructure.Data;
using EPR.Shared.Contracts.Extensions;
using EPR.Shared.Contracts.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ── Services ────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddJwtAuthentication(configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddTransportServices(configuration);
builder.Services.AddExternalServiceClients(configuration);
builder.Services.AddCorsPolicy();

// SharedLib
builder.Services.AddSharedServices(configuration);
builder.Services.AddSharedCompression();

// Cross-cutting
builder.Services.AddTransportApiVersioning();
builder.Services.AddTransportHealthChecks();
builder.Services.AddTransportRateLimiting(configuration);

var app = builder.Build();

// ── Middleware Pipeline ─────────────────────────────────────────
// Global exception handler — must be first to catch all unhandled exceptions
app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transport API v1");
        c.SwaggerEndpoint("/swagger/internal/swagger.json", "Transport Internal APIs");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseResponseCompression();
app.UseRateLimiter();
app.UseCorrelationId();
app.UseAuthentication();
app.UseMiddleware<InternalApiMiddleware>();
app.UseGatewayContext();
app.UseAuthorization();

app.MapControllers();
app.MapTransportHealthChecks();

// ── Auto-Migrate ────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TransportDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
