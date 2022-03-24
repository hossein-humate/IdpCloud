using IdpCloud.DataProvider.Entity.SSO;

namespace IdpCloud.ServiceProvider.Service
{
    /// <summary>
    /// The current UserSession Service
    /// </summary>
    public interface ICurrentUserSessionService
    {
        /// <summary>
        /// The UserSession object with private setter.
        /// </summary>
        UserSession UserSession { get; }

        /// <summary>
        /// User is Administrator or not
        /// </summary>
        bool IsAdministrator { get; }
    }
}
