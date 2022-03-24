using IdpCloud.DataProvider.Entity.SSO;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.SSO
{
    internal class JwtSettingConfiguration : BaseEntityConfiguration<JwtSetting>
    {
        public override void Configure(EntityTypeBuilder<JwtSetting> builder)
        {
            base.Configure(builder);
            builder.HasOne(j => j.Software)
                .WithOne(s => s.JwtSetting)
                .HasForeignKey<JwtSetting>(j => j.SoftwareId);
        }
    }
}
