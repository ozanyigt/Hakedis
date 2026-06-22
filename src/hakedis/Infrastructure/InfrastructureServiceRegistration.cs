using Application.Services.ImageService;
using Application.Services.MetrajCalculation;
using Infrastructure.Adapters.ImageService;
using Infrastructure.Metraj;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ImageServiceBase, CloudinaryImageServiceAdapter>();
        services.AddScoped<IMetrajCalculationService, NetDxfMetrajCalculationService>();

        return services;
    }
}
