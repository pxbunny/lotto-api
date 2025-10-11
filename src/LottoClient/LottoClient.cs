using System.Net.Http.Json;

namespace Lotto.LottoClient;

internal sealed class LottoClient(HttpClient client) : ILottoClient
{
    public async Task<DrawResults> GetLatestDrawResultsAsync(CancellationToken cancellationToken)
    {
        const string uri = "open/v1/lotteries/draw-results/last-results-per-game?gameType=Lotto";
        var response = (await client.GetFromJsonAsync<IEnumerable<LottoDrawResultsResponse>>(uri, cancellationToken) ?? []).ToList();

        if (response.Count == 0) throw new HttpRequestException("Couldn't retrieve data from API.");

        var lottoNumbers = response.First(r => r.GameType == "Lotto").Results.First().ResultsJson.ToList();
        var plusNumbers = (response.FirstOrDefault(r => r.GameType == "LottoPlus")?.Results.First().ResultsJson ?? []).ToList();

        return new DrawResults
        {
            DrawDate = response.First().DrawDate.ToString(Constants.DateFormat),
            LottoNumbers = lottoNumbers,
            PlusNumbers = plusNumbers,
            LottoNumbersString = string.Join(",", lottoNumbers),
            PlusNumbersString = string.Join(",", plusNumbers)
        };
    }
}
