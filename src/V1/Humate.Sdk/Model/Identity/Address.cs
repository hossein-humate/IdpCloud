using System;

namespace Humate.Sdk.Model.Identity
{
    public class Address
    {
        public Guid AddressId { get; set; }

        public string HomeNumber { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string State { get; set; }
        
        public AddressType Type { get; set; }

        public string Full { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public bool Confirmed { get; set; }

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
