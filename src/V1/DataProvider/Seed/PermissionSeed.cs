using Entity.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataProvider.Seed
{
    public static class PermissionSeed
    {
        private static readonly IEnumerable<Permission> Permissions;

        static PermissionSeed()
        {
            var filePath = string.Format("{0}{1}wwwroot{1}jsonData{1}AllPermissions.json",
                Environment.CurrentDirectory, Path.DirectorySeparatorChar);
            using var data = new StreamReader(filePath);
            var json = data.ReadToEnd();
            Permissions = JsonConvert.DeserializeObject<IEnumerable<Permission>>(json);
        }

        public static IEnumerable<Permission> Get()
        {
            return Permissions;
        }
    }
}
