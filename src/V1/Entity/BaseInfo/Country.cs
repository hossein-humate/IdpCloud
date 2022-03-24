using Entity.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.BaseInfo
{
    [Table("Countries", Schema = "BaseInfo")]
    public class Country : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CountryId { get; set; }
        public string CommonName { get; set; }
        public string OfficialName { get; set; }
        public string CommonNativeName { get; set; }
        public string OfficialNativeName { get; set; }
        public string TwoCharacterCode { get; set; }
        public string ThreeCharacterCode { get; set; }
        public string CallingCode { get; set; }
        public bool? IsActive { get; set; }

        public List<Person> CountryLivingPersons { get; set; }
        public List<Person> NationalityPersons { get; set; }
        public List<City> Cities { get; set; }
        public List<Address> Addresses { get; set; }
    }
}
