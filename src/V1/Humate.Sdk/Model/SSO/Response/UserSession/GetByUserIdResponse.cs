using System.Collections.Generic;

namespace Humate.Sdk.Model.SSO.Response.UserSession
{
    public class GetByUserIdResponse : BaseResponse
    {
        public IEnumerable<SSO.UserSession> UserSessions { get; set; }
    }
}
