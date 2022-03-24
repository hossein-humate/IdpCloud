using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.Identity.Response.User
{
    public class GetBySoftwareIdResponse:BaseResponse
    {
        public IEnumerable<Identity.User> Users { get; set; }
    }
}
