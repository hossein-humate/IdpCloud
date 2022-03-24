using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Response.Address
{
    public class GetByUserIdResponse : BaseResponse
    {
        public IEnumerable<Identity.Address> Addresses { get; set; }
    }
}
