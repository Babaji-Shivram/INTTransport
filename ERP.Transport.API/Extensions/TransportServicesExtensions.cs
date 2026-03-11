using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Application.Mapping;
using ERP.Transport.Application.Services;
using ERP.Transport.Infrastructure.Data;
using ERP.Transport.Infrastructure.Repositories;
using EPR.Shared.Contracts.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace ERP.Transport.API.Extensions;

/// <summary>
/// DI registration — DbContext, AutoMapper, FluentValidation, Scrutor scan, Repository, UnitOfWork.
/// Mirrors CRM's CrmServicesExtensions.
/// </summary>
public static class TransportServicesExtensions
{
    public static IServiceCollection AddTransportServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // ── DbContext ──────────────────────────────────────────
        services.AddHttpContextAccessor(); // Required for DataScopeService → UserContext
        services.AddDbContext<TransportDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(TransportDbContext).Assembly.FullName));

            // SharedLibrary: audit interceptor
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

            options.AddAuditInterceptor(
                currentUserIdProvider: () =>
                {
                    var userId = httpContextAccessor.HttpContext?.GetUserContext()?.UserId;
                    return Guid.TryParse(userId, out var guid) ? guid : null;
                },
                currentUserEmailProvider: () => httpContextAccessor.HttpContext?.GetUserContext()?.Email,
                correlationIdProvider: () => httpContextAccessor.HttpContext?.GetCorrelationId(),
                ipAddressProvider: () => httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
            );
        });

        // ── AutoMapper ─────────────────────────────────────────
        services.AddAutoMapper(typeof(TransportMappingProfile).Assembly);

        // ── FluentValidation ───────────────────────────────────
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<TransportMappingProfile>();

        // ── Scrutor auto-scan (Service + Repository) ───────────
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(TransportJobService), typeof(Repository<>))
            .AddClasses(classes => classes.Where(type =>
                type.Name.EndsWith("Service") || type.Name.EndsWith("Repository")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // ── Generic Repository & UnitOfWork ────────────────────
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
