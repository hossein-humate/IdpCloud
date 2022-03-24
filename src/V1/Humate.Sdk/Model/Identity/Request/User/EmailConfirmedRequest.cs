using System;

namespace Humate.Sdk.Model.Identity.Request.User
{
    public class EmailConfirmedRequest
    {
        public Guid UserId { get; set; }
        public string Secret { get; set; }
    }
}
