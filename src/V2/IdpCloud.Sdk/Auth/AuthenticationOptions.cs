using System;
using System.Collections.Generic;
using IdpCloud.Sdk.Model;
using Microsoft.AspNetCore.Http;

namespace IdpCloud.Sdk.Auth
{
    public class AuthenticationOptions
    {
        public AuthenticationOptions()
        {
            AnonymousUrls = new HashSet<string>();
            AuthenticationHeader = "Authorization";
            HasSchema = true;
            AuthenticationSchema = "Bearer";
        }

        /// <summary>
        /// Add urls path to exclude from authorization pipeline. example: {"/api/user/add", ...}
        /// <para> Hint: Conditional statement check the current Request.Path does StartWith one of the item in this enumerable.</para>
        /// </summary>
        public IEnumerable<string> AnonymousUrls { get; set; }

        public string AuthenticationHeader { get; set; }

        public bool HasSchema { get; set; }

        public string AuthenticationSchema { get; set; }

        /// <summary>
        /// Invoke Before Basemap Authentication handler start processing.
        /// </summary>
        public Action<HttpContext> Before { get; set; }

        /// <summary>
        /// Invoke After Basemap Authentication handler reach end of process.
        /// </summary>
        public Action<HttpContext, IAuthUser> After { get; set; }

        /// <summary>
        /// Invoke when Basemap Authentication handler failed for any kind of reason.
        /// <para> Hint: For example if provided Schema is wrong, The JwtToken has been Expired or LimitOnRefreshTime.</para>
        /// <para> After invocation is completed, The returned value will be write to HttpContext.Response</para>
        /// </summary>
        public Func<BaseResponse, object> AuthenticationFailed { get; set; }
    }
}
