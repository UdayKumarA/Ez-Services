using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class Userworkfromhome
    {
        public int UserwfhID { get; set; }
        public int UserID { get; set; }
        public string UserWfhStartDate { get; set; }
        public string UserWfhEndDate { get; set; }
        public string Comments { get; set; }
        public int StatuID { get; set; }
        public bool IsWorkfromhome { get; set; }

    }
}