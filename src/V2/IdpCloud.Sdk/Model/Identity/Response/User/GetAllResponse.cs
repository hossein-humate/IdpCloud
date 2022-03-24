using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.Identity.Response.User
{
    public class GetAllResponse:BaseResponse
    {
        public IEnumerable<Identity.User> Users { get; set; }
    }
}
