using DataProvider.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.BaseInfo;
using Entity.Identity;

namespace DataProvider.EntityConfiguration.BaseInfo
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
            builder.HasMany(c=>c.CountryLivingPersons)
                .WithOne(u => u.CountryLiving)
                .HasForeignKey(c => c.CountryLivingId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasMany(c=>c.NationalityPersons)
                .WithOne(u => u.Nationality)
                .HasForeignKey(c => c.NationalityId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasMany(c => c.Cities)
                .WithOne(u => u.Country)
                .HasForeignKey(c => c.CountryId);
            builder.HasMany(c => c.Addresses)
                .WithOne(u => u.Country)
                .HasForeignKey(c => c.CountryId);
             //builder.HasData(CountrySeed.Get());
        }
    }
}
