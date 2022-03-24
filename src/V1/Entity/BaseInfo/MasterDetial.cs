using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entity.Identity;

namespace Entity.BaseInfo
{
    [Table(name: "MasterDetails", Schema = "BaseInfo")]
    public class MasterDetail : BaseEntity
    {
        public MasterDetail()
        {
            MasterDetailId = Guid.NewGuid();
        }

        ~MasterDetail() { Dispose(true); }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid MasterDetailId { get; set; }

        public string Name { get; set; }

        public string Parameter { get; set; }

        public Guid MasterId { get; set; }

        public MasterDetail Master { get; set; }

        public Guid SoftwareId { get; set; }

        public Software Software { get; set; }

        public int? Order { get; set; }

        public List<MasterDetail> Details { get; set; }
    }
}
