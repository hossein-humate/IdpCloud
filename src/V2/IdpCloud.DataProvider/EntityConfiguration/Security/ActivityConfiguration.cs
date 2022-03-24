using IdpCloud.DataProvider.Entity.Security;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.Security
{
    internal class ActivityConfiguration : BaseEntityConfiguration<Activity>
    {
        public override void Configure(EntityTypeBuilder<Activity> builder)
        {
            base.Configure(builder);
            builder.HasKey(a => a.ActivityId);

            builder.Property(a => a.ActivityId)
                .ValueGeneratedNever();

            builder.Property(a => a.Ip)
              .HasMaxLength(15);

            builder.HasOne(a => a.User)
                .WithMany(u => u.Activities)
                .HasForeignKey(a => a.UserId);

            builder.HasOne(a => a.ResetPassword)
               .WithMany(u => u.Activities)
               .HasForeignKey(a => a.ResetPasswordId);
        }
    }
}
