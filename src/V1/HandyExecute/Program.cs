using System;
using HandyExecute.SeedData;

namespace HandyExecute
{
    internal class Program
    {
        private static void Main()
        {
            //RunSeed.Run();
            new NationalCode().ShowNationalCode();
            //var obj = new User1
            //{
            //    Username = "Humate",
            //    PersonInfo = new Person
            //    {
            //        Firstname = "Hossein",
            //        Lastname = "Bagheri"
            //    }
            //};
            //var result = ReflectionHelper.GetValue(obj, "PersonInfo.Firstname");
            Console.ReadLine();
        }
    }
}