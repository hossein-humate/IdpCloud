using System;

namespace Humate.Sdk.Model.BaseInfo.Request.MasterDetail
{
    public class UpdateMasterDetailRequest
    {
        public Guid MasterDetailId { get; set; }

        public string Name { get; set; }

        public string Parameter { get; set; }

        public int? Order { get; set; }
    }
}
