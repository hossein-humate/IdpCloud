using DataProvider.Seed;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.BaseInfo;

namespace DataProvider.EntityConfiguration.BaseInfo
{
    public class CityConfiguration : BaseEntityConfiguration<City>
    {
        public override void Configure(EntityTypeBuilder<City> builder)
        {
            base.Configure(builder);
            builder.HasOne(c => c.Country)
                .WithMany()
                .HasForeignKey(t => t.CountryId);
            builder.HasMany(c => c.Addresses)
                .WithOne()
                .HasForeignKey(t => t.CityId);
            //builder.HasData(CitySeed.Get());
        }
    }
}
