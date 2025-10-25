using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Lotto.InfrastructureTests;

public sealed class TypesVisibilityTests
{
    [Fact]
    void AllTypes_Should_BeInternal()
    {
        var visibleTypes = GetNotGeneratedCode().Where(t => !IsInternal(t));
        Assert.Empty(visibleTypes);
    }

    [Fact]
    void AllNonAbstractTypes_Should_BeSealed()
    {
        var nonSealedTypes = GetNotGeneratedCode().Where(t => t is { IsAbstract: false, IsSealed: false });
        Assert.Empty(nonSealedTypes);
    }

    static IEnumerable<Type> GetNotGeneratedCode()
    {
        var assembly = typeof(IAssemblyMarker).Assembly;
        return assembly.GetTypes().Where(t => !IsGeneratedCode(t));
    }

    static bool IsGeneratedCode(MemberInfo t)
    {
        return t.IsDefined(typeof(CompilerGeneratedAttribute), false) ||
               t.IsDefined(typeof(GeneratedCodeAttribute), false) ||
               t.IsDefined(typeof(DebuggerNonUserCodeAttribute), false);
    }

    static bool IsInternal(Type t) => t is
    {
        IsVisible: false,
        IsPublic: false,
        IsNotPublic: true,
        IsNested: false,
        IsNestedPublic: false,
        IsNestedFamily: false,
        IsNestedPrivate: false,
        IsNestedAssembly: false,
        IsNestedFamORAssem: false,
        IsNestedFamANDAssem: false
    };
}
