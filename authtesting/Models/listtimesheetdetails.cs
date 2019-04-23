using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class TotalTimeSheetTimeDetails
    {

        public timesheet timesheets { get; set; }
        public List<listtimesheetdetail> listtimesheetdetails { get; set; }
        public AddTimesheetStatus addTimesheetStatus { get; set; }
        public Status status { get; set; }
    }



    public class TotalTimeSheetInfo
    {

        public Status status { get; set; }
        public TimeSheetTimeData timeSheetActions { get; set; }

    }


    public class AddTimesheetStatus
    {

        public Status status { get; set; }

        public int Transoutput { get; set; }
        public string Message { get; set; }
        public string SubmittedState { get; set; }
        public string SubmittedType { get; set; }


    }
    public class AddTMStatus
    {
        //  public Status status { get; set; }
        public int Transoutput { get; set; }
        public string Message { get; set; }
        public string SubmittedState { get; set; }
        public string SubmittedType { get; set; }
    }

    public class Status
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }

    public class TimeSheetTimeData
    {

        public int EmpUsrID { get; set; }
        public int TimesheetID { get; set; }
        public string Comments { get; set; }
        public string ManagerId { get; set; }
        public string SubmittedType { get; set; }
        public string SubmittedState { get; set; }
        public string Position { get; set; }
        public string Message { get; set; }
        public int Transoutput { get; set; }


    }


    public class timesheet
    {
        public string UserName { get; set; }
        public int TimesheetID { get; set; }
        public int UserID { get; set; }
        public int EmpUsrID { get; set; }
        public string TaskDate { get; set; }
        public string TimeSheetMonth { get; set; }
        public string Comments { get; set; }
        public int ProjectID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string TaskId { get; set; }
        public string EmailAppOrRejStatus { get; set; }
        public int Transoutput { get; set; }
        public string ManagerId { get; set; }
        public string ActionType { get; set; }
        public string SubmittedType { get; set; }
        public string SubmittedFlag { get; set; }
        public string ManagerID1 { get; set; }
        public string ManagerEmail1 { get; set; }
        public string ManagerID2 { get; set; }
        public string ManagerEmail2 { get; set; }
        public string AccManagerEmail { get; set; }
        public string AccManagerID { get; set; }
        public string UserEmailId { get; set; }
        public Nullable<System.DateTime> SubmittedDate { get; set; }
        public string L1ApproverStatus { get; set; }
        public string L2ApproverStatus { get; set; }
        public Nullable<System.DateTime> L1_ApproverDate { get; set; }
        public Nullable<System.DateTime> L2_ApproverDate { get; set; }
        public Nullable<System.DateTime> L1_RejectedDate { get; set; }
        public Nullable<System.DateTime> L2_RejectedDate { get; set; }
        public string Position { get; set; }
        public string Message { get; set; }
        public string SubmittedState { get; set; }
        public string ManagerName1 { get; set; }
        public string ManagerName2 { get; set; }
    }


    //public class timesheet
    //{
    //    public string UserName { get; set; }
    //    public int TimesheetID { get; set; }
    //    public int UserID { get; set; }
    //    public int EmpUsrID { get; set; }
    //    public string TaskDate { get; set; }
    //    public string Comments { get; set; }
    //    public int ProjectID { get; set; }
    //    public System.DateTime CreatedDate { get; set; }
    //    public string TaskId { get; set; }
    //    public int FlagEmailStatus { get; set; }
    //    public int Transoutput { get; set; }
    //    public string ManagerId { get; set; }
    //    public string ActionType { get; set; }
    //    public string SubmittedType { get; set; }
    //    public string SubmittedFlag { get; set; }
    //    public string ManagerEmail1 { get; set; }
    //    public string ManagerEmail2 { get; set; }
    //    public string AccManagerEmail { get; set; }
    //    public string AccManagerID { get; set; }
    //    public string UserEmailId { get; set; }
    //    public Nullable<System.DateTime> SubmittedDate { get; set; }
    //    public string L1ApproverStatus { get; set; }
    //    public string L2ApproverStatus { get; set; }
    //    public Nullable<System.DateTime> L1_ApproverDate { get; set; }
    //    public Nullable<System.DateTime> L2_ApproverDate { get; set; }
    //    public Nullable<System.DateTime> L1_RejectedDate { get; set; }
    //    public Nullable<System.DateTime> L2_RejectedDate { get; set; }
    //    public string Position { get; set; }
    //    public string Message { get; set; }
    //    public string SubmittedState { get; set; }

    //}
    public class listtimesheetdetail
    {

        public int projectid { get; set; }
        public int taskid { get; set; }
        public int hoursWorked { get; set; }
        public string taskDate { get; set; }
        public int taskDay { get; set; }

    }

}