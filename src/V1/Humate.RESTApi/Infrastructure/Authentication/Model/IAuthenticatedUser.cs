using System.Collections.Generic;
using Entity.Identity;
using Entity.SSO;

namespace Humate.RESTApi.Infrastructure.Authentication.Model
{
    public interface IAuthenticatedUser
    {
        Software Software { get; set; }

        User User { get; set; }

        UserSession UserSession { get; set; }

        Permission Permission { get; set; }

        IEnumerable<Software> AccessSoftwares { get; set; }

        IEnumerable<Role> AccessRoles { get; set; }
    }
}