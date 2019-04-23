using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class ProjSpecificTaskEntity
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<ProjSpecificTasks> projSpecificTasks { get; set; }
    }

    public class ProjSpecificTasks
    {
        public int Proj_SpecificTaskId { get; set; }
        public string Proj_SpecificTaskName { get; set; }
    }

}