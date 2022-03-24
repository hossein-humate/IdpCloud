using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.SSO.Response.UserSoftware
{
    /// <summary>
    /// Response model for Controller API GET: /sso/userSoftware/SoftwareList
    /// </summary>
    public class SoftwareListResponse : BaseResponse
    {
        /// <summary>
        /// Represent list of Software for specific user
        /// </summary>
        public IEnumerable<SoftwareDto> Softwares { get; set; }
    }
}
