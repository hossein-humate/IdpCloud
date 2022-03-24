using Entity.BaseInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace HandyExecute.SeedData.Seed
{
    public static class CountrySeed
    {
        private static readonly IEnumerable<Country> Countries;

        static CountrySeed()
        {
            var filePath = string.Format("{0}{1}SeedData{1}JsonData{1}AllCountries.json",
                Environment.CurrentDirectory, Path.DirectorySeparatorChar);
            using var data = new StreamReader(filePath);
            var json = data.ReadToEnd();
            Countries = JsonConvert.DeserializeObject<IEnumerable<Country>>(json);
        }

        public static IEnumerable<Country> Get()
        {
            return Countries;
        }
    }
}
