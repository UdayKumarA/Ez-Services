using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class UserLeavesApprovedEntites
    {

        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        //public int Employeeid { get; set; }
        //public string Employeename { get; set; }
        //public string Startdate { get; set; }
        public List<MyLeaveDetails> myLeaveDetails { get; set; }
        // public List<TeamLeaves> teamleavedetails { get; set; }
        public List<TeamLeaveDetails> TeamLeaveDetails { get; set; }

    }


    public class MyLeaveDetails
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveDate { get; set; }
        public string EndDate { get; set; }
    }

    public class TeamLeaveDetails
    {
        public string LeaveDate { get; set; }
        //public List<TeamLeaves> EmpDetails { get; set; }
        public List<TeamLeaves> Empdetails { get; set; }
    }

    public class TeamLeaves
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

    }

}