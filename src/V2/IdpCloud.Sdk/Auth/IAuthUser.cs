using IdpCloud.Sdk.Model.Identity;
using IdpCloud.Sdk.Model.SSO;

namespace IdpCloud.Sdk.Auth
{
    public interface IAuthUser
    {
        User User { get; set; }
        UserSession CurrentSession { get; set; }
        Role Role { get; set; }
    }
}
