using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.Identity.Response.Permission
{
    public class GetBySoftwareIdResponse: BaseResponse
    {
        public IEnumerable<Identity.Permission> Permissions { get; set; }
    }
}
