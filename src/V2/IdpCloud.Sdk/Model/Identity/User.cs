using System;
using System.Collections.Generic;
using IdpCloud.Sdk.Enum;
using IdpCloud.Sdk.Model.BaseInfo;

namespace IdpCloud.Sdk.Model.Identity
{
    /// <summary>
    /// User DTO represent the User Account information
    /// </summary>
    public class User
    {
        /// <summary>
        /// Represent the UserId
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Represent the Username of this account
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Represent the Firstname of the account owner
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// Represent the Lastname of the account owner
        /// </summary>
        public string Lastname { get; set; }

        /// <summary>
        /// Represent the Mobile number of the account owner
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Represent the Email address of the account owner
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Represent the Active or Deactive status of this account
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        /// Represent the Description about this account
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Represent the Registeration IP address when this account created 
        /// </summary>
        public string RegisterIp { get; set; }

        /// <summary>
        /// Represent the Email Confirmed Status of this user account 
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Represent the Profile Picture of this user account 
        /// </summary>
        public string Picture { get; set; }

        /// <summary>
        /// Represent the Last successful Login IP Address when this user account logged in
        /// </summary>
        public string LastLoginIp { get; set; }

        /// <summary>
        /// Represent the Registeration Datetime when this account created 
        /// </summary>
        /// <remarks>
        /// This parameter accept NULL value if this user account created by administrator of organisation
        /// If this parameter is NOT NULL then it represent that this account was created by the owner of User Account
        /// and He/She was passed the registeration process.
        /// </remarks>
        public DateTime? RegisterDate { get; set; }

        /// <summary>
        /// Represent how many times a user successfully logged in with this User Account
        /// </summary>
        public int LoginTimes { get; set; }

        /// <summary>
        /// Represent the Last successful login Datetime
        /// </summary>
        /// <remarks>
        /// This parameter accept NULL value if this user account created by administrator of organisation or account owner
        /// and He/She never used this account to logged in to the SSO application.<br/>
        /// If this parameter is NOT NULL then it represent that this account has been used before 
        /// and logged in successfully.
        /// </remarks>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Represent the Create Datetime
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Represent the Language of this account
        /// </summary>
        public Language Language { get; set; }

        /// <summary>
        /// Represent the list of Softwares this user account has access
        /// </summary>
        public IEnumerable<string> AccessSoftwares { get; set; }
    }
}
