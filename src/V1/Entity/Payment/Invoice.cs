using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entity.BaseInfo;
using Entity.Identity;

namespace Entity.Payment
{
    [Table(name: "Invoices", Schema = "Payment")]
    public class Invoice : BaseEntity
    {
        public Invoice()
        {
            InvoiceId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid InvoiceId { get; set; }

        public Guid? InformationId { get; set; }

        public Information Information { get; set; }

        public InvoiceStatus Status { get; set; }

        public string Number { get; set; }

        public string Description { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; }

        public double Discount { get; set; }

        public double TaxPercent { get; set; } = 21;

        public short CurrencyId { get; set; } = 3;

        public Currency Currency { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public string StripePaymentId { get; set; }

        [Description("This is Stripe Payment Intent Client Secret")]
        public string StripeClientSecret { get; set; }
    }

    public enum InvoiceStatus : byte
    {
        Issued,
        Sent,
        Paid,
        Recalculated
    }
}
