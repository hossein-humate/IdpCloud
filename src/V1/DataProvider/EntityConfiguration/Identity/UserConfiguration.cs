using Entity.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataProvider.EntityConfiguration.Identity
{
    public class UserConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            //builder.HasMany(u => u.Visits)
            //    .WithOne(v => v.User)
            //    .HasForeignKey(v => v.UserId);
            builder.HasMany(u => u.UserSoftwares)
                .WithOne(s => s.User)
                .HasForeignKey(a => a.UserId);
            builder.HasMany(u => u.UserRoles)
                .WithOne(s => s.User)
                .HasForeignKey(a => a.UserId);
            builder.HasMany(u => u.UserPermissions)
                .WithOne(s => s.User)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(td => td.Language)
                .WithMany(c => c.Users)
                .HasForeignKey(a => a.LanguageId);
            builder.HasMany(u => u.Addresses)
                .WithOne(s => s.User)
                .HasForeignKey(a => a.UserId);
            //builder.HasData(UserSeed.Get());
        }
    }
}
