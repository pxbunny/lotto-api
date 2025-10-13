using System.Reflection;
using Microsoft.Azure.Functions.Worker.Builder;

namespace Lotto;

internal static class Installers
{
    public static void RunFeatureInstallers(this FunctionsApplicationBuilder builder, Assembly? assembly = null)
    {
        var installerTypes = (assembly ?? Assembly.GetCallingAssembly()).GetTypes()
            .Where(t => typeof(IFeatureInstaller).IsAssignableFrom(t) &&
                        t is { IsAbstract: false, IsInterface: false });

        foreach (var type in installerTypes)
        {
            if (Activator.CreateInstance(type) is not IFeatureInstaller installer)
                continue;

            installer.Install(builder.Services, builder.Configuration, builder.Environment);
        }
    }
}
