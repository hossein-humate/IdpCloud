using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataProvider.EntityConfiguration.Identity
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
