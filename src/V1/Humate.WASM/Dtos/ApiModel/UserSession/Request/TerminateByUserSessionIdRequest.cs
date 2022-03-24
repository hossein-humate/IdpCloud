using System;

namespace Humate.WASM.Dtos.ApiModel.UserSession.Request
{
    public class TerminateByUserSessionIdRequest
    {
        public Guid UserSessionId { get; set; }
    }
}
