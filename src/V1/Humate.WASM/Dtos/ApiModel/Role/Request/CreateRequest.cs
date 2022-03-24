using System;

namespace Humate.WASM.Dtos.ApiModel.Role.Request
{
    public class CreateRequest
    {
        public Guid SoftwareId { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}
