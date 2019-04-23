using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class LeaveCategoriesEntity
    {

        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<LeaveCategoriesDetails> EmpLeaveCategory { get; set; }


    }


    public class LeaveCategoriesDetails
    {

        public string leaveType { get; set; }
        public string totLeavesCount { get; set; }
        public string totHolidaysUsed { get; set; }
        public int leaveTypeID { get; set; }

    }

    public class manageremails
    {
        public string manageremail { get; set; }
        public string managerid { get; set; }
        public string managername { get; set; }
    }

}