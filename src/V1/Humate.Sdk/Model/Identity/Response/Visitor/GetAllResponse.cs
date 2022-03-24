using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Response.Visitor
{
    public class GetAllResponse : BaseResponse
    {
        public IEnumerable<Identity.Visitor> Visitors { get; set; }
    }
}
