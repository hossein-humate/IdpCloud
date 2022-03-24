namespace IdpCloud.Sdk.Model
{
    /// <summary>
    /// Represent list of Request result that could send as a response ResultCode in <see cref="BaseResponse.ResultCode"/>
    /// </summary>
    public enum RequestResult
    {
        RequestSuccessful = 0,
        UnHandledException,
        IdentityServiceUnavailable,
        NotAllowedThisOperation,
        UserSessionInvalidOrExpired,
        InvalidApiKey,
        ClientIdInvalidOrNull,
        AuthenticationHeaderNotFound,
        AuthenticationSchemaNotFound,
        EmptyEntryNotAllowed,
        RoleNotFound,
        EmptyRoleId,
        InvalidRoleId,
        EmptyNameAndDescription,
        IsDefaultAlreadyDefined,
        NotFoundRolesForSoftwareId,
        UserNotAccessToAnySoftware,
        SoftwareNotFound,
        EmptyNameAndBusinessDescription,
        EmptySoftwareId,
        InvalidSoftwareId,
        EmptyUserId,
        InvalidUserId,
        UsernameExist,
        EmailExist,
        PasswordLengthLowerThan8,
        EmailUsernameOrPasswordWrong,
        PasswordAndRePasswordNotMatched,
        EmailNotConfirmed,
        ConfirmationLinkExpired,
        ConfirmationCompletedBefore,
        NotFoundPermissionForSoftwareId,
        EmptyPermissionId,
        InvalidPermissionId,
        EmptyNameSoftwareIdParentId,
        EmptyNamePermissionIdParentId,
        UserAlreadyHasAccessToSoftware,
        NotExistUserAccessToSoftware,
        InvalidRefreshToken,
        RefreshTokenIsDisable,
        RefreshTokenHasBeenExpired,
        CannotRefreshBeforeExpiration,
        LimitOnRefreshTime,
        RoleAlreadyHasAccessToPermission,
        NotExistRoleAccessToPermission,
        UserAlreadyHasAccessToRole,
        NotExistUserAccessToRole,
        ResetPasswordExpiredOrInvalid,
        TooManyRequests,
        EmailAddressConfirmed,
        OrganisationHasUser,
        NotExistCannotContinue,

        /// <summary>
        /// The given Old Password is not correct.
        /// </summary>
        OldPasswordWrong
    }
}
