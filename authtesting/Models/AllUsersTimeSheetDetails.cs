using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class AllUsersTimeSheetDetails
    {
        public string Month_Year { get; set; }
        public string CompanyBillingHours { get; set; }
        public string ResourceWorkingHours { get; set; }
        public int TimesheetID { get; set; }
        public string TimesheetStatus { get; set; }


    }
    public class ManagerTimesheet
    {
        public int Userid { get; set; }
        public string Username { get; set; }
        public int TimesheetID { get; set; }
        public string Month_Year { get; set; }
        public int CompanyBillMyWorkingHours { get; set; }
        public int CompanyBillingHours { get; set; }
        public string TimesheetStatus { get; set; }



    }
}