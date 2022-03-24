using System;
using Humate.WASM.Dtos.ApiModel.Software;

namespace Humate.WASM.Dtos.ApiModel.Role
{
    public class RoleApiModel
    {
        public Guid RoleId { get; set; }

        public Guid SoftwareId { get; set; }

        public SoftwareApiModel Software { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public long CreateDate { get; set; }
    }
}
