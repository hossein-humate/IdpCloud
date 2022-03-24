using Entity.BaseInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataProvider.EntityConfiguration.BaseInfo
{
    public class MasterDetailConfiguration : BaseEntityConfiguration<MasterDetail>
    {
        public override void Configure(EntityTypeBuilder<MasterDetail> builder)
        {
            base.Configure(builder);
            builder.HasMany(p => p.Details)
                .WithOne(c => c.Master)
                .HasForeignKey(u => u.MasterId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(m => m.Software)
                .WithMany(s => s.MasterDetails)
                .HasForeignKey(m => m.SoftwareId);
            //builder.HasData(MasterDetailSeed.Get());
        }
    }
}
