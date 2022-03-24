using IdpCloud.DataProvider.Entity.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.Identity
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
            builder.HasOne(s => s.JwtSetting)
                .WithOne(j => j.Software);
            builder.HasOne(s => s.SoftwareDetail)
              .WithOne(j => j.Software);
        }
    }
}
