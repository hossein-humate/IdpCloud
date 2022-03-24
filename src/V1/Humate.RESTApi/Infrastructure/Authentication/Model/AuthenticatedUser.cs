using System.Collections.Generic;
using Entity.Identity;
using Entity.SSO;

namespace Humate.RESTApi.Infrastructure.Authentication.Model
{
    public class AuthenticatedUser : IAuthenticatedUser
    {
        public Software Software { get; set; }
        public User User { get; set; }
        public UserSession UserSession { get; set; }
        public Permission Permission { get; set; }
        public IEnumerable<Software> AccessSoftwares { get; set; }
        public IEnumerable<Role> AccessRoles { get; set; }
    }
}
