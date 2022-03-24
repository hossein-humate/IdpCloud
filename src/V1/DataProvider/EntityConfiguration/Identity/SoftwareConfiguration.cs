using DataProvider.Seed;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.Identity;

namespace DataProvider.EntityConfiguration.Identity
{
    public class SoftwareConfiguration : BaseEntityConfiguration<Software>
    {
        public override void Configure(EntityTypeBuilder<Software> builder)
        {
            base.Configure(builder);
            builder.HasMany(td => td.UserSoftwares)
                .WithOne(c => c.Software)
                .HasForeignKey(u => u.SoftwareId);
            builder.HasMany(s => s.Roles)
                .WithOne(r => r.Software)
                .HasForeignKey(r => r.SoftwareId);
            builder.HasMany(s => s.Permissions)
                .WithOne(r => r.Software)
                .HasForeignKey(r => r.SoftwareId);
            builder.HasMany(s => s.MasterDetails)
                .WithOne(r => r.Software)
                .HasForeignKey(r => r.SoftwareId);
            //builder.HasData(SoftwareSeed.Get());
        }
    }
}
