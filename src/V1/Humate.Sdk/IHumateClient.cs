using Humate.Sdk.Model;
using Humate.Sdk.Model.Identity.Response.Permission;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.Sdk
{
    public interface IHumateClient
    {
        #region BaseInfo

        #region City

        Task<Model.BaseInfo.Response.City.GetByCountryIdResponse> CityGetByCountryIdAsync(string token,
            short countryId, CancellationToken cancellationToken = default);
        #endregion

        #region Country

        Task<Model.BaseInfo.Response.Country.GetAllResponse> CountryGetAllAsync(string token,
            CancellationToken cancellationToken = default);
        #endregion

        #region Language

        Task<Model.BaseInfo.Response.Language.GetAllResponse> LanguageGetAllAsync(string token,
           CancellationToken cancellationToken = default);
        #endregion

        #region MasterDetail

        Task<Model.BaseInfo.Response.MasterDetail.GetAllMasterResponse> MasterDetailGetAllMasterAsync(
           string token,
           CancellationToken cancellationToken = default);

        Task<Model.BaseInfo.Response.MasterDetail.GetAllDetailResponse> MasterDetailGetAllDetailAsync(
           string token,
           Guid masterId, CancellationToken cancellationToken = default);

        Task<Model.BaseInfo.Response.MasterDetail.GetByIdResponse> MasterDetailGetByIdAsync(string token,
           Guid masterDetailId, CancellationToken cancellationToken = default);

        Task<BaseResponse> MasterDetailCreateMasterAsync(string token,
           Model.BaseInfo.Request.MasterDetail.CreateMasterRequest request,
           CancellationToken cancellationToken = default);

        Task<BaseResponse> MasterDetailUpdateAsync(string token,
           Model.BaseInfo.Request.MasterDetail.UpdateMasterDetailRequest request,
           CancellationToken cancellationToken = default);

        Task<BaseResponse> MasterDetailCreateDetailAsync(string token,
            Model.BaseInfo.Request.MasterDetail.CreateDetailRequest request,
            CancellationToken cancellationToken = default);

        Task<BaseResponse> MasterDetailDeleteAsync(string token,
            Guid masterDetailId, CancellationToken cancellationToken = default);
        #endregion

        #endregion

        #region Identity
        Task<BaseResponse> IsValidTokenAsync(string token,
            CancellationToken cancellationToken = default);

        #region Address

        Task<Model.Identity.Response.Address.GetByUserIdResponse> AddressGetByUserIdAsync(string token,
            Guid userId, CancellationToken cancellationToken = default);

        Task<BaseResponse> AddressCreateAsync(string token,
            Model.Identity.Request.Address.CreateRequest request,
            CancellationToken cancellationToken = default);

        Task<BaseResponse> AddressUpdateAsync(string token,
            Model.Identity.Request.Address.UpdateRequest request,
            CancellationToken cancellationToken = default);

        Task<BaseResponse> AddressDeleteAsync(string token,
            Guid addressId, CancellationToken cancellationToken = default);
        #endregion

        #region Permission
        Task<Model.Identity.Response.Permission.GetByIdResponse> PermissionGetByIdAsync(
            string token, string permissionId, CancellationToken cancellationToken = default);

        Task<Model.Identity.Response.Permission.GetBySoftwareIdResponse> PermissionGetBySoftwareIdAsync(
            string token,
            string softwareId, CancellationToken cancellationToken = default);

        Task<BaseResponse> PermissionCreateAsync(string token,
            Model.Identity.Request.Permission.CreateRequest request,
            CancellationToken cancellationToken = default);

        Task<BaseResponse> PermissionUpdateAsync(string token,
            Model.Identity.Request.Permission.UpdateRequest request,
            CancellationToken cancellationToken = default);

        Task<BaseResponse> PermissionUpdateSortOrderAsync(string token,
            Model.Identity.Request.Permission.UpdateSortOrderRequest request,
            CancellationToken cancellationToken = default);

        Task<BaseResponse> PermissionDeleteAsync(string token, string permissionId,
            CancellationToken cancellationToken = default);

        Task<GetOnRoleStateAndTreeFormatResponse> PermissionGetOnRoleStateAndTreeFormatAsync(string token,
            Guid softwareId, Guid roleId, CancellationToken cancellationToken = default);

        #endregion

        #endregion
    }
}