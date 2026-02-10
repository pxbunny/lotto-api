namespace Lotto.Features.Http;

internal sealed record DrawResultsDto(
    string DrawDate,
    IEnumerable<int> LottoNumbers,
    IEnumerable<int> PlusNumbers);

internal sealed record ErrorResponse(
    string Error);
