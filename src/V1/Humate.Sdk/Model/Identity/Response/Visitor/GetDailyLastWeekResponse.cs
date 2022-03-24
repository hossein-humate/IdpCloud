using System;
using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Response.Visitor
{
    public class GetDailyLastWeekResponse : BaseResponse
    {
        public IEnumerable<DailyVisit> LastWeek { get; set; }
    }

    public class DailyVisit
    {
        public int Count { get; set; }
        public DateTime Date { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}
