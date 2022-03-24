using IdpCloud.DataProvider.Entity.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.Identity
{
    public class RoleConfiguration : BaseEntityConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            base.Configure(builder);
            builder.HasOne(td => td.Software)
                .WithMany(c => c.Roles)
                .HasForeignKey(u => u.SoftwareId);
            builder.HasMany(r => r.RolePermissions)
                .WithOne(c => c.Role)
                .HasForeignKey(u => u.RoleId);
            builder.HasMany(r => r.UserRoles)
                .WithOne(c => c.Role)
                .HasForeignKey(u => u.RoleId);
        }
    }
}
