using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace Lotto;

internal static class Installers
{
    public static void RunFeatureInstallers(this IHostApplicationBuilder builder, Assembly? assembly = null)
    {
        var installerTypes = GetFeatureInstallerTypes(assembly);

        foreach (var type in installerTypes)
        {
            if (Activator.CreateInstance(type) is not IFeatureInstaller installer)
                continue;

            installer.Install(builder.Services, builder.Configuration, builder.Environment);
        }
    }

    public static IEnumerable<Type> GetFeatureInstallerTypes(Assembly? assembly = null)
    {
        return (assembly ?? Assembly.GetCallingAssembly()).GetTypes()
            .Where(t => typeof(IFeatureInstaller).IsAssignableFrom(t) &&
                        t is { IsAbstract: false, IsInterface: false });
    }
}
