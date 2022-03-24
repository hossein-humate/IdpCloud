using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entity.BaseInfo;
using Entity.Identity;

namespace Entity.Payment
{
    [Table(name: "Informations", Schema = "Payment")]
    public class Information : BaseEntity
    {
        public Information()
        {
            InformationId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid InformationId { get; set; }

        public string StripeCustomerId { get; set; }

        [Description("This is Stripe Setup Intent Client Secret")]
        public string StripeClientSecret { get; set; }

        public string StripePaymentMethodId { get; set; }

        public string StripeJsonPayload { get; set; }

        public PaymentMethod Method { get; set; }

        public string Description { get; set; }

        public string CreditCardNumber { get; set; }

        public string CardHolder { get; set; }

        public string Iban { get; set; }

        public string AccountHolderName { get; set; }

        public string Email { get; set; }

        public bool IsDefault { get; set; }

        public string TaxId { get; set; }

        public long ExpirationDate { get; set; }

        public string SecurityCode { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public short CurrencyId { get; set; }

        public Currency Currency { get; set; }

        public List<Invoice> Invoices { get; set; }

        public static string GetPaymentMethod(PaymentMethod item)
        {
            return item switch
            {
                PaymentMethod.SepaDirectDebit => "sepa_debit",
                PaymentMethod.CreditCard => "card",
                PaymentMethod.IdealBankRedirect => "ideal",
                _ => "card"
            };
        }
    }

    public enum PaymentMethod : byte
    {
        SepaDirectDebit = 0,
        CreditCard,
        IdealBankRedirect
    }
}
