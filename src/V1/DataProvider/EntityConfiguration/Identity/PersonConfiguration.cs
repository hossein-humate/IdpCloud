using Entity.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataProvider.EntityConfiguration.Identity
{
    public class PersonConfiguration : BaseEntityConfiguration<Person>
    {
        public override void Configure(EntityTypeBuilder<Person> builder)
        {
            base.Configure(builder);
            builder.HasMany(r => r.Users)
                .WithOne(c => c.Person)
                .HasForeignKey(u => u.PersonId);
            builder.HasOne(p=>p.CountryLiving)
                .WithMany(u => u.CountryLivingPersons)
                .HasForeignKey(c => c.CountryLivingId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasOne(p => p.Nationality)
                .WithMany(u => u.NationalityPersons)
                .HasForeignKey(c => c.NationalityId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            //builder.HasData(RoleSeed.Get());
        }
    }
}
