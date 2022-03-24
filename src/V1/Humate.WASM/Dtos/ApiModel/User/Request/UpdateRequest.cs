using System;

namespace Humate.WASM.Dtos.ApiModel.User.Request
{
    public class UpdateRequest
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public ActiveStatus Status { get; set; }

        public string Description { get; set; }

        public short? LanguageId { get; set; }
    }
}
