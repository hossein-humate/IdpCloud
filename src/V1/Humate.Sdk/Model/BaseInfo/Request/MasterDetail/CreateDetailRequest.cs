using System;

namespace Humate.Sdk.Model.BaseInfo.Request.MasterDetail
{
    public class CreateDetailRequest
    {
        public string Name { get; set; }

        public string Parameter { get; set; }

        public Guid MasterId { get; set; }

        public int? Order { get; set; }
    }
}
