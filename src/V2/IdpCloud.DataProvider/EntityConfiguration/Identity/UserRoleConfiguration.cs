using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.Identity
{
    public class UserRoleConfiguration : BaseEntityConfiguration<UserRole>
    {
        public override void Configure(EntityTypeBuilder<UserRole> builder)
        {
            base.Configure(builder);
            builder.HasOne(p => p.User)
                .WithMany(s => s.UserRoles)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(p => p.Role)
                .WithMany(rp => rp.UserRoles)
                .HasForeignKey(rp => rp.RoleId);
        }
    }
}
