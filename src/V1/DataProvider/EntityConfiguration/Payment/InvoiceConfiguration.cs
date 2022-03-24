using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.Payment;

namespace DataProvider.EntityConfiguration.Payment
{
    public class InvoiceConfiguration : BaseEntityConfiguration<Invoice>
    {
        public override void Configure(EntityTypeBuilder<Invoice> builder)
        {
            base.Configure(builder);
            builder.HasOne(c => c.Information)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.InformationId);
            builder.HasMany(c => c.InvoiceItems)
                .WithOne(u => u.Invoice)
                .HasForeignKey(i => i.InvoiceId);
        }
    }
}
