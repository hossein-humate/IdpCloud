using System;
using Humate.WASM.Dtos.ApiModel.User;

namespace Humate.WASM.Common
{
    public class GlobalVariable
    {
        public static string ApiKey;
        public static string ApiBaseAddress;
        public static string Token;
        public static UserApiModel CurrentUser;
        public static Guid CurrentProject;
    }
}
