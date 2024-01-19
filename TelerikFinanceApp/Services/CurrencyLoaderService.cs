using Contracts;
using NbrbAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services
{
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
    }
}
