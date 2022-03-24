using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Response.Role
{
    public class GetBySoftwareIdResponse:BaseResponse
    {
        public IEnumerable<Identity.Role> Roles { get; set; }
    }
}
