using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Identity
{
    [Table(name: "SocialNetworks", Schema = "Identity")]
    public class SocialNetwork : BaseEntity
    {
        public SocialNetwork()
        {
            SocialNetworkId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid SocialNetworkId { get; set; }
        ~SocialNetwork() { Dispose(true); }

        public Guid UserId { get; set; }

        public string Username { get; set; }

        public SocialNetworkProvider Provider { get; set; }
    }

    public enum SocialNetworkProvider : byte
    {
        Facebook,
        Instagram,
        Twitter,
        YouTube,
        Telegram,
        Viber,
        WhatsApp,
        Imo,
        Soroush,
        LinkedIn,
        StackOverFlow,
        WeChat,
        Skype,
        SnapChat,
        Reddit,
        Pinterest
    }
}