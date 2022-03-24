using IdpCloud.DataProvider.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.Identity
{
    public class PermissionConfiguration : BaseEntityConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            base.Configure(builder);
            builder.HasMany(p => p.Childrens)
                .WithOne(c => c.Parent)
                .HasForeignKey(u => u.ParentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(p => p.Software)
                .WithMany(s => s.Permissions)
                .HasForeignKey(p => p.SoftwareId);
            builder.HasMany(p => p.RolePermissions)
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.PermissionId);
            builder.HasMany(p => p.UserPermissions)
                .WithOne(up => up.Permission)
                .HasForeignKey(u => u.UserId); 
            //builder.HasData(PermissionSeed.Get());
        }
    }
}
