using Entity.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataProvider.Seed
{
    public static class SoftwareSeed
    {
        private static readonly IEnumerable<Software> Softwares;

        static SoftwareSeed()
        {
            var filePath = string.Format("{0}{1}wwwroot{1}jsonData{1}AllSoftwares.json",
                Environment.CurrentDirectory, Path.DirectorySeparatorChar);
            using var data = new StreamReader(filePath);
            var json = data.ReadToEnd();
            Softwares = JsonConvert.DeserializeObject<IEnumerable<Software>>(json);
        }

        public static IEnumerable<Software> Get()
        {
            return Softwares;
        }
    }
}
