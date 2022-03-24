using Entity.BaseInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataProvider.Seed
{
    public static class CurrencySeed
    {
        private static readonly IEnumerable<Currency> Currencies;

        static CurrencySeed()
        {
            var filePath = string.Format("{0}{1}wwwroot{1}jsonData{1}AllCurrencies.json",
                Environment.CurrentDirectory, Path.DirectorySeparatorChar);
            using var data = new StreamReader(filePath);
            var json = data.ReadToEnd();
            Currencies = JsonConvert.DeserializeObject<IEnumerable<Currency>>(json);
        }

        public static IEnumerable<Currency> Get()
        {
            return Currencies;
        }
    }
}
