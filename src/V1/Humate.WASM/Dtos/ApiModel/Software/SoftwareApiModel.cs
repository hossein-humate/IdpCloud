using System;

namespace Humate.WASM.Dtos.ApiModel.Software
{
    public class SoftwareApiModel
    {
        public Guid SoftwareId { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string BusinessDescription { get; set; }

        public string LogoImage { get; set; }

        public string ApiKey { get; set; }

        public long KeyExpire { get; set; }

        public Guid? OwnerUserId { get; set; }

        public Status Status { get; set; }
    }

    public enum Status
    {
        Active,
        Deactive
    }
}
