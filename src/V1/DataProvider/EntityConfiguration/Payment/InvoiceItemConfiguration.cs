using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.Payment;

namespace DataProvider.EntityConfiguration.Payment
{
    public class InvoiceItemConfiguration : BaseEntityConfiguration<InvoiceItem>
    {
        public override void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            base.Configure(builder);
            builder.HasOne(c => c.Invoice)
                .WithMany(u => u.InvoiceItems)
                .HasForeignKey(i => i.InvoiceId);
        }
    }
}
