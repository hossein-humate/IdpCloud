using Humate.WASM.Dtos.ApiModel.Language;
using System;

namespace Humate.WASM.Dtos.ApiModel.User
{
    public class UserApiModel
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public ActiveStatus Status { get; set; }

        public string Description { get; set; }

        public string RegisterIp { get; set; }

        public bool TwoFactorEnable { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool MobileConfirmed { get; set; }

        public string Picture { get; set; }

        public string LastLoginIp { get; set; }

        public long? RegisterDate { get; set; }

        public int LoginTimes { get; set; }

        public long? LastLoginDate { get; set; }

        public LanguageApiModel Language { get; set; }
    }

    public enum ActiveStatus
    {
        DeActive = 0,
        Active = 1,
        PaymentInfoNotProvided = 2
    }
}
