using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyExecute
{
    public static class ReflectionHelper
    {
        public static string GetValue(object currentObject, string pathName)
        {
            if (pathName.Contains("."))
            {
                string[] fieldNames = pathName.Split(".");
                foreach (string fieldName in fieldNames)
                {
                    var typeofObj = currentObject.GetType();
                    var property1 = typeofObj.GetProperties();
                    var property = typeofObj.GetProperty(fieldName);
                    if (property != null)
                    {
                        currentObject = property.GetValue(currentObject, null);
                    }
                    else
                    {
                        throw new NullReferenceException(pathName + " Column not exist in this list");
                    }
                }
                return currentObject.ToString();
            }
            var col = currentObject.GetType().GetProperty(pathName);
            if (col == null)
                throw new NullReferenceException(pathName + " Column not exist in this list");
            return col.GetValue(currentObject, null)?.ToString() ?? string.Empty;
        }
    }

    public class User1
    {
        public string Username { get; set; }
        public Person PersonInfo { get; set; }
    }

    public class Person
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
