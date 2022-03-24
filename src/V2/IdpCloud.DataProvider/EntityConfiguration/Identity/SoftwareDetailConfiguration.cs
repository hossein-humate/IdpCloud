using IdpCloud.DataProvider.Entity.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.Identity
{
    public class SoftwareDetailConfiguration : BaseEntityConfiguration<SoftwareDetail>
    {
        public override void Configure(EntityTypeBuilder<SoftwareDetail> builder)
        {
            base.Configure(builder);
            builder.HasOne(j => j.Software)
                .WithOne(s => s.SoftwareDetail)
                .HasForeignKey<SoftwareDetail>(j => j.SoftwareId);
        }
    }
}
