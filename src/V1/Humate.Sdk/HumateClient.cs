using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Humate.Sdk;
using Humate.Sdk.Model;
using Humate.Sdk.Model.Identity.Request.Software;
using Humate.Sdk.Model.Identity.Request.User;
using Humate.Sdk.Model.Identity.Response.Permission;
using Humate.Sdk.Model.Identity.Response.Software;
using Humate.Sdk.Model.Identity.Response.User;

namespace Humate.Sdk
{
    public class HumateClient : IHumateClient
    {
        public static string BaseUrl { get; set; }
        public static string ApiKey { get; set; }

        #region BaseInfo

        #region City
        public async Task<Model.BaseInfo.Response.City.GetByCountryIdResponse> CityGetByCountryIdAsync(string token,
            short countryId, CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.GetAsync<Model.BaseInfo.Response.City.GetByCountryIdResponse>(
                BaseUrl, $"/api/baseinfo/city/GetByCountryId/{countryId}",
                token, cancellationToken: cancellationToken);
            return response;
        }
        #endregion

        #region Country
        public async Task<Model.BaseInfo.Response.Country.GetAllResponse> CountryGetAllAsync(string token,
            CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.GetAsync<Model.BaseInfo.Response.Country.GetAllResponse>(
                BaseUrl, "/api/baseinfo/country/GetAll",
                token, cancellationToken: cancellationToken);
            return response;
        }
        #endregion

        #region Language
        public async Task<Model.BaseInfo.Response.Language.GetAllResponse> LanguageGetAllAsync(string token,
            CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.GetAsync<Model.BaseInfo.Response.Language.GetAllResponse>(
                BaseUrl, "/api/baseinfo/language/GetAll",
                token, cancellationToken: cancellationToken);
            return response;
        }
        #endregion

        #region MasterDetail
        public async Task<Model.BaseInfo.Response.MasterDetail.GetAllMasterResponse> MasterDetailGetAllMasterAsync(string token,
            CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.GetAsync<Model.BaseInfo.Response.MasterDetail.GetAllMasterResponse>(
                BaseUrl, "/api/baseinfo/masterDetail/GetAllMaster",
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<Model.BaseInfo.Response.MasterDetail.GetAllDetailResponse> MasterDetailGetAllDetailAsync(string token,
            Guid masterId, CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.GetAsync<Model.BaseInfo.Response.MasterDetail.GetAllDetailResponse>(
                BaseUrl, $"/api/baseinfo/masterDetail/GetAllDetail/{masterId}",
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<Model.BaseInfo.Response.MasterDetail.GetByIdResponse> MasterDetailGetByIdAsync(string token,
            Guid masterDetailId, CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.GetAsync<Model.BaseInfo.Response.MasterDetail.GetByIdResponse>(
                BaseUrl, $"/api/baseinfo/masterDetail/GetById/{masterDetailId}",
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<BaseResponse> MasterDetailCreateMasterAsync(string token,
            Model.BaseInfo.Request.MasterDetail.CreateMasterRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.PostAsync<BaseResponse, Model.BaseInfo.Request
                .MasterDetail.CreateMasterRequest>(
                BaseUrl, "/api/baseinfo/masterDetail/CreateMaster", request,
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<BaseResponse> MasterDetailUpdateAsync(string token,
            Model.BaseInfo.Request.MasterDetail.UpdateMasterDetailRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.PutAsync<BaseResponse, Model.BaseInfo.Request
                .MasterDetail.UpdateMasterDetailRequest>(
                BaseUrl, "/api/baseinfo/masterDetail/UpdateMasterDetail", request,
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<BaseResponse> MasterDetailCreateDetailAsync(string token,
           Model.BaseInfo.Request.MasterDetail.CreateDetailRequest request,
           CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.PostAsync<BaseResponse, Model.BaseInfo.Request
                .MasterDetail.CreateDetailRequest>(
                BaseUrl, "/api/baseinfo/masterDetail/CreateDetail", request,
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<BaseResponse> MasterDetailDeleteAsync(string token,
            Guid masterDetailId, CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.DeleteAsync<BaseResponse>(
                BaseUrl, $"/api/baseinfo/masterDetail/Delete/{masterDetailId}",
                token, cancellationToken: cancellationToken);
            return response;
        }
        #endregion

        #endregion

        #region Identity

        public async Task<BaseResponse> IsValidTokenAsync(string token,
        CancellationToken cancellationToken = default)
        {
            var generateJwtResponse = await ApiManager.OptionsAsync<BaseResponse>(
                BaseUrl, "/api/identity/IsValidToken", token,
                new Dictionary<string, string> { { "X-Secret-Key", ApiKey } },
                cancellationToken);
            return generateJwtResponse;
        }

        #region Address
        public async Task<Model.Identity.Response.Address.GetByUserIdResponse> AddressGetByUserIdAsync(string token,
            Guid userId, CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.GetAsync<Model.Identity.Response.Address
                .GetByUserIdResponse>(BaseUrl,
                $"/api/identity/address/GetByUserId/{userId}",
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<BaseResponse> AddressCreateAsync(string token,
            Model.Identity.Request.Address.CreateRequest request, CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.PostAsync<BaseResponse, Model.Identity.Request
                .Address.CreateRequest>(BaseUrl,
                "/api/identity/address/Create", request,
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<BaseResponse> AddressUpdateAsync(string token,
            Model.Identity.Request.Address.UpdateRequest request, CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.PostAsync<BaseResponse, Model.Identity.Request
                .Address.UpdateRequest>(BaseUrl,
                "/api/identity/address/Update", request,
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<BaseResponse> AddressDeleteAsync(string token,
            Guid addressId, CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.DeleteAsync<BaseResponse>(BaseUrl,
                $"/api/identity/address/Delete/{addressId}",
                token, cancellationToken: cancellationToken);
            return response;
        }
        #endregion

        #region Permission
        public async Task<Model.Identity.Response.Permission.GetByIdResponse> PermissionGetByIdAsync(
            string token, string permissionId, CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.GetAsync<Model.Identity
                .Response.Permission.GetByIdResponse>(BaseUrl,
                $"/api/identity/permission/GetById/{permissionId}",
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<Model.Identity.Response.Permission.GetBySoftwareIdResponse> PermissionGetBySoftwareIdAsync(string token,
            string softwareId, CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.GetAsync<Model.Identity.Response
                .Permission.GetBySoftwareIdResponse>(
                BaseUrl, $"/api/identity/permission/GetBySoftwareId/{softwareId}",
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<BaseResponse> PermissionCreateAsync(string token,
            Model.Identity.Request.Permission.CreateRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.PostAsync<BaseResponse,
                Model.Identity.Request.Permission.CreateRequest>(
                BaseUrl, "/api/identity/permission/create", request,
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<BaseResponse> PermissionUpdateAsync(string token,
            Model.Identity.Request.Permission.UpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.PutAsync<BaseResponse,
                Model.Identity.Request.Permission.UpdateRequest>(
                BaseUrl, "/api/identity/permission/Update", request,
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<BaseResponse> PermissionUpdateSortOrderAsync(string token,
            Model.Identity.Request.Permission.UpdateSortOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.PatchAsync<BaseResponse, Model.Identity
                .Request.Permission.UpdateSortOrderRequest>(
                BaseUrl, $"/api/identity/permission/UpdateSortOrder", request,
                token, cancellationToken: cancellationToken);
            return response;
        }

        public async Task<BaseResponse> PermissionDeleteAsync(string token, string permissionId,
            CancellationToken cancellationToken = default)
        {
            var permissionDeleteResponse = await ApiManager.DeleteAsync<BaseResponse>(
                BaseUrl, $"/api/identity/permission/Delete/{permissionId}",
                token, cancellationToken: cancellationToken);
            return permissionDeleteResponse;
        }

        public async Task<GetOnRoleStateAndTreeFormatResponse> PermissionGetOnRoleStateAndTreeFormatAsync(string token,
            Guid softwareId, Guid roleId, CancellationToken cancellationToken = default)
        {
            var response = await ApiManager.GetAsync<GetOnRoleStateAndTreeFormatResponse>(
                BaseUrl, $"/api/identity/permission/GetOnRoleStateAndTreeFormat" +
                         $"/{softwareId}/{roleId}", token, cancellationToken: cancellationToken);
            return response;
        }
        #endregion
        #endregion
    }
}
