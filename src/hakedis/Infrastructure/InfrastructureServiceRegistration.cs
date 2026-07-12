using Application.Services.ImageService;
using Application.Services.MetrajCalculation;
using Application.Services.MetrajJudgment;
using Infrastructure.Adapters.Anthropic;
using Infrastructure.Adapters.ImageService;
using Infrastructure.Metraj;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ImageServiceBase, CloudinaryImageServiceAdapter>();
        services.AddScoped<IMetrajCalculationService, NetDxfMetrajCalculationService>();

        services.Configure<AnthropicSettings>(configuration.GetSection(AnthropicSettings.SectionName));
        services.AddHttpClient<IMetrajJudgmentService, ClaudeMetrajJudgmentService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(120);
        });

        return services;
    }
}
