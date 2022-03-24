using IdpCloud.DataProvider.Entity.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.Identity
{
    public class OrganisationConfiguration : BaseEntityConfiguration<Organisation>
    {
        public override void Configure(EntityTypeBuilder<Organisation> builder)
        {
            base.Configure(builder);
            builder.HasKey(e => e.OrganisationId);
            builder.Property(o => o.OrganisationId)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Name)
                .IsRequired()
              .HasMaxLength(250);

            builder.Property(a => a.BillingEmail)
               .IsRequired()
             .HasMaxLength(500);

            builder.Property(a => a.Phone)
             .HasMaxLength(25);

            builder.Property(a => a.BillingAddress)
                .IsRequired()
             .HasMaxLength(500);

            builder.Property(a => a.VatNumber)
            .HasMaxLength(20);

            builder.HasMany(u => u.Users)
                .WithOne(s => s.Organisation)
                .HasForeignKey(a => a.OrganisationId);
        }
    }
}
