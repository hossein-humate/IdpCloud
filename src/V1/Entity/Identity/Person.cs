using Entity.BaseInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Identity
{
    [Table(name: "Persons", Schema = "Identity")]
    public class Person : BaseEntity
    {
        ~Person(){Dispose(true);}
        public Person()
        {
            PersonId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid PersonId { get; set; }

        public Guid SoftwareId { get; set; }

        public Software Software { get; set; }

        public string Firstname { get; set; }

        public string Middlename { get; set; }

        public string Lastname { get; set; }

        public short? CountryLivingId { get; set; }

        public Country CountryLiving { get; set; }

        public short? NationalityId { get; set; }

        public Country Nationality { get; set; }

        public string PicturePath { get; set; }

        public GenderType Gender { get; set; }

        public List<User> Users { get; set; }

        //[NotMapped]
        //public string PersonPictureUrl => string.IsNullOrEmpty(PicturePath) ? string.Empty :
        //    string.Concat(GlobalParameter.CdnDomainUrl, GlobalParameter.PersonImagePath, PicturePath);
    }
    public enum GenderType : byte
    {
        Male,
        Female,
        NotInterest
    }
}
