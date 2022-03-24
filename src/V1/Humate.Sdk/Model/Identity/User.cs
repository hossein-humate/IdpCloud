using System;

namespace Humate.Sdk.Model.Identity
{
    public class User
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public UserStatus Status { get; set; }

        public string Description { get; set; }

        public string RegisterIp { get; set; }

        public bool TwoFactorEnable { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool MobileConfirmed { get; set; }

        public string Picture { get; set; }

        public string LastLoginIp { get; set; }

        public DateTime? RegisterDate { get; set; }

        public int LoginTimes { get; set; }

        public DateTime? LastLoginDate { get; set; }

        //public LanguageApiModel Language { get; set; }

        public Person Person { get; set; }
    }

    public enum UserStatus
    {
        DeActive = 0,
        Active = 1
    }
}
