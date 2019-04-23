using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class UserProperties
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Position { get; set; }
        public string Team { get; set; }
    }
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Stateid { get; set; }
    }
}