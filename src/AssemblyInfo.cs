using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Lotto.InfrastructureTests")]
[assembly: InternalsVisibleTo("Lotto.IntegrationTests")]
[assembly: InternalsVisibleTo("Lotto.UnitTests")]

namespace Lotto;

internal interface IAssemblyMarker;
