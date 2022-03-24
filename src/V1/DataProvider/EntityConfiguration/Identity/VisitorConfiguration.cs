using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.Identity;

namespace DataProvider.EntityConfiguration.Identity
{
    public class VisitorConfiguration : BaseEntityConfiguration<Visitor>
    {
        public override void Configure(EntityTypeBuilder<Visitor> builder)
        {
            base.Configure(builder);
            //builder.HasOne(v => v.User)
            //    .WithMany()
            //    .HasForeignKey(a => a.UserId);
            //builder.HasOne(v => v.Recipient)
            //    .WithMany()
            //    .HasForeignKey(a => a.RecipientId);
        }
    }
}
