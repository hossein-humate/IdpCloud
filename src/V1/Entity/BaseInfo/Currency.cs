using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.BaseInfo
{
    [Table(name: "Currencies", Schema = "BaseInfo")]
    public class Currency : BaseEntity
    {
        public Currency()
        {
            IsCryptoCurrency = false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CurrencyId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string NativeSymbol { get; set; }
        public byte DecimalDigits { get; set; }
        public int Rounding { get; set; }
        public string Code { get; set; }
        public string PluralName { get; set; }
        public bool? IsCryptoCurrency { get; set; }
    }
}
