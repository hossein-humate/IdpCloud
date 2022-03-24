using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Response.UserRole
{
    public class GetByUserIdResponse : BaseResponse
    {
        public IEnumerable<GetByRoleId> UserRoles { get; set; }
    }
}
