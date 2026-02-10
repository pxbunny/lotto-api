namespace Lotto.Features.Http.GetSync;

internal sealed record SyncDto(
    string? LatestSyncDate,
    string LatestDrawDate,
    bool IsUpToDate);
