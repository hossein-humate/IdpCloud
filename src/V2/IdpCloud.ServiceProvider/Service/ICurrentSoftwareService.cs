using IdpCloud.DataProvider.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service
{
    /// <summary>
    /// The current UserSession Service
    /// </summary>
    public interface ICurrentSoftwareService
    {
        /// <summary>
        /// The UserSession object with private setter.
        /// </summary>
        Software Software { get; }
    }
}
