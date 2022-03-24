using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.Identity;

namespace DataProvider.EntityConfiguration.Identity
{
    public class AddressConfiguration : BaseEntityConfiguration<Address>
    {
        public override void Configure(EntityTypeBuilder<Address> builder)
        {
            base.Configure(builder);
            builder.HasOne(c => c.Country)
                .WithMany(t => t.Addresses)
                .HasForeignKey(t => t.CountryId);
            builder.HasOne(c => c.City)
                .WithMany(t => t.Addresses)
                .HasForeignKey(t => t.CityId);
            builder.HasOne(c => c.User)
                .WithMany(t => t.Addresses)
                .HasForeignKey(t => t.UserId);
        }
    }
}
