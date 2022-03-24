using System;

namespace IdpCloud.Sdk.Model.Identity.Request.Software
{
    public class UpdateRequest
    {
        public Guid  SoftwareId { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string BusinessDescription { get; set; }

        public byte[] LogoContent { get; set; }

        public string LogoName { get; set; }
    }
}
