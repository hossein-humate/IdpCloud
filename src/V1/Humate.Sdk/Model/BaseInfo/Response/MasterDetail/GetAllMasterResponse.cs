using System.Collections.Generic;

namespace Humate.Sdk.Model.BaseInfo.Response.MasterDetail
{
    public class GetAllMasterResponse : BaseResponse
    {
        public IEnumerable<BaseInfo.MasterDetail> Masters { get; set; }
    }
}
