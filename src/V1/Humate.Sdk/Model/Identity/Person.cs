using System;
using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity
{
    public class Person
    {
        public Guid PersonId { get; set; }
        
        public Software Software { get; set; }

        public string Firstname { get; set; }

        public string Middlename { get; set; }

        public string Lastname { get; set; }

        //public CountryApiModel CountryLiving { get; set; }

        //public CountryApiModel Nationality { get; set; }

        public string PicturePath { get; set; }

        public GenderType Gender { get; set; }

        public List<User> Users { get; set; }
    }

    public enum GenderType : byte
    {
        Male,
        Female,
        NotInterest
    }
}
