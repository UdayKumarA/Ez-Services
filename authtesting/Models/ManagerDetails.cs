using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class ManagerDetails
    {

        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public TotalRows totalRow { get; set; }

        public List<AllUsersTimeSheetDetails> mytimesheets { get; set; }
        public List<ManagerTimesheet> timesheetsforapproval { get; set; }

    }

    public class TotalRows
    {
        public int TotalCountforApproval { get; set; }
    }
    public class TimesheetDetails
    {

        public string userid { get; set; }
        public int startposition { get; set; }
        public int endPosition { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    }
}