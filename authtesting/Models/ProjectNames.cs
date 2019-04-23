using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class ProjectNames
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<projects> projectName { get; set; }
    }
    public class projects
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }


    }
}