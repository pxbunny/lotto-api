namespace Lotto.Interfaces;

internal interface IRowKeyGenerator
{
    string GenerateRowKey(DateTime date);
}
