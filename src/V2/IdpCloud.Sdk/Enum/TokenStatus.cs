namespace IdpCloud.Sdk.Enum
{
    /// <summary>
    /// Token Status type
    /// </summary>
    public enum TokenStatus
    {
        /// <summary>
        /// Token is valid
        /// </summary>
        Valid = 0,
        /// <summary>
        /// Token is Invalid or expired
        /// </summary>
        Invalid = 1,
        /// <summary>
        /// Token has refresh time for Refresh token
        /// </summary>
        HasRefreshTime = 2
    }
}
