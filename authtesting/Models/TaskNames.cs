using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{

    public class TaskNames
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<Tasks> taskNames { get; set; }

    }
    public class Tasks
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }

    }

}