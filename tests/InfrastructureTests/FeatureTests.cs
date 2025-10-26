namespace Lotto.InfrastructureTests;

public sealed class FeatureTests
{
    const string FeaturesNamespace = "Lotto.Features";

    [Fact]
    void FeatureTypes_ShouldNot_BePlacedInTheRootFeatureNamespace()
    {
        var featureTypes = GetFeatureTypes();
        var rootFeatreNamespaceDepth = FeaturesNamespace.Split(".").Length;
        Assert.All(featureTypes, t => Assert.True(t.Namespace!.Split(".").Length > rootFeatreNamespaceDepth));
    }

    static IEnumerable<Type> GetFeatureTypes()
    {
        var assembly = typeof(IAssemblyMarker).Assembly;
        return assembly.DefinedTypes.Where(t =>
            t.Namespace is not null && t.Namespace.StartsWith($"{FeaturesNamespace}."));
    }

    static IEnumerable<IGrouping<string, Type>> GetFeatureTypesGroupedByFeatureName() =>
        GetFeatureTypes().GroupBy(t => t.Namespace!.Split(".")[2]);
}
