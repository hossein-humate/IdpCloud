using IdpCloud.DataProvider.Entity.Security;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.Security
{
    internal class ResetPasswordConfiguration : BaseEntityConfiguration<ResetPassword>
    {
        public override void Configure(EntityTypeBuilder<ResetPassword> builder)
        {
            base.Configure(builder);
            builder.HasKey(a => a.ResetPasswordId);
            builder.Property(a => a.ResetPasswordId)
                .ValueGeneratedNever();
            builder.HasMany(rs => rs.Activities)
                .WithOne(a => a.ResetPassword)
                .HasForeignKey(a => a.ResetPasswordId);
            builder.HasOne(rs => rs.User)
                .WithMany(u => u.ResetPasswords)
                .HasForeignKey(rs => rs.UserId);
        }
    }
}
