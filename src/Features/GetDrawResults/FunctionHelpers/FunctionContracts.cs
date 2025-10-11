﻿namespace Lotto.Features.GetDrawResults.FunctionHelpers;

internal sealed record DrawResultsDto(string DrawDate, IEnumerable<int> LottoNumbers, IEnumerable<int> PlusNumbers);

internal sealed record DrawResultsCsvRecord(string DrawDate, string LottoNumbers, string? PlusNumbers);
