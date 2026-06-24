using Application.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NArchitecture.Core.Persistence.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories;

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BaseDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("BaseDb")));
        services.AddDbMigrationApplier(buildServices => buildServices.GetRequiredService<BaseDbContext>());

        services.AddScoped<IEmailAuthenticatorRepository, EmailAuthenticatorRepository>();
        services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
        services.AddScoped<IOtpAuthenticatorRepository, OtpAuthenticatorRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserOperationClaimRepository, UserOperationClaimRepository>();

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IDrawingRepository, DrawingRepository>();
        services.AddScoped<IMetrajRuleTemplateRepository, MetrajRuleTemplateRepository>();
        services.AddScoped<IMetrajResultRepository, MetrajResultRepository>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();
        services.AddScoped<IPuantajRecordRepository, PuantajRecordRepository>();
        services.AddScoped<IContractItemRepository, ContractItemRepository>();
        services.AddScoped<IHakedisPeriodRepository, HakedisPeriodRepository>();
        services.AddScoped<IProgressEntryRepository, ProgressEntryRepository>();
        services.AddScoped<IHakedisDeductionLineRepository, HakedisDeductionLineRepository>();
        services.AddScoped<IProjectMetrajLayerMappingRepository, ProjectMetrajLayerMappingRepository>();
        return services;
    }
}
