using System;

namespace IdpCloud.DataProvider.Entity
{
    public class BaseEntity
    {
        protected BaseEntity()
        {
            CreateDate = DateTime.Now;
        }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime? DeleteDate { get; set; }
    }
}
