namespace Lotto.Interfaces;

interface IRowKeyGenerator
{
    string GenerateRowKey(DateTime date);
}
