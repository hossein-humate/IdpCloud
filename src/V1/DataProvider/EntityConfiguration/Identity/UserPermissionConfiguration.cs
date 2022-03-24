using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataProvider.EntityConfiguration.Identity
{
    public class UserPermissionConfiguration : BaseEntityConfiguration<UserPermission>
    {
        public override void Configure(EntityTypeBuilder<UserPermission> builder)
        {
            base.Configure(builder);
            builder.HasOne(p => p.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(up => up.Permission)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(up => up.PermissionId);
        }
    }
}
