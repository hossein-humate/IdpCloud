using System;
using System.Collections.Generic;

namespace Humate.Sdk.Model.BaseInfo
{
    public class MasterDetail
    {
        public Guid MasterDetailId { get; set; }

        public string Name { get; set; }

        public string Parameter { get; set; }

        public Guid MasterId { get; set; }

        public int? Order { get; set; }

        public IList<MasterDetail> Details { get; set; }
    }
}
