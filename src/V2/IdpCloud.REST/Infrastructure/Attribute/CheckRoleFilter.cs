using IdpCloud.Sdk.Enum;
using IdpCloud.Sdk.Model;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace IdpCloud.REST.Infrastructure.Attribute
{
    /// <summary>
    /// Action Filter to check current authenticated user's roles to continue operation
    /// </summary>
    public class CheckRoleFilter : IAsyncActionFilter
    {
        private readonly ICurrentUserSessionService _currentUserSession;
        private readonly SsoRole[] _roles;

        /// <summary>
        /// Initialize an instanse of <see cref="CheckRoleFilter"/>
        /// </summary>
        /// <param name="currentUserSession"></param>
        public CheckRoleFilter(SsoRole[] ssoRoles, ICurrentUserSessionService currentUserSession)
        {
            _roles = ssoRoles;
            _currentUserSession = currentUserSession;
        }

        /// <Inheritdoc />
        public async Task OnActionExecutionAsync(
          ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var hasAccess = false;
            foreach (var role in _roles)
            {
                if (_currentUserSession.UserSession.User
                    .UserRoles.Any(ur => ur.Role.Name == role.ToString()))
                {
                    hasAccess = true;
                    break;
                }
            }

            if (hasAccess)
            {
                await next();
            }
            else
            {
                context.Result = new ObjectResult(BaseResponseCollection
                        .GetBaseResponse(RequestResult.NotAllowedThisOperation))
                {
                    StatusCode = 403
                };
            }
        }
    }
}
