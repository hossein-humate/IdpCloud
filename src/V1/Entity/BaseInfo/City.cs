using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entity.Identity;

namespace Entity.BaseInfo
{
    [Table("Cities", Schema = "BaseInfo")]
    public class City : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CityId { get; set; }

        public string Name { get; set; }

        public short CountryId { get; set; }

        public Country Country { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public List<Address> Addresses { get; set; }
    }
}
