using System;
using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.Identity.Response.UserRole
{
    public class GetAssignedByRoleIdResponse : BaseResponse
    {
        public IEnumerable<AssignmentUserRole> AssignmentUserRoles { get; set; }
        
    }
    public class AssignmentUserRole
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public bool Assigned { get; set; }
        public string Status { get; set; }
        public DateTime? AssignmentDate { get; set; }
    }

}
