using System.Collections.Generic;

namespace Humate.Sdk.Model.BaseInfo.Response.MasterDetail
{
    public class GetAllDetailResponse : BaseResponse
    {
        public IEnumerable<BaseInfo.MasterDetail> Details { get; set; }
    }
}
