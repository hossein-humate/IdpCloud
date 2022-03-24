﻿namespace Humate.WASM.Dtos.ApiModel.Software.Request
{
    public class CreateRequest
    {
        public string Name { get; set; }

        public string Brand { get; set; }

        public string BusinessDescription { get; set; }

        public byte[] LogoContent { get; set; } 

        public string LogoName { get; set; } 
    }
}
