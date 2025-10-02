namespace Lotto.Functions.Http.GetDrawResults;

internal sealed record GetDrawResultsQueryParams(DateOnly? DateFrom, DateOnly? DateTo, int? Top);
