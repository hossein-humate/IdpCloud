using System;
using Humate.WASM.Dtos.ApiModel.Software;
using Humate.WASM.Dtos.ApiModel.User;

namespace Humate.WASM.Dtos.ApiModel.UserSession
{
    public class UserSessionApiModel
    {
        public Guid UserSessionId { get; set; }

        public Status Status { get; set; }

        public AuthType AuthType { get; set; }

        public string Ip { get; set; }

        public string UserAgent { get; set; }

        public Guid? SoftwareId { get; set; }

        public SoftwareApiModel Software { get; set; }

        public Guid UserId { get; set; }

        public UserApiModel User { get; set; }
    }

    public enum Status : byte
    {
        DeActive = 0,
        Active = 1
    }

    public enum AuthType : byte
    {
        UserPass = 0,
        GoogleAccount = 1,
        OtpCode = 2,
        TrustedUsernameOrEmailProvider = 3
    }
}
