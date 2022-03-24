using IdpCloud.Sdk.Enum;
using Microsoft.AspNetCore.Mvc;

namespace IdpCloud.REST.Infrastructure.Attribute
{
    /// <summary>
    /// Check Role Attribute represent that the given role or roles, has enough authority to access this API
    /// and can execute this operation 
    /// </summary>
    public class CheckRoleAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initialise and instanse of <see cref="CheckRoleAttribute"/>
        /// </summary>
        /// <param name="ssoRoles">Represent array of roles those are valid to access this operation,
        /// If one of these roles exist in user privileges then it is safe to continue execute the operation</param>
        public CheckRoleAttribute(SsoRole[] ssoRoles) : base(typeof(CheckRoleFilter))
        {
            Arguments = new object[] { ssoRoles };
        }
    }
}
