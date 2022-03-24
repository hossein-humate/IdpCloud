using System;
using System.Collections.Generic;

namespace HandyExecute
{
    public class NationalCode
    {
        public void ShowNationalCode(int count = 10)
        {
            var nationalCodeGenerator = new NationalCode();
            var result = nationalCodeGenerator.GenerateNationalCode(10);
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }

        public List<string> GenerateNationalCode(int count = 2)
        {
            var result = new List<string>();
            while (result.Count != count)
            {
                var array = new int[10];
                var sumIndexes = 0;
                for (int i = 1; i <= 9; i++)
                {
                    array[i] = new Random().Next(0, 9);
                    sumIndexes += array[i] * (i+1);
                }

                var mod = sumIndexes % 11;
                if (mod < 2)
                {
                    array[0] = 0;
                    result.Add(string.Join("", array));
                    array[0] = 1;
                    result.Add(string.Join("", array));
                }
                else
                {
                    array[0] = 11 - mod;
                    result.Add(string.Join("", array));
                }
            }

            return result;
        }
    }
}
