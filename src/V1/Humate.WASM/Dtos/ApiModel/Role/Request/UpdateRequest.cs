using System;

namespace Humate.WASM.Dtos.ApiModel.Role.Request
{
    public class UpdateRequest
    {
        public Guid RoleId { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}
