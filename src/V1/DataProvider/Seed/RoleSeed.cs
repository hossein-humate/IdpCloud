using Entity.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataProvider.Seed
{
    public static class RoleSeed
    {
        private static readonly IEnumerable<Role> Roles;

        static RoleSeed()
        {
            var filePath = string.Format("{0}{1}wwwroot{1}jsonData{1}AllRoles.json",
                Environment.CurrentDirectory, Path.DirectorySeparatorChar);
            using var data = new StreamReader(filePath);
            var json = data.ReadToEnd();
            Roles = JsonConvert.DeserializeObject<IEnumerable<Role>>(json);
        }

        public static IEnumerable<Role> Get()
        {
            return Roles;
        }
    }
}
