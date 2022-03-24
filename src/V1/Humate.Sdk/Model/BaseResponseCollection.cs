using System.Collections.Generic;
using System.Linq;

namespace Humate.Sdk.Model
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

            #region Unhandle Error from 1 to 10
		    new BaseResponse
            {
                Message = "Humate Server cannot handle something, Please contact for support.",
                ResultCode = RequestResult.UnHandledException
            },
            #endregion

            #region Not Allowed Error from 11 to 20
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
            #endregion

            #region General Error 21 to 50
            new BaseResponse
            {
                Message = "Please fill all required parameters and try again.",
                ResultCode = RequestResult.FillRequiredParameter
            },
            new BaseResponse
            {
                Message = "This User has not Assigned to the related Data.",
                ResultCode = RequestResult.NotAssignYet
            },
            new BaseResponse
            {
                Message = "You entered invalid entries.",
                ResultCode = RequestResult.InvalidDataEntries
            },
            new BaseResponse
            {
                Message = "Invalid file format or content-type.",
                ResultCode = RequestResult.InvalidFileFormat
            },
	        #endregion

            #region BaseInfo Errors
		  
            #region MasterDetail Error 2001 to 2050
            new BaseResponse
            {
                ResultCode = RequestResult.InvalidMasterDetailId,
                Message = "Cannot find any Master with this 'MasterDetailId' based on your " +
                          "data Access."
            },
            new BaseResponse
            {
                ResultCode = RequestResult.ParameterAlreadyExist,
                Message = "'Parameter' value already exist, 'Parameter' value in MasterDetail" +
                          " for each software must be unique."
            },
	        #endregion

            #region City Error 2051 to 2100
            new BaseResponse
            {
                ResultCode = RequestResult.RequiredCityId,
                Message = "Parameters 'CityId' must be filled."
            },
            new BaseResponse
            {
                ResultCode = RequestResult.InvalidCityId,
                Message = "Cannot find city by this 'CityId'"
            },
            #endregion

            #region Country Error 2101 to 2150
            new BaseResponse
            {
                ResultCode = RequestResult.RequiredCountryId,
                Message = "Parameters 'CountryId' must be filled."
            },
            new BaseResponse
            {
                ResultCode = RequestResult.InvalidCountryId,
                Message = "Cannot find city by this 'CountryId'"
            },
            #endregion 
	        #endregion

            #region Identity Errors
            #region Address Error from 3001 to 3050
            new BaseResponse
            {
                Message = "Parameters 'AddressId' must be filled.",
                ResultCode = RequestResult.RequiredAddressId
            },
            new BaseResponse
            {
                Message = "Cannot find address by this 'AddressId'",
                ResultCode = RequestResult.InvalidAddressId
            },
	        #endregion

            #region Software Error from 3051 to 3100
            new BaseResponse
            {
                Message = "Cannot find Software by this 'SoftwareId'",
                ResultCode = RequestResult.InvalidSoftwareId
            },
            new BaseResponse
            {
                Message = "Parameter SoftwareId must be filled.",
                ResultCode = RequestResult.InvalidSoftwareId
            },
            #endregion

            #region Permission Error from 3101 to 3150
            new BaseResponse
            {
                Message = "Parameters 'Name','SoftwareId' and 'ParentId' must be filled.",
                ResultCode = RequestResult.RequiredNameSoftwareIdParentId
            },
            new BaseResponse
            {
                Message = "The provided permissions array is not valid,This array contains an invalid 'PermissionId'.",
                ResultCode = RequestResult.InvalidArrayOfPermissionIds
            },
            #endregion

            #region Role Error from 3151 to 3200
            new BaseResponse
            {
                Message = "You have this Role 'Name' in your software, 'Name' parameter must be unique.",
                ResultCode = RequestResult.RoleNameAlreadyExist
            },
            new BaseResponse
            {
                Message = "You can define Only One Default Role per software.",
                ResultCode = RequestResult.OneDefaultRoleOnly
            },
            #endregion

            #region User Error from 3201 to 3300
            new BaseResponse
            {
                Message = "This Username has been registered before.",
                ResultCode = RequestResult.UsernameExistBefore
            },
            new BaseResponse
            {
                Message = "Cannot find User by this 'UserId'.",
                ResultCode = RequestResult.InvalidUserId
            },
            new BaseResponse
            {
                Message = "Email/Username or Password is not correct.",
                ResultCode = RequestResult.WrongUsernamePassword
            },
            new BaseResponse
            {
                Message = "Cannot find image for this User.",
                ResultCode = RequestResult.UserImageNotFound
            }, 
            new BaseResponse
            {
                Message =  "The Email Activation Link has been expired, Request a new Activation Link.",
                ResultCode = RequestResult.ActivationEmailExpired
            },
            new BaseResponse
            {
                Message =  "This Email has been confirmed before.",
                ResultCode = RequestResult.EmailConfirmedBefore
            },
            new BaseResponse
            {
                Message =  "Profile 'Picture' could not be empty or null.",
                ResultCode = RequestResult.ProfilePictureNull
            },
            new BaseResponse
            {
                Message =  "Profile 'Picture' could not be more than 2MB.",
                ResultCode = RequestResult.ProfilePictureSize
            },
            new BaseResponse
            {
                Message =  "Username already exist.",
                ResultCode = RequestResult.UsernameExist
            },
            new BaseResponse
            {
                Message =  "Email already exist.",
                ResultCode = RequestResult.EmailExist
            },
            new BaseResponse
            {
                Message =  "Password length should be at least 8 character.",
                ResultCode = RequestResult.LowPasswordLength
            },
            new BaseResponse
            {
                Message = "The provided users array is not valid,This array contains an invalid 'UserId'.",
                ResultCode = RequestResult.InvalidArrayOfUserIds
            },
            #endregion
	        #endregion

            #region SSO Errors
            #region UserSession Error from 4001 to 4050
            new BaseResponse
            {
                Message = "You have not active session with provided token.",
                ResultCode = RequestResult.NotAnyActiveSession
            },
            #endregion
	        #endregion
        };

        public static BaseResponse GetBaseResponse(RequestResult result)
        {
            return BaseResponses.FirstOrDefault(r => r.ResultCode == result);
        }
    }

    public enum RequestResult
    {
        RequestSuccessful = 0,

        #region Unhandle Error from 1 to 10
        UnHandledException = 2,
        #endregion

        #region Not Allowed Error from 11 to 20
        NotAllowedThisOperation = 11,
        UserSessionInvalidOrExpired = 12,
        InvalidApiKey = 13,
        #endregion

        #region General Error 21 to 30
        FillRequiredParameter = 21,
        NotAssignYet = 22,
        InvalidDataEntries = 23,
        InvalidFileFormat = 24,
        #endregion

        #region BaseInfor Errors

        #region MasterDetail Error from 2001 to 2050
        InvalidMasterDetailId = 2001,
        ParameterAlreadyExist = 2002,
        #endregion

        #region City Error from 2051 to 2100
        RequiredCityId = 2051,
        InvalidCityId = 2052,
        #endregion

        #region Country Error from 2101 to 2150
        RequiredCountryId = 2101,
        InvalidCountryId = 2150,
        #endregion
        #endregion

        #region Identity Errors
        #region Address Error from 3001 to 3050
        RequiredAddressId=3001,
        InvalidAddressId=3002,
        #endregion

        #region Software Error from 3051 to 3100
        InvalidSoftwareId = 3051,
        RequiredSoftwareId = 3052,
        #endregion

        #region Permission Error from 3101 to 3150
        RequiredNameSoftwareIdParentId = 3101,
        InvalidArrayOfPermissionIds=3102,
        #endregion

        #region Role Error from 3151 to 3200
        RoleNameAlreadyExist = 3151,
        OneDefaultRoleOnly = 3152,
        #endregion

        #region User Error from 3201 to 3300
        UsernameExistBefore = 3201,
        InvalidUserId = 3202,
        WrongUsernamePassword = 3203,
        UserImageNotFound = 3204,
        ActivationEmailExpired = 3205,
        EmailConfirmedBefore = 3206,
        ProfilePictureNull = 3207,
        ProfilePictureSize = 3207,
        UsernameExist = 3208,
        EmailExist = 3209,
        LowPasswordLength = 3210,
        InvalidArrayOfUserIds = 3211,
        #endregion
        #endregion

        #region SSO Errors

        #region UserSession Error from 4001 to 4050
        NotAnyActiveSession = 4001,
        #endregion
        #endregion
    }
}
