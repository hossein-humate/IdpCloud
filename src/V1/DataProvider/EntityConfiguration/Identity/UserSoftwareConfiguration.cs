using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataProvider.EntityConfiguration.Identity
{
    public class UserSoftwareConfiguration : BaseEntityConfiguration<UserSoftware>
    {
        public override void Configure(EntityTypeBuilder<UserSoftware> builder)
        {
            base.Configure(builder);
            builder.HasOne(td => td.User)
                .WithMany(c => c.UserSoftwares)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(td => td.Software)
                .WithMany(c => c.UserSoftwares)
                .HasForeignKey(a => a.SoftwareId);
        }
    }
}
