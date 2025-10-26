namespace Lotto.InfrastructureTests;

public sealed class FeatureInstallersTests
{
    const string FeaturesNamespace = "Lotto.Features";

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

    [Fact]
    void AllFeatureInstallers_Should_HaveCorrectNamespace()
    {
        var installerTypes = GetFeatureInstallerTypes().ToList();
        Assert.All(installerTypes, t => Assert.StartsWith($"{FeaturesNamespace}.", t.Namespace));
    }

    [Fact]
    void AllFeatureInstallers_Should_HaveExpectedNamespaceDepthLevel()
    {
        const int expectedNamespaceDepthLevel = 3;
        var installerTypes = GetFeatureInstallerTypes().ToList();
        var namespaceParts = installerTypes.Select(t => t.Namespace?.Split('.')).ToList();
        Assert.All(namespaceParts, t => Assert.Equal(t?.Length, expectedNamespaceDepthLevel));
    }

    [Fact]
    void EachFeature_Should_HaveOneInstaller()
    {
        var installerTypes = GetFeatureInstallerTypes().ToList();
        var namespaces = installerTypes.Select(t => t.Namespace).ToList();
        Assert.Equal(namespaces.Count, namespaces.Distinct().Count());
    }

    static IEnumerable<Type> GetFeatureInstallerTypes()
    {
        var assembly = typeof(IAssemblyMarker).Assembly;
        return Installers.GetFeatureInstallerTypes(assembly);
    }
}
