namespace Lotto.InfrastructureTests;

public sealed class FeatureInstallersTests
{
    [Fact]
    void MainInstaller_Should_FindFeatureInstallers()
    {
        var installerTypes = GetFeatureInstallerTypes();
        Assert.NotEmpty(installerTypes);
    }

    [Fact]
    void AllFeatureInstallers_Should_HavePublicParameterlessConstructor()
    {
        var installerTypes = GetFeatureInstallerTypes().ToList();

        Assert.NotEmpty(installerTypes);
        Assert.All(installerTypes, t => Assert.NotNull(t.GetConstructor(Type.EmptyTypes)));
    }

    static IEnumerable<Type> GetFeatureInstallerTypes()
    {
        var assembly = typeof(IAssemblyMarker).Assembly;
        return Installers.GetFeatureInstallerTypes(assembly);
    }
}
