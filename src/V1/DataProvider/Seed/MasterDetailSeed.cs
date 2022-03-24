using Entity.BaseInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataProvider.Seed
{
    public static class MasterDetailSeed
    {
        private static readonly IEnumerable<MasterDetail> MasterDetails;

        static MasterDetailSeed()
        {
            var filePath = string.Format("{0}{1}wwwroot{1}jsonData{1}AllMasterDetails.json",
                Environment.CurrentDirectory, Path.DirectorySeparatorChar);
            using var data = new StreamReader(filePath);
            var json = data.ReadToEnd();
            MasterDetails = JsonConvert.DeserializeObject<IEnumerable<MasterDetail>>(json);
        }

        public static IEnumerable<MasterDetail> Get()
        {
            return MasterDetails;
        }
    }
}
