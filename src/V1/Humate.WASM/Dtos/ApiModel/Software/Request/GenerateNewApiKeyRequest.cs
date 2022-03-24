using System;

namespace Humate.WASM.Dtos.ApiModel.Software.Request
{
    public class GenerateNewApiKeyRequest
    {
        public DateTime? ExpireDate { get; set; }
        public Guid SoftwareId { get; set; }
    }
}
