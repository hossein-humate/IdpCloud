using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Response.Permission
{
    public class GetBySoftwareIdResponse : BaseResponse
    {
        public IEnumerable<Identity.Permission> Permissions { get; set; }
    }
}
