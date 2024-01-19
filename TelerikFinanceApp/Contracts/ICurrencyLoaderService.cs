﻿using NbrbAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICurrencyLoaderService
    {
        Task<List<Currency>> GetCurrenciesAsync();
        Task<List<Rate>> GetOfficialRatesAsync(int periodicity = 0);
    }
}