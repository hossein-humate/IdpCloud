using AutoMapper;
using EntityServiceProvider;
using General;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Infrastructure.Controller
{
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected readonly IConfiguration Configuration;
        protected readonly IMapper Mapper;
        protected readonly IAuthenticatedUser AuthenticatedUser;
        public ApiBaseController()
        {

        }

        public ApiBaseController(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper,
            IAuthenticatedUser authenticatedUser)
        {
            UnitOfWork = unitOfWork;
            Configuration = configuration;
            Mapper = mapper;
            AuthenticatedUser = authenticatedUser;
        }

        protected IUnitOfWork UnitOfWork { get; }

        #region Methods
        [NonAction]
        public async Task<BaseResponse> CheckSoftwareExistAsync(Guid softwareId,
            CancellationToken cancellationToken = default)
        {
            if (softwareId == default)
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.RequiredSoftwareId);
            }

            if (!await UnitOfWork.Softwares.AnyAsNoTrackingAsync(s => s.SoftwareId == softwareId,
                cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.InvalidSoftwareId);
            }
            if (softwareId.Equals(Configuration["Application:SoftwareId"].ToGuid())
                && AuthenticatedUser.AccessRoles.All(ar =>
                    ar.RoleId != Configuration["Application:AdministratorRoleId"].ToGuid()))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.NotAllowedThisOperation);
            }
            return null;
        }

        [NonAction]
        public async Task<BaseResponse> CheckUserExistAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (userId == default)
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.FillRequiredParameter);
            }

            if (!await UnitOfWork.Users.AnyAsNoTrackingAsync(s => s.UserId == userId,
                cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.InvalidUserId);
            }
            return null;
        }

        [NonAction]
        public async Task<BaseResponse> CheckRoleExistAsync(Guid roleId,
            CancellationToken cancellationToken = default)
        {
            if (roleId == default)
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.FillRequiredParameter);
            }

            if (!await UnitOfWork.Roles.AnyAsNoTrackingAsync(s => s.RoleId == roleId,
                cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.InvalidDataEntries);
            }
            if (roleId.Equals(Configuration["Application:AdministratorRoleId"].ToGuid())
                && AuthenticatedUser.AccessRoles.All(ar =>
                    ar.RoleId != Configuration["Application:AdministratorRoleId"].ToGuid()))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.NotAllowedThisOperation);
            }
            return null;
        }

        [NonAction]
        public async Task<BaseResponse> CheckPermissionExistAsync(Guid permissionId,
            CancellationToken cancellationToken = default)
        {
            if (permissionId == default)
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.FillRequiredParameter);
            }

            if (!await UnitOfWork.Permissions.AnyAsNoTrackingAsync(s => s.PermissionId == permissionId,
                cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.InvalidDataEntries);
            }

            return null;
        }

        [NonAction]
        public async Task<BaseResponse> CheckMasterDetailExistAsync(Guid masterDetailId,
            CancellationToken cancellationToken = default)
        {
            if (!await UnitOfWork.MasterDetails.AnyAsNoTrackingAsync(m =>
                m.MasterDetailId == masterDetailId, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.InvalidMasterDetailId);
            }
            return null;
        }

        [NonAction]
        public async Task<BaseResponse> CheckCityExistAsync(int cityId,
            CancellationToken cancellationToken = default)
        {
            if (cityId == default)
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.RequiredCityId);
            }
            if (!await UnitOfWork.Cities.AnyAsNoTrackingAsync(m =>
                m.CityId == cityId, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.InvalidCityId);
            }
            return null;
        }

        [NonAction]
        public async Task<BaseResponse> CheckCountryExistAsync(short countryId,
            CancellationToken cancellationToken = default)
        {
            if (countryId == default)
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.RequiredCountryId);
            }
            if (!await UnitOfWork.Countries.AnyAsNoTrackingAsync(m =>
                m.CountryId == countryId, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.InvalidCountryId);
            }
            return null;
        }

        [NonAction]
        public async Task<BaseResponse> CheckAddressExistAsync(Guid addressId,
            CancellationToken cancellationToken = default)
        {
            if (addressId == default)
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.RequiredAddressId);
            }
            if (!await UnitOfWork.Addresses.AnyAsNoTrackingAsync(m =>
                m.AddressId == addressId, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.InvalidAddressId);
            }
            return null;
        }
        #endregion
    }
}
