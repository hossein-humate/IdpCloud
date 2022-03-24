using System;

namespace Humate.Sdk.Model.SSO
{
    public class UserSession
    {
        public Guid UserSessionId { get; set; }

        public Status Status { get; set; }

        public AuthType AuthType { get; set; }

        public string Ip { get; set; }

        public string UserAgent { get; set; }

        public Guid? SoftwareId { get; set; }

        public Identity.Software Software { get; set; }

        public Guid UserId { get; set; }

        public Identity.User User { get; set; }
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
