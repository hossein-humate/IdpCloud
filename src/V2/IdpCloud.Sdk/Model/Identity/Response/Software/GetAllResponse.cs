using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.Identity.Response.Software
{
    public class GetAllResponse : BaseResponse
    {
        public IEnumerable<Identity.Software> Softwares { get; set; }
    }
}
