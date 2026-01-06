namespace Lotto.Features.Http.GetLatestDrawStatus;

internal sealed record LatestDrawStatusDto(string? LatestSyncedDate, string LatestDrawDate, bool IsUpToDate);
