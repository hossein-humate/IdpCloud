using IdpCloud.DataProvider.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.Identity
{
    public class RolePermissionConfiguration : BaseEntityConfiguration<RolePermission>
    {
        public override void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            base.Configure(builder);
            builder.HasOne(p => p.Permission)
                .WithMany(s => s.RolePermissions)
                .HasForeignKey(p => p.PermissionId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(p => p.Role)
                .WithMany(rp => rp.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);
        }
    }
}
