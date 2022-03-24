using System.Collections.Generic;
using System.Linq;

namespace Humate.Sdk.Model.Identity.Response.User
{
    public class GetAllResponse : BaseResponse
    {
        public IQueryable<SoftwareUserDto>  Result { get; set; }
    }

    public class SoftwareUserDto
    {
        public Identity.Software Software { get; set; }

        public IEnumerable<Identity.User> Users { get; set; }
    }
}
