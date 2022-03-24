using System;

namespace Humate.WASM.Dtos.ApiModel.Software.Response
{
    public class GenerateNewApiKeyResponse : BaseResponse
    {
        public string ApiKey { get; set; }
        public DateTime KeyExpireDate { get; set; }
    }
}
