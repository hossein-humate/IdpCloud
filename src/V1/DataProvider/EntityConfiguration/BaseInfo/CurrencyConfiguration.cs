using DataProvider.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity.BaseInfo;

namespace DataProvider.EntityConfiguration.BaseInfo
{
    public class CurrencyConfiguration : BaseEntityConfiguration<Currency>
    {
        public override void Configure(EntityTypeBuilder<Currency> builder)
        {
            base.Configure(builder);
            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(c => c.PluralName)
                .HasMaxLength(100);
            builder.Property(c => c.Symbol)
                .HasMaxLength(10);
            builder.Property(c => c.NativeSymbol)
                .HasMaxLength(10);
            builder.Property(c => c.Code)
                .HasMaxLength(5)
                .IsRequired();
            builder.Property(c => c.DecimalDigits)
                .HasColumnType("tinyint");
            builder.Property(c => c.IsCryptoCurrency)
                .HasDefaultValue(false); 
            //builder.HasData(CurrencySeed.Get());
        }
    }
}
