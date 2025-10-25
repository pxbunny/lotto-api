using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lotto.Interfaces;

interface IFeatureInstaller
{
    void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment);
}
