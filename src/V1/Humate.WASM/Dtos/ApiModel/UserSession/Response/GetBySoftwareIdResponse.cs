using System.Collections.Generic;

namespace Humate.WASM.Dtos.ApiModel.UserSession.Response
{
    public class GetBySoftwareIdResponse : BaseResponse
    {
        public IEnumerable<UserSessionApiModel> UserSessions { get; set; }
    }
}
