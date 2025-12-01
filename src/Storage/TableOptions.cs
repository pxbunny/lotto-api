using Microsoft.Extensions.Configuration;

namespace Lotto.Storage;

internal sealed class TableOptions
{
    public const string SectionName = "TableNames";

    [ConfigurationKeyName("DrawResults")]
    public required string DrawResultsTableName { get; init; }
}
