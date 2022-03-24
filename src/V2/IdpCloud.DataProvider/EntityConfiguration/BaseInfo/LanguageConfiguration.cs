using IdpCloud.DataProvider.Entity.BaseInfo;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpCloud.DataProvider.EntityConfiguration.BaseInfo
{
    public class LanguageConfiguration : BaseEntityConfiguration<Language>
    {
        public override void Configure(EntityTypeBuilder<Language> builder)
        {
            base.Configure(builder);
            builder.HasMany(l => l.Users)
                .WithOne(u => u.Language)
                .HasForeignKey(u => u.LanguageId);
            //builder.HasData(LanguageSeed.Get());
        }
    }
}
