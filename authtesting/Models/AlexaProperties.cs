using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace authtesting.Models
{
    public class Authentication
    {
        public int Userid { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
       
    }
    public class HolidayCalender
    {
        public string Holidayname { get; set; }
        public string HolidayDate { get; set; }
    }
   
    public class GetHolidays
    {
        public List<HolidayCalender> lstholidays { get; set; }
        public int HolidaysCount { get; set; }
        public string FirstName { get; set; }
        public int Workingdays { get; set; }
        public int Workinghours { get; set; }
        public string TimesheetMonth { get; set; }
        //public int Status { get; set; }
       
    }
   
    public class AuthBody
    {
        public int Userid { get; set; }
    }
    public class AuthBody1
    {
       // public string Emailid { get; set; }
        
        public string deviceid { get; set; }
    }
    public class HolidaysList
    {
      
        public int Month { get; set; }
        public string Deviceid { get; set; }
       // public string Phonenumber { get; set; }
        //public int Pin { get; set; }
    }
    public class HelloMessage
    {
        public ResponseCodes Message { get; set; }
        //public string Lastname { get; set; }
    }
    public class HelloMessage1
    {
        public string Message { get; set; }
        //public string Lastname { get; set; }
    }
    public class ValidatePhoneClass
    {
        public string Phone { get; set; }
        public ResponseCodes Flag { get; set; }
        //public string Lastname { get; set; }
    }
    //sarat
    public class Validatepinclass
    {
        public int pin { get; set; }
        public ResponseCodes Flag { get; set; }
        //public string Lastname { get; set; }
    }
    public class StatusFlags
    {
        public string Response { get; set; }
    }
    public class Uservalidatepin
    {
        public string DeviceId { get; set; }
        public int pin { get; set; }
       // public string Passcode { get; set; }
    }

    public class SubmitTimesheet
    {
        //public int TimesheetSubmitted { get; set; }
        public int Timesheetstatus { get; set; }
        public int GetWorkingHours { get; set; }
      
       // public int TimesheetID { get; set; }
        //public int ProjectID { get; set; }
        //public int TaskID { get; set; }
        //public int TotalMonthDays { get; set; }
    }

    public class Devices
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
    }
    public class Uservalidate
    {
        public string DeviceId { get; set; }
        public string Phonenumber { get; set; }
        public string Passcode { get; set; }
    }
    public enum ResponseCodes
    {
        Exits=1,
        NotExists=0
    }
    //public class AlexaMessages
    //{
    //    public string Message { get; set; }
    //    public ResponseCodes Responsess { get; set; }
    //}
    public class FetchUsers
    {
        public string Name { get; set; }
        //public string Passcode { get; set; }
        public string Mobilenumber { get; set; }
         public int Status { get; set; }
    }
    


}