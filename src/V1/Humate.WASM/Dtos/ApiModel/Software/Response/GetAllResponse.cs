using System.Collections.Generic;

namespace Humate.WASM.Dtos.ApiModel.Software.Response
{
    public class GetAllResponse:BaseResponse
    {
        public IEnumerable<SoftwareApiModel> Softwares { get; set; }
    }
}
