using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Payment
{
    [Table(name: "InvoiceItems", Schema = "Payment")]
    public class InvoiceItem : BaseEntity
    {
        public InvoiceItem()
        {
            InvoiceItemId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid InvoiceItemId { get; set; }

        public Guid InvoiceId { get; set; }

        public Invoice Invoice { get; set; }

        public string Description { get; set; }

        public double Price { get; set; } = 7;

        public double Discount { get; set; }
    }
}
