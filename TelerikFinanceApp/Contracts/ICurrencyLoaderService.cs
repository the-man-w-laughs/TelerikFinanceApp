using NbrbAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICurrencyLoaderService
    {
        // Gets a list of currencies asynchronously from the National Bank of the Republic of Belarus API.
        // Returns: A list of Currency objects representing different currencies.
        Task<List<Currency>> GetCurrenciesAsync();

        // Gets a list of official rates asynchronously from the National Bank of the Republic of Belarus API.
        // Parameters:
        //   periodicity: An optional parameter indicating the periodicity of the rates.
        // Returns: A list of Rate objects representing official rates.
        Task<List<Rate>> GetOfficialRatesAsync(int periodicity = 0);

        // Gets the dynamics of rates for a specific currency within a date range asynchronously.
        // Parameters:
        //   curId: The ID of the currency.
        //   startDate: The start date of the range.
        //   endDate: The end date of the range.
        // Returns: A list of Rate objects representing the rate dynamics.
        Task<List<Rate>> GetRatesDynamicsAsync(int curId, System.DateTime startDate, System.DateTime endDate);
    }
}