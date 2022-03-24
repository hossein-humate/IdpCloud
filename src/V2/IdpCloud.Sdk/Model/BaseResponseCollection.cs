using System.Collections.Generic;
using System.Linq;

namespace IdpCloud.Sdk.Model
{
    public static class BaseResponseCollection
    {
        private static readonly List<BaseResponse> BaseResponses = new List<BaseResponse>
        {
            new BaseResponse
            {
                Message = "Request Completed Successfully.",
                ResultCode = RequestResult.RequestSuccessful
            },
            new BaseResponse
            {
                Message = "Basemap Identity Server cannot handle something, Please contact for support.",
                ResultCode = RequestResult.UnHandledException
            },
            new BaseResponse
            {
                Message = "Basemap Identity Server/SSO Service not available right now, Please contact for support if you receive this message again.",
                ResultCode = RequestResult.IdentityServiceUnavailable
            },
            new BaseResponse
            {
                Message = "You have not permission to complete this operation.",
                ResultCode = RequestResult.NotAllowedThisOperation
            },
            new BaseResponse
            {
                Message = "User session has been Expired/Invalid try to Login again and create new credential.",
                ResultCode = RequestResult.UserSessionInvalidOrExpired
            },
            new BaseResponse
            {
                Message = "Invalid ApiKey.",
                ResultCode = RequestResult.InvalidApiKey
            },
            new BaseResponse
            {
                Message = "Cannot accept client request without valid ClientId.",
                ResultCode = RequestResult.ClientIdInvalidOrNull
            },
            new BaseResponse
            {
                Message = "Cannot found the 'AuthenticationHeader' in the current request, Add the AuthenticationHeader Key to the request and try again.",
                ResultCode = RequestResult.AuthenticationHeaderNotFound
            },
            new BaseResponse
            {
                Message = "Cannot found the 'AuthenticationSchema' in the current request AuthenticationHeader, Add the correct AuthenticationSchema value to the request AuthenticationHeader and try again.",
                ResultCode = RequestResult.AuthenticationSchemaNotFound
            },
            new BaseResponse
            {
                Message = "Required parameters in not provided in this request.",
                ResultCode = RequestResult.EmptyEntryNotAllowed
            },
            new BaseResponse
            {
                Message = "Cannot find 'Role'.",
                ResultCode = RequestResult.RoleNotFound
            },
            new BaseResponse
            {
                Message = "Parameters 'Name' and 'Description' must be filled.",
                ResultCode = RequestResult.EmptyNameAndDescription
            },
            new BaseResponse
            {
                Message = "Cannot find any Role by this 'SoftwareId'.",
                ResultCode = RequestResult.NotFoundRolesForSoftwareId
            },
            new BaseResponse
            {
                Message = "Default 'Role' already exist for this software.",
                ResultCode = RequestResult.IsDefaultAlreadyDefined
            },
            new BaseResponse
            {
                Message = "This user has not access to any Software.",
                ResultCode = RequestResult.UserNotAccessToAnySoftware
            },
            new BaseResponse
            {
                Message = "Cannot find software.",
                ResultCode = RequestResult.SoftwareNotFound
            },
            new BaseResponse
            {
                Message = "Parameters 'Name' and 'BusinessDescription' must be filled.",
                ResultCode = RequestResult.EmptyNameAndBusinessDescription
            },
            new BaseResponse
            {
                Message = "Parameters 'SoftwareId' must be filled.",
                ResultCode = RequestResult.EmptySoftwareId
            },
            new BaseResponse
            {
                Message = "Cannot find Software by this 'SoftwareId'.",
                ResultCode = RequestResult.InvalidSoftwareId
            },
            new BaseResponse
            {
                Message = "Parameters 'UserId' must be filled.",
                ResultCode = RequestResult.EmptyUserId
            },
            new BaseResponse
            {
                Message = "Cannot find User by this 'UserId'.",
                ResultCode = RequestResult.InvalidUserId
            },
            new BaseResponse {
                Message = "Username already exist.",
                ResultCode = RequestResult.UsernameExist
            },
            new BaseResponse {
                Message = "Email already exist.",
                ResultCode = RequestResult.EmailExist
            },
            new BaseResponse
            {
                Message = "Password length should be at least 8 character.",
                ResultCode = RequestResult.PasswordLengthLowerThan8
            },
            new BaseResponse
            {
                Message = "Email/Username or Password is not correct.",
                ResultCode = RequestResult.EmailUsernameOrPasswordWrong
            },
            new BaseResponse
            {
                Message = "Before continue to login in the SSO system you need to Confirm you email address.",
                ResultCode = RequestResult.EmailNotConfirmed
            },
            new BaseResponse
            {
                Message = "The Email Confirmation Link has been expired, Request a new Confirmation Link or contact the support.",
                ResultCode = RequestResult.ConfirmationLinkExpired
            },
            new BaseResponse
            {
                Message = "This Email Address has been confirmed before.",
                ResultCode = RequestResult.ConfirmationCompletedBefore
            },
            new BaseResponse
            {
                Message = "Cannot find any Permission by this 'SoftwareId'.",
                ResultCode = RequestResult.NotFoundPermissionForSoftwareId
            },
            new BaseResponse
            {
                Message = "Parameters 'PermissionId' must be filled.",
                ResultCode = RequestResult.EmptyPermissionId
            },
            new BaseResponse
            {
                Message = "Cannot find Permission by this 'PermissionId'.",
                ResultCode = RequestResult.InvalidPermissionId
            },
            new BaseResponse
            {
                Message = "Parameters 'Name','SoftwareId' and 'ParentId' must be filled.",
                ResultCode = RequestResult.EmptyNameSoftwareIdParentId
            },
            new BaseResponse
            {
                Message = "Parameters 'Name','PermissionId' and 'ParentId' must be filled.",
                ResultCode = RequestResult.EmptyNamePermissionIdParentId
            },
            new BaseResponse
            {
                Message = "User already has access to this software.",
                ResultCode = RequestResult.UserAlreadyHasAccessToSoftware
            },
            new BaseResponse
            {
                Message = "Current User access to this software not exist.",
                ResultCode = RequestResult.NotExistUserAccessToSoftware
            },
            new BaseResponse
            {
                Message = "The provided RefreshToken is not valid.",
                ResultCode = RequestResult.InvalidRefreshToken
            },
            new BaseResponse
            {
                Message = "You cannot use RefreshToken when the 'Enable Refresh Token' switch in JWT Settings of the Software is disable.",
                ResultCode = RequestResult.RefreshTokenIsDisable
            },
            new BaseResponse
            {
                Message = "This RefreshToken has been expired, You need to Authenticate with a valid credential to get new Token and RefreshToken.",
                ResultCode = RequestResult.RefreshTokenHasBeenExpired
            },
            new BaseResponse
            {
                Message = "Cannot use RefreshToken before current Token expiration time, Try after Token expired.",
                ResultCode = RequestResult.CannotRefreshBeforeExpiration
            },
            new BaseResponse
            {
                Message = "This Token limited only for refresh, You can use it with your RefreshToken to refresh the current session.",
                ResultCode = RequestResult.LimitOnRefreshTime
            },
            new BaseResponse
            {
                Message = "Role already has access to this permission.",
                ResultCode = RequestResult.RoleAlreadyHasAccessToPermission
            },
            new BaseResponse
            {
                Message = "Current Role access to this permission not exist.",
                ResultCode = RequestResult.NotExistRoleAccessToPermission
            },
            new BaseResponse
            {
                Message = "User have already access to 'Role'.",
                ResultCode = RequestResult.UserAlreadyHasAccessToRole
            },
            new BaseResponse
            {
                Message = "User role does not exists.",
                ResultCode = RequestResult.NotExistUserAccessToRole
            },
            new BaseResponse
            {
                Message = "Reset password Link expired or is not valid, Please try again.",
                ResultCode = RequestResult.ResetPasswordExpiredOrInvalid
            },
            new BaseResponse
            {
                Message = "Too many requests made. IP banned for 24 hours.",
                ResultCode = RequestResult.TooManyRequests
            },
            new BaseResponse
            {
                Message = "The provided Email Address is already confimred.",
                ResultCode = RequestResult.EmailAddressConfirmed
            },
            new BaseResponse
            {
                Message = "Cannot complete operation, Organisation has one or more Users.",
                ResultCode = RequestResult.OrganisationHasUser
            },
            new BaseResponse
            {
                Message = "Not Found/Exist by the given data, Cannot continue the operation.",
                ResultCode = RequestResult.NotExistCannotContinue
            },
            new BaseResponse
            {
                Message = "The given Old Password is not correct.",
                ResultCode = RequestResult.OldPasswordWrong
            }
        };

        /// <summary>
        /// Get the BaseResponse by the given RequestResult and cast it to the generice defined 
        /// </summary>
        /// <typeparam name="T">Is the given type that should inherit from base class <see cref="BaseResponse"/></typeparam>
        /// <param name="result">Represent the Request result</param>
        /// <returns>An instanse of T object with provide ResultCode and Message parameters</returns>
        public static T GetGenericeResponse<T>(RequestResult result) where T : BaseResponse, new()
        {
            var response = BaseResponses.FirstOrDefault(r => r.ResultCode == result);
            return new T
            {
                Message = response.Message,
                ResultCode = response.ResultCode,
            };
        }

        public static BaseResponse GetBaseResponse(RequestResult result)
        {
            return BaseResponses.FirstOrDefault(r => r.ResultCode == result);
        }
    }
}
