using IdpCloud.DataProvider.Entity.BaseInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.BaseInfo
{
    public class CountryConfiguration : BaseEntityConfiguration<Country>
    {
        public override void Configure(EntityTypeBuilder<Country> builder)
        {
            base.Configure(builder);
            builder.Property(c => c.CommonName)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(c => c.OfficialName)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(c => c.CommonNativeName)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(c => c.OfficialNativeName)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(c => c.TwoCharacterCode)
                .HasMaxLength(2)
                .IsFixedLength()
                .IsRequired();
            builder.Property(c => c.ThreeCharacterCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .IsRequired();
            builder.Property(c => c.CallingCode)
                .HasMaxLength(10)
                .IsRequired();
            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);
             //builder.HasData(CountrySeed.Get());
        }
    }
}
