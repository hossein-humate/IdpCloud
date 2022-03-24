using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdpCloud.ServiceProvider.Service
{
    /// <summary>
    /// JwtSecutiryHandler response
    /// </summary>
    public class JwtSecurityHandlerResponse
    {
        /// <summary>
        /// Security Token 
        /// </summary>
        public SecurityToken SecurityToken { get; set; }

        /// <summary>
        /// List of claims
        /// </summary>
        public IEnumerable<Claim> Claims { get; set; }
    }
}
