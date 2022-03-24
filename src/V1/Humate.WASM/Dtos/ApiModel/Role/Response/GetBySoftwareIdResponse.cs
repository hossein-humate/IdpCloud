using System.Collections.Generic;

namespace Humate.WASM.Dtos.ApiModel.Role.Response
{
    public class GetBySoftwareIdResponse : BaseResponse
    {
        public IEnumerable<RoleApiModel> Roles { get; set; }
    }
}
