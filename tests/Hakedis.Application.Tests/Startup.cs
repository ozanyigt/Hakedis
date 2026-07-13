using Microsoft.Extensions.DependencyInjection;
using StarterProject.Application.Tests.DependencyResolvers;

namespace Hakedis.Application.Tests;

public sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddUsersServices();
        services.AddAuthServices();
    }
}
