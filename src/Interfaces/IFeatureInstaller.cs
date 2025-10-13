using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lotto.Interfaces;

internal interface IFeatureInstaller
{
    void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment);
}
