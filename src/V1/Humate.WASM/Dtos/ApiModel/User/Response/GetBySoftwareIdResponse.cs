using System.Collections.Generic;

namespace Humate.WASM.Dtos.ApiModel.User.Response
{
    public class GetBySoftwareIdResponse : BaseResponse
    {
        public IEnumerable<UserApiModel> Users { get; set; }
    }
}