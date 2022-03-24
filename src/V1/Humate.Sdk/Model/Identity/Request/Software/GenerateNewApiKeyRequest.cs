using System;

namespace Humate.Sdk.Model.Identity.Request.Software
{
    public class GenerateNewApiKeyRequest
    {
        public DateTime? ExpireDate { get; set; }
        public Guid SoftwareId { get; set; } 
    }
}
