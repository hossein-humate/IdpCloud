using System.Collections.Generic;

namespace Humate.WASM.Dtos.ApiModel.Permission.Response
{
    public class GetBySoftwareIdResponse : BaseResponse
    {
        public IEnumerable<PermissionApiModel> Permissions { get; set; }
    }
}
