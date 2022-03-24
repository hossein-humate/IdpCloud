using Entity.BaseInfo;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Identity
{
    [Table("Addresses", Schema = "Identity")]
    public class Address : BaseEntity
    {
        public Address()
        {
            AddressId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid AddressId { get; set; }

        public string HomeNumber { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public int? CityId { get; set; }

        public City City { get; set; }

        public string State { get; set; }

        public short? CountryId { get; set; }

        public Country Country { get; set; }

        public AddressType Type { get; set; }

        public string Full { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public bool Confirmed { get; set; }

        public Guid? UserId { get; set; }

        public User User { get; set; }
    }

    public enum AddressType
    {
        Home,
        Work,
        Office,
        Company,
        BillTo,
        ShipTo,
        Primary,
        Other
    }
}
