using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.BaseInfo.Response.Country
{
    public class GetAllResponse : BaseResponse
    {
        public IEnumerable<BaseInfo.Country> Countries { get; set; }
    }
}
