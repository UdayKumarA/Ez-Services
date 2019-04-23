using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class Logindetails
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        //public Status status { get; set; }
        public UserAuth EmpInfobyLogin { get; set; }

    }


    public class LogindetailsToken
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        //public Status status { get; set; }
        public List<UserAuth> EmpInfobyLoginTokens { get; set; }

    }


    public class ChangePassworddetails
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public Message message { get; set; }


    }

    public class Message
    {
        public string message { get; set; }
    }
    public class UserAuth
    {

        public int userid { get; set; }
        public int usertype { get; set; }
        public int projectID { get; set; }
        public int UsTUserTypeID { get; set; }
        public int TaskTypeID { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string projectName { get; set; }
        public string TaskName { get; set; }
        public string userName { get; set; }
        public string AccountID { get; set; }
        public string AccountType { get; set; }
        public string Companylogopath { get; set; }
        public string EmpId { get; set; }
        public string EmpEmailId { get; set; }
        public string DOJ { get; set; }
        public string EmpFirstName { get; set; }
        public string EmpLastName { get; set; }
        public int Trans_Output { get; set; }
        public string Message { get; set; }
    }





    public class Login
    {
        public string username { get; set; }
        public string password { get; set; }
        public string NewPassword { get; set; }
        public string newdeviceid { get; set; }
        public string token { get; set; }
        public string loginType { get; set; }


    }

    //public string deviceid { get; set; }
    //public string modifieddate { get; set; }
    //
}