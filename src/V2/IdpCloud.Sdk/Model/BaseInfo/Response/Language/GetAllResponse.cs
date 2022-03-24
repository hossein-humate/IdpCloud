using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.BaseInfo.Response.Language
{
    public class GetAllResponse : BaseResponse
    {
        public IEnumerable<BaseInfo.Language> Languages { get; set; }
    }
}
