using IdpCloud.DataProvider.Entity.SSO;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.SSO
{
    internal class UserSessionConfiguration : BaseEntityConfiguration<UserSession>
    {
        public override void Configure(EntityTypeBuilder<UserSession> builder)
        {
            base.Configure(builder);
            builder.HasOne(td => td.User)
                .WithMany(c => c.UserSessions)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(td => td.Software)
                .WithMany(c => c.UserSessions)
                .HasForeignKey(a => a.SoftwareId);
        }
    }
}
