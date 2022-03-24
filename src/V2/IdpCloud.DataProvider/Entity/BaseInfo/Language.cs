using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IdpCloud.DataProvider.Entity.Identity;

namespace IdpCloud.DataProvider.Entity.BaseInfo
{
    [Table(name: "Languages", Schema = "BaseInfo")]
    public class Language: BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short LanguageId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public List<User> Users { get; set; }
    }
}
