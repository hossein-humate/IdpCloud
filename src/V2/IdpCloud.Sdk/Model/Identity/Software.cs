using System;

namespace IdpCloud.Sdk.Model.Identity
{
    public class Software
    {
        public Guid SoftwareId { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string BusinessDescription { get; set; }

        public string LogoImage { get; set; }

        public string ApiKey { get; set; }

        public DateTime KeyExpire { get; set; }

        public Guid? OwnerUserId { get; set; }

        public SoftwareStatus Status { get; set; }
    }

    public enum SoftwareStatus
    {
        Active,
        Deactive
    }
}
