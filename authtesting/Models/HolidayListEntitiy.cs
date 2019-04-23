using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class HolidayListEntitiy
    {
        //public int StatusCode { get; set; }
        //public string StatusMessage { get; set; }
        public Status status { get; set; }
        public List<HolidayDetails> holidayDetails { get; set; }
    }

    public class HolidayDetails
    {
        public string HolidayName { get; set; }
        public string HolidayDate { get; set; }
        public string isOptionalHoliday { get; set; }

    }
}