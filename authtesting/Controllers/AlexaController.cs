using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Alexa.NET.Request;
using authtesting.Models;
using log4net;

namespace authtesting.Controllers
{
    [EnableCors("*", "*", "*")]
    public class AlexaController : ApiController
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger("AlexaController");

        AlexaAuthentication objalexaauth = new AlexaAuthentication();
        // GET api/<controller>
        [HttpPost]
        public HttpResponseMessage AlexaGetAuth([FromBody] AuthBody auth)
        {
            List<Authentication> lstauth = new List<Authentication>();
            try
            {
                lstauth = objalexaauth.GetAuthUsers(auth.Userid);
                if(lstauth!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,lstauth);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,"Data Not Found");
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
        [HttpPost]
        public HttpResponseMessage AlexaGetAuthUsers([FromBody] AuthBody1 auth)
        {
            Authentication lstauth = new Authentication();
            log.InfoFormat("AuthUser :: DeviceId : {0}", auth.deviceid);
            try
            {
                lstauth = objalexaauth.GetUsers(auth.deviceid);
                if (lstauth != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, lstauth);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Data Not Found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
        [HttpGet]
        public HttpResponseMessage GetHelloMessage()
        {
            HelloMessage1 objmessage = new HelloMessage1();
            objmessage.Message = "Hello Evolutyz!!!";
            
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, objmessage);
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
        [HttpPost]
        public HttpResponseMessage AlexaGetTimeSheet([FromBody] HolidaysList list)
        {
            GetHolidays objgetholidays = new GetHolidays();
            log.InfoFormat("GetUserTimeSheets :: DeviceId : {0}", list.Deviceid);

            try
            {
                objgetholidays = objalexaauth.GetHours(list.Month,list.Deviceid /*,list.Pin*/);
                if(objgetholidays!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, objgetholidays);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Data Not Found");
                }
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
          
        }
        [HttpPost]
        public HttpResponseMessage AlexaSubmittedTimesheet([FromBody] HolidaysList list)
        {
            SubmitTimesheet objtimesheet = new SubmitTimesheet();
            log.InfoFormat("GetUserSubmittedTimeSheets :: DeviceId : {0}", list.Deviceid);

            try
            {
                objtimesheet = objalexaauth.GetTimeSheet(list.Month,list.Deviceid /*,list.Pin*/);
                if (objtimesheet != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, objtimesheet);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Data Not Found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
        [HttpPost]
        public HttpResponseMessage AlexaCheckStatus([FromBody] AuthBody1 list)
        {
            StatusFlags objflags = new StatusFlags();
            log.InfoFormat("AlexaCheckStatus :: DeviceId : {0}", list.deviceid);

            try
            {
                objflags = objalexaauth.CheckStatus(list.deviceid);
                if (objflags != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, objflags);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Data Not Found");
                }
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }

        }
       
        [HttpPost]
        public HttpResponseMessage uploadImage([FromBody] uploadimage image)
        {
            var request = HttpContext.Current.Request;

            if (Request.Content.IsMimeMultipartContent())
            {
                if (request.Files.Count > 0)
                {
                    var postedFile = request.Files.Get("file");
                    var title = request.Params["title"];
                    string root = "http://192.168.75.19:5001/Content/images/CompanyLogos/"+ image.filename;

                    //string root = HttpContext.Current.Server.MapPath("~/Images");
                    root = root + "/" + postedFile.FileName;
                    postedFile.SaveAs(root);
                    //Save post to DB
                    return Request.CreateResponse(HttpStatusCode.Found, new
                    {
                        error = false,
                        status = "created",
                        path = root
                    });

                }
            }

            return null;
        }
        //[HttpPost]
        //public string FunctionHandler(SkillRequest skillReqParam)
        //{
        //    var accessToken = skillReqParam.Context.System.User.AccessToken;
        //    var decryptAccessToken = Startup.OAuthOptions.AccessTokenFormat.Unprotect(accessToken);
        //    var userEmail = decryptAccessToken.Identity.Name;
        //    return userEmail;

        //}

        [HttpPost]
        public HttpResponseMessage UpdateDeviceId([FromBody] Devices request)
        {
            Devices device = new Devices();

            device = objalexaauth.InsertDeviceId(request.DeviceId);
           
            if (device != null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.OK, "Success");
            }
            else
            return Request.CreateErrorResponse(HttpStatusCode.OK, "Invalid request");
        }

        [HttpPut]
        public HttpResponseMessage AlexaWakeUp([FromBody] Devices request)
        {
            FetchUsers objusers = new FetchUsers();

            // AlexaMessages objmessages = new AlexaMessages();

            try
            {
                objusers = objalexaauth.FetchData(request.DeviceId);

                if (objusers.Name != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, objusers);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,ResponseCodes.NotExists);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
                
        }
        [HttpPut]
        public HttpResponseMessage ValidateUser([FromBody] Uservalidate request)
        {
            HelloMessage objusers = new HelloMessage();

            // AlexaMessages objmessages = new AlexaMessages();

            try
            {
                objusers = objalexaauth.ValidateUser(request.DeviceId,request.Phonenumber/*,request.Passcode*/);

                if (objusers!= null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,ResponseCodes.Exits);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ResponseCodes.NotExists);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }

        }
        [HttpPut]
        public HttpResponseMessage validatePhone([FromBody] Uservalidate request)
        {
            ValidatePhoneClass objusers = new ValidatePhoneClass();

            // AlexaMessages objmessages = new AlexaMessages();

            try
            {
                objusers = objalexaauth.ValidatePhone(request.DeviceId, request.Phonenumber);

                if (objusers.Phone != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,objusers);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ResponseCodes.NotExists);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }

        }

        [HttpPut]
        public HttpResponseMessage validatepin([FromBody] Uservalidatepin request)
        {
            
            Validatepinclass objusers = new Validatepinclass();
            //log.InfoFormat("validatepin :: DeviceId : {0} & Some Name : {1}", request.DeviceId,null);
            log.InfoFormat("validatepin :: DeviceId : {0} ", request.DeviceId);


            // AlexaMessages objmessages = new AlexaMessages();
            try
            {
                objusers = objalexaauth.Validatepin(request.DeviceId,request.pin);

                if (objusers.pin !=0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, objusers);
                }
                else 
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ResponseCodes.NotExists);
                }
            }
            catch (Exception ex)
            {
               
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());

            }

        }

        [HttpPut]
        public HttpResponseMessage validatePasscode([FromBody] Uservalidate request)
        {
            HelloMessage objusers = new HelloMessage();

            // AlexaMessages objmessages = new AlexaMessages();

            try
            {
                objusers = objalexaauth.ValidatePasscode(request.DeviceId, request.Phonenumber,request.Passcode);

                if (objusers != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,objusers);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ResponseCodes.NotExists);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }

        }
    }


}