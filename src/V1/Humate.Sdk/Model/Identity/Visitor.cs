using System;

namespace Humate.Sdk.Model.Identity
{
    public class Visitor
    {
        public Guid VisitorId { get; set; }

        public string Ip { get; set; }

        public double ExecuteTime { get; set; }

        public string Browser { get; set; }

        public string Platform { get; set; }

        public string Device { get; set; }

        public string UserAgent { get; set; }

        public string UrlPath { get; set; }

        public bool IsOurUser { get; set; }

        public Guid? UserId { get; set; }

        public User User { get; set; }

        public string StatusCode { get; set; }
    }
}
