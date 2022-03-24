using Entity.BaseInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace HandyExecute.SeedData.Seed
{
    public static class LanguageSeed
    {
        private static readonly IEnumerable<Language> Languages;

        static LanguageSeed()
        {
            var filePath = string.Format("{0}{1}SeedData{1}JsonData{1}AllLanguages.json",
                Environment.CurrentDirectory, Path.DirectorySeparatorChar);
            using var data = new StreamReader(filePath);
            var json = data.ReadToEnd();
            Languages = JsonConvert.DeserializeObject<IEnumerable<Language>>(json);
        }

        public static IEnumerable<Language> Get()
        {
            return Languages;
        }
    }
}
