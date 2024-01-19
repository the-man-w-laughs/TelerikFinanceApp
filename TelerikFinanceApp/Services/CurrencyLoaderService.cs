using Contracts;
using NbrbAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services
{
// This service provides methods to load currency-related data from the National Bank of the Republic of Belarus API.
public class CurrencyLoaderService : ICurrencyLoaderService
{
    private readonly HttpClient _httpClient;
    private const string CurrenciesApiUrl = "https://api.nbrb.by/exrates/currencies";
    private const string OfficialRatesApiUrl = "https://api.nbrb.by/exrates/rates";
    
    public CurrencyLoaderService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<Currency>> GetCurrenciesAsync()
    {
        string jsonResult = await _httpClient.GetStringAsync(CurrenciesApiUrl);
        return JsonConvert.DeserializeObject<List<Currency>>(jsonResult);
    }

    public async Task<List<Rate>> GetOfficialRatesAsync(int periodicity = 0)
    {
        string apiUrl = $"{OfficialRatesApiUrl}?periodicity={periodicity}";
        string jsonResult = await _httpClient.GetStringAsync(apiUrl);
        return JsonConvert.DeserializeObject<List<Rate>>(jsonResult);
    }

    public async Task<List<Rate>> GetRatesDynamicsAsync(int curId, DateTime startDate, DateTime endDate)
    {
        string apiUrl = $"https://api.nbrb.by/exrates/rates/dynamics/{curId}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        string jsonResult = await _httpClient.GetStringAsync(apiUrl);

        List<Rate> rates = JsonConvert.DeserializeObject<List<Rate>>(jsonResult);

        return rates;
    }
}

}
