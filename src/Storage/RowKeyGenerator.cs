using System.Globalization;

namespace Lotto.Storage;

internal sealed class RowKeyGenerator : IRowKeyGenerator
{
    public string GenerateRowKey(DateTime date)
    {
        var dateDifference = DateTime.MaxValue - date;
        var dateReversed = DateTime.MinValue + dateDifference;
        return dateReversed.ToString(
            Constants.DateFormat.Replace("-", "", StringComparison.InvariantCulture),
            CultureInfo.InvariantCulture);
    }
}
