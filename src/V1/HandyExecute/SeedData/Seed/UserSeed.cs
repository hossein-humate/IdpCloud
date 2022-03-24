using Entity.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace HandyExecute.SeedData.Seed
{
    public static class UserSeed
    {
        private static readonly IEnumerable<User> Users;

        static UserSeed()
        {
            var filePath = string.Format("{0}{1}SeedData{1}JsonData{1}AllUsers.json",
                Environment.CurrentDirectory, Path.DirectorySeparatorChar);
            using var data = new StreamReader(filePath);
            var json = data.ReadToEnd();
            Users = JsonConvert.DeserializeObject<IEnumerable<User>>(json);
        }

        public static IEnumerable<User> Get()
        {
            return Users;
        }
    }
}
