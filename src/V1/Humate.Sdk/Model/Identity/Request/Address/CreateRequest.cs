using System;

namespace Humate.Sdk.Model.Identity.Request.Address
{
    public class CreateRequest
    {
        public Guid UserId { get; set; }

        public string HomeNumber { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public int CityId { get; set; }

        public string State { get; set; }

        public short CountryId { get; set; }

        public AddressType Type { get; set; }

        public string Full { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }
}
