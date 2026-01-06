namespace Lotto.InfrastructureTests;

public sealed class FeatureInstallersTests
{
    private const string FeaturesNamespace = "Lotto.Features";

    [Fact]
    private void MainInstaller_Should_FindFeatureInstallers()
    {
        var installerTypes = GetFeatureInstallerTypes();
        Assert.NotEmpty(installerTypes);
    }

    [Fact]
    private void AllFeatureInstallers_Should_HavePublicParameterlessConstructor()
    {
        var installerTypes = GetFeatureInstallerTypes().ToList();
        Assert.NotEmpty(installerTypes);
        Assert.All(installerTypes, t => Assert.NotNull(t.GetConstructor(Type.EmptyTypes)));
    }

    [Fact]
    private void AllFeatureInstallers_Should_HaveCorrectNamespace()
    {
        var installerTypes = GetFeatureInstallerTypes().ToList();
        Assert.All(installerTypes, t => Assert.StartsWith($"{FeaturesNamespace}.", t.Namespace));
    }

    [Fact]
    private void AllFeatureInstallers_Should_HaveExpectedNamespaceDepthLevel()
    {
        const int expectedNamespaceDepthLevel = 4;
        var installerTypes = GetFeatureInstallerTypes().ToList();
        var namespaceParts = installerTypes.Select(t => t.Namespace?.Split('.')).ToList();
        Assert.All(namespaceParts, t => Assert.Equal(t?.Length, expectedNamespaceDepthLevel));
    }

    [Fact]
    private void EachFeature_Should_HaveOneInstaller()
    {
        var installerTypes = GetFeatureInstallerTypes().ToList();
        var namespaces = installerTypes.Select(t => t.Namespace).ToList();
        Assert.Equal(namespaces.Count, namespaces.Distinct().Count());
    }

    private static IEnumerable<Type> GetFeatureInstallerTypes()
    {
        var assembly = typeof(IAssemblyMarker).Assembly;
        return Installers.GetFeatureInstallerTypes(assembly);
    }
}
