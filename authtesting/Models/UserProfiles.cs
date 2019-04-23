using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class UserProfiles
    {

        public UserProfileDetails userProfilesDetails { get; set; }
        // public UserProfilesInfo userProfilesInfo { get; set; }

    }

    public class UserProfileDetails
    {
        public Status status { get; set; }
        public int UsrP_UserProfileID { get; set; }
        public int UsrP_UserID { get; set; }
        public string UsrPFullName { get; set; }
        public string UsrFstName { get; set; }
        public string UsrLstName { get; set; }
        public string UsrP_EmployeeID { get; set; }
        public string UsrP_EmailID { get; set; }
        public string UsrP_MobNum { get; set; }
        public string UsrP_PhoneNumber { get; set; }
        public string UsrP_ProfilePicture { get; set; }
        public string UsrP_DOB { get; set; }
        public string UsrP_DOJ { get; set; }
        public int UsrP_ActiveStatus { get; set; }
        public string UsrP_CreatedDate { get; set; }
        public System.DateTime UsrP_ModifiedDate { get; set; }
        public int Trans_Output { get; set; }

    }

    public class UserProfilesInfo
    {

        public Status status { get; set; }

        public UserProfilesData Userprofilesdata { get; set; }

    }

    public class UserProfilesData
    {
        public int UsrP_UserProfileID { get; set; }
        public int UsrP_UserID { get; set; }
        public string UsrPFullName { get; set; }
        public string UsrP_EmployeeID { get; set; }
        public string UsrP_EmailID { get; set; }
        public string UsrP_MobNum { get; set; }
        public string UsrP_PhoneNumber { get; set; }
        public string UsrP_ProfilePicture { get; set; }
        public string UsrP_DOJ { get; set; }
    }
}