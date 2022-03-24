using IdpCloud.DataProvider.Entity.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.Security
{
    internal class BanListConfiguration : IEntityTypeConfiguration<BanList>
    {
        public virtual void Configure(EntityTypeBuilder<BanList> builder)
        {
            builder.HasKey(b => b.BanListId);

            builder.Property(b => b.BanListId)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Ip)
                .IsRequired()
                .HasMaxLength(15);
        }
    }
}
