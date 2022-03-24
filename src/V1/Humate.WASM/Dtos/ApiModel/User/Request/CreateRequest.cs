using System;

namespace Humate.WASM.Dtos.ApiModel.User.Request
{
    public class CreateRequest
    {
        public Guid SoftwareId { get; set; }

        public Guid RoleId { get; set; }

        public string Username { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public ActiveStatus Status { get; set; }

        public string Description { get; set; }

        public short CountryLivingId { get; set; }

        public short NationalityId { get; set; }

        public string Address { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public short? LanguageId { get; set; }
    }
}
