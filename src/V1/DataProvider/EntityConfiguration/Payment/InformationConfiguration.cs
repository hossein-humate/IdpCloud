using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.Payment;

namespace DataProvider.EntityConfiguration.Payment
{
    public class InformationConfiguration : BaseEntityConfiguration<Information>
    {
        public override void Configure(EntityTypeBuilder<Information> builder)
        {
            base.Configure(builder);
            builder.HasMany(c => c.Invoices)
                .WithOne(u => u.Information)
                .HasForeignKey(i => i.InformationId);
        }
    }
}
