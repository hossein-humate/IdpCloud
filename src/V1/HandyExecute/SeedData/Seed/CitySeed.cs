using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entity.BaseInfo;
using Newtonsoft.Json;

namespace HandyExecute.SeedData.Seed
{
    public static class CitySeed
    {
        private static readonly IList<City> Cities;

        static CitySeed()
        {
            var filePathCity = string.Format("{0}{1}SeedData{1}JsonData{1}AllCities.json",
                Environment.CurrentDirectory, Path.DirectorySeparatorChar);
            var filePathCountry = string.Format("{0}{1}SeedData{1}JsonData{1}AllCountries.json",
                Environment.CurrentDirectory, Path.DirectorySeparatorChar);
            using var countryData = new StreamReader(filePathCountry);
            var countryJson = countryData.ReadToEnd();
            var countries = JsonConvert.DeserializeObject<IEnumerable<Country>>(countryJson);
            using var cityData = new StreamReader(filePathCity);
            var cityJson = cityData.ReadToEnd();
            var cities = JsonConvert.DeserializeObject<IEnumerable<CityDto>>(cityJson);
            Cities = new List<City>();
            int counter = 1;
            foreach (var city in cities)
            {
                Cities.Add(new City
                {
                    CityId = counter,
                    Name = city.Name,
                    Latitude = city.Lat,
                    Longitude = city.Lng,
                    CountryId = countries.FirstOrDefault(c => c.TwoCharacterCode == city.Country).CountryId,
                });
                counter++;
            }
        }

        public static IList<City> Get()
        {
            return Cities;
        }
    }
    public class CityDto
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}
