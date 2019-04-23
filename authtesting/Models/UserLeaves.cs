using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class UserLeaves
    {
        public userLeaveData applyLeave { get; set; }
        public List<lstUserLeaveTypeConsumed> leavesConsumed { get; set; }
        public AddLeaveStatus addLeaveStatus { get; set; }
    }

    public class userLeaveData
    {
        public int Usrl_LeaveId { get; set; }
        public int Usrl_UserId { get; set; }
        public string LeaveStartDate { get; set; }
        public string LeaveEndDate { get; set; }
        public string Comments { get; set; }
    }

    public class lstUserLeaveTypeConsumed
    {
        public int Usrlt_TypeConsumeId { get; set; }
        public int Usrl_LeaveId { get; set; }
        public int LSchm_LeaveSchemeID { get; set; }
        public int LeaveTypeId { get; set; }
        public int Accountid { get; set; }
        public int No_Of_Days { get; set; }
    }

    public class AddLeaveStatus
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public AddStatus appliedStatus { get; set; }
    }

    public class AddStatus
    {
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }

}