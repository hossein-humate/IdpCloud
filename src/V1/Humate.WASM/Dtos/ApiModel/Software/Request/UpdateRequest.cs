using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Humate.WASM.Dtos.ApiModel.Software.Request
{
    public class UpdateRequest
    {
        public Guid SoftwareId { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string BusinessDescription { get; set; }
        public byte[] LogoContent { get; set; }

        public string LogoName { get; set; }
    }
}
