
namespace IdpCloud.Common
{
    /// <summary>
    /// The RandomPasswordGenerator creating random password
    /// </summary>
    public interface IRandomPasswordGenerator
    {
        /// <summary>
        /// Creates random password
        /// </summary>
        /// <returns>A string random password</returns>
        string RandomPassword();

    }
}
