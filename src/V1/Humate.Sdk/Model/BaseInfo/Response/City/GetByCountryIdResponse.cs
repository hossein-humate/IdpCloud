using System.Collections.Generic;

namespace Humate.Sdk.Model.BaseInfo.Response.City
{
    public class GetByCountryIdResponse : BaseResponse
    {
        public IEnumerable<BaseInfo.City> Cities { get; set; }
    }
}
