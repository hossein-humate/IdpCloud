using System;

namespace Humate.Sdk.Model.Identity.Response.Software
{
    public class GenerateNewApiKeyResponse : BaseResponse
    {
        public string ApiKey { get; set; }
        public DateTime KeyExpireDate { get; set; }
    }
}
