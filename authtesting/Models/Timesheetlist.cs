using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class Timesheetlist
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<TimeSheetDetails> timeSheetDetails { get; set; }

    }

    public class TimeSheetDetails
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int Taskid { get; set; }
        public string Taskname { get; set; }
        public int ProjectId { get; set; }
        public int NoofHoursWorked { get; set; }
        public string Comments { get; set; }
        public string TaskDate { get; set; }
        public string Submitted_Type { get; set; }


    }
}