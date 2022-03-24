using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdpCloud.DataProvider.Entity.Security
{
    /// <summary>
    /// Ban List entity represent that an IP of client has been Banned,
    /// because of too many unnecessary requests came from that IP in a limited time.
    /// </summary>
    [Table(name: "BanLists", Schema = "Security")]
    public class BanList
    {
        public int BanListId { get; set; }

        /// <summary>
        /// Client Request Ip Address
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Represent when started to Ban this client IP
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Represent 
        /// </summary>
        public DateTime ReleaseDate { get; set; }
    }
}
