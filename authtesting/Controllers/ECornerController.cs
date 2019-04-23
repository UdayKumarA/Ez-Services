using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Description;
using authtesting.Models;
using System.Web.Http.Cors;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace evolCorner.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ECornerController : ApiController
    {
        #region instance declartions
        string str = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        ETimesheet objevolTime = new ETimesheet();
        HttpResponseMessage response = new HttpResponseMessage();

        #endregion



        #region PostLogin
        [HttpPost]
        [Route("api/ECorner/UserAuth")]
        public HttpResponseMessage UserAuth([FromBody]Login logindetails)
        {
            Logindetails objLogindetails = new Logindetails();

            try
            {
                objLogindetails = objevolTime.userAuth(logindetails);
                if (objLogindetails.EmpInfobyLogin != null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objLogindetails);
                    objLogindetails.StatusCode = 200;
                    objLogindetails.StatusMessage = "OK";

                }
                else if (objLogindetails.EmpInfobyLogin == null && objLogindetails.StatusCode != 500)
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, objLogindetails);
                    objLogindetails.StatusCode = 404;
                    objLogindetails.StatusMessage = "Not Found";

                }

                else if (objLogindetails.StatusCode == 500)
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, objLogindetails);
                    objLogindetails.StatusCode = 500;
                    objLogindetails.StatusMessage = objLogindetails.StatusMessage;
                }

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objLogindetails);
                objLogindetails.StatusCode = 500;
                objLogindetails.StatusMessage = ex.Message.ToString();

                //objLogindetails.status = new Status
                //{
                //    StatusCode = Convert.ToInt32(response.StatusCode),
                //    StatusMessage = ex.InnerException.ToString(),
                //};

            }
            return response;

        }
        #endregion




        #region PostLogin
        [HttpPost]
        [Route("api/ECorner/UserAuthenticate")]
        public HttpResponseMessage UserAuthenticate([FromBody]Login logindetails)
        {
            Logindetails objLogindetails = new Logindetails();

            try
            {
                objLogindetails = objevolTime.UserAuthenticateusingToken(logindetails);
                if (objLogindetails.EmpInfobyLogin.Trans_Output == 1)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objLogindetails);
                    objLogindetails.StatusCode = 200;
                    objLogindetails.StatusMessage = "OK";

                }
                else if (objLogindetails.EmpInfobyLogin.Trans_Output != 1 && objLogindetails.StatusCode != 500)
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, objLogindetails);
                    objLogindetails.StatusCode = 404;
                    objLogindetails.StatusMessage = "Not Found";

                }

                else if (objLogindetails.StatusCode == 500)
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, objLogindetails);
                    objLogindetails.StatusCode = 500;
                    objLogindetails.StatusMessage = objLogindetails.StatusMessage;
                }

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objLogindetails);
                objLogindetails.StatusCode = 500;
                objLogindetails.StatusMessage = ex.Message.ToString();

                //objLogindetails.status = new Status
                //{
                //    StatusCode = Convert.ToInt32(response.StatusCode),
                //    StatusMessage = ex.InnerException.ToString(),
                //};

            }
            return response;

        }
        #endregion




        #region GetddlTaskNames
        [HttpPost]
        [Route("api/ECorner/GetTaskList/{UserID}")]
        public HttpResponseMessage GetTaskList(int UserID)
        {
            TaskNames gettasklist = new TaskNames();
            try
            {
                gettasklist = objevolTime.getTaskList(UserID);
                if (gettasklist.taskNames.Count > 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, gettasklist);
                    gettasklist.StatusCode = Convert.ToInt32(response.StatusCode);
                    gettasklist.StatusMessage = "OK";

                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, gettasklist);
                    gettasklist.StatusCode = Convert.ToInt32(response.StatusCode);
                    gettasklist.StatusMessage = "NotFound";
                }

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, gettasklist);
                gettasklist.StatusCode = Convert.ToInt32(response.StatusCode);
                gettasklist.StatusMessage = ex.InnerException.ToString();
            }

            return response;
        }
        #endregion

        #region GetProjectList
        [HttpGet]
        [ActionName("GetProjectList")]
        public HttpResponseMessage GetProjects()
        {

            ProjectNames getprojetcs = new ProjectNames();

            try
            {
                getprojetcs = objevolTime.getProjectList();
                if (getprojetcs.projectName.Count > 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, getprojetcs);
                    getprojetcs.StatusCode = Convert.ToInt32(response.StatusCode);
                    getprojetcs.StatusMessage = "OK";
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, getprojetcs);
                    getprojetcs.StatusCode = Convert.ToInt32(response.StatusCode);
                    getprojetcs.StatusMessage = "NotFound";
                }

            }
            catch (Exception ex)
            {

                string message = ex.Message;

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, getprojetcs);
                getprojetcs.StatusCode = Convert.ToInt32(response.StatusCode);
                getprojetcs.StatusMessage = ex.InnerException.ToString();

            }

            return response;
        }
        #endregion


        #region GetProjSpecificTasks
        [HttpGet]
        [ActionName("GetProjSpecificTasks")]
        public HttpResponseMessage GetProjSpecificTasks()
        {
            ProjSpecificTaskEntity lstgetProjSpecificTasks = new ProjSpecificTaskEntity();
            try
            {
                lstgetProjSpecificTasks = objevolTime.GetProjSpecificList();
                if (lstgetProjSpecificTasks.projSpecificTasks.Count > 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, lstgetProjSpecificTasks);
                    lstgetProjSpecificTasks.StatusCode = Convert.ToInt32(response.StatusCode);
                    lstgetProjSpecificTasks.StatusMessage = "OK";
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, lstgetProjSpecificTasks);
                    lstgetProjSpecificTasks.StatusCode = Convert.ToInt32(response.StatusCode);
                    lstgetProjSpecificTasks.StatusMessage = "NotFound";
                }

            }
            catch (Exception ex)
            {

                string message = ex.Message;

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, lstgetProjSpecificTasks);
                lstgetProjSpecificTasks.StatusCode = Convert.ToInt32(response.StatusCode);
                lstgetProjSpecificTasks.StatusMessage = ex.InnerException.ToString();


            }

            return response;
        }
        #endregion


        #region  InsertTimesheetSubmission
        [HttpPost]
        [Route("api/ECorner/AddSubmitTimeSheet")]
        public HttpResponseMessage AddSubmitTimeSheet([FromBody]TotalTimeSheetTimeDetails sheetObj)
        {
            TotalTimeSheetTimeDetails objAddTimesheet = new TotalTimeSheetTimeDetails();

            try
            {
                objAddTimesheet = objevolTime.addSubmitTimeSheet(sheetObj);
                if (objAddTimesheet.addTimesheetStatus.Transoutput > 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objAddTimesheet.addTimesheetStatus);

                    objAddTimesheet.addTimesheetStatus.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "OK",
                    };
                }


                else if (objAddTimesheet.addTimesheetStatus.Transoutput == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, objAddTimesheet.addTimesheetStatus);
                    objAddTimesheet.addTimesheetStatus.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "BadRequest",
                    };

                }
            }


            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objAddTimesheet.status);

            }
            return response;




        }
        #endregion


        //#region  InsertTimesheetSubmission
        //[HttpPost]
        //[Route("api/ECorner/AddSubmitTimeSheet")]
        //public HttpResponseMessage AddSubmitTimeSheet([FromBody]TotalTimeSheetTimeDetails sheetObj)
        //{
        //    TotalTimeSheetTimeDetails objAddTimesheet = new TotalTimeSheetTimeDetails();
        //    try
        //    {
        //        objAddTimesheet = objevolTime.addSubmitTimeSheet(sheetObj);
        //        if (objAddTimesheet.timesheets.Transoutput == 104)
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.OK, objAddTimesheet);
        //            objAddTimesheet.StatusCode = Convert.ToInt32(response.StatusCode);
        //            objAddTimesheet.StatusMessage = "OK";


        //        }

        //        if (objAddTimesheet.timesheets.Transoutput == 1)
        //        {
        //            if (objAddTimesheet.timesheets.SubmittedType == "Submit")
        //            {
        //                response = Request.CreateResponse(HttpStatusCode.OK, objAddTimesheet);
        //                objAddTimesheet.StatusCode = Convert.ToInt32(response.StatusCode);
        //                objAddTimesheet.StatusMessage = "OK";

        //            }
        //            else if (objAddTimesheet.timesheets.SubmittedType == "Save")
        //            {
        //                response = Request.CreateResponse(HttpStatusCode.OK, objAddTimesheet);
        //                objAddTimesheet.StatusCode = Convert.ToInt32(response.StatusCode);
        //                objAddTimesheet.StatusMessage = "OK";

        //            }
        //        }
        //        if (objAddTimesheet.timesheets.Transoutput == 0)
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, objAddTimesheet);
        //            objAddTimesheet.StatusCode = Convert.ToInt32(response.StatusCode);
        //            objAddTimesheet.StatusMessage = "BadRequest";
        //        }

        //    }
        //    catch (System.Exception ex)
        //    {

        //        response = Request.CreateResponse(HttpStatusCode.InternalServerError, objAddTimesheet);
        //        objAddTimesheet.StatusCode = Convert.ToInt32(response.StatusCode);
        //        objAddTimesheet.StatusMessage = ex.InnerException.ToString();

        //    }
        //    return response;




        //}
        //#endregion


        #region  EditTimeSheetDetails
        [HttpPost]
        [Route("api/ECorner/EditTimeSheetDetails/{TimeSheetId}")]
        public HttpResponseMessage EditTimeSheetList(int TimeSheetId, [FromBody]TotalTimeSheetTimeDetails EditsheetObj)
        {
            TotalTimeSheetTimeDetails objEditTimesheet = new TotalTimeSheetTimeDetails();
            AddTimesheetStatus objstatus = new AddTimesheetStatus();

            try
            {
                objEditTimesheet = objevolTime.EditTimeSheetList(TimeSheetId, EditsheetObj);

                if (objEditTimesheet.addTimesheetStatus.Transoutput > 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objEditTimesheet.addTimesheetStatus);

                    objEditTimesheet.addTimesheetStatus.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "OK",
                    };
                }


                else if (objEditTimesheet.addTimesheetStatus.Transoutput == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, objEditTimesheet.addTimesheetStatus);
                    objEditTimesheet.addTimesheetStatus.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "BadRequest",
                    };

                }

            }
            catch (System.Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objEditTimesheet.status);


                objEditTimesheet.status = new Status
                {
                    StatusCode = 500,
                    StatusMessage = ex.Message.ToString(),
                };
            }
            return response;
        }
        #endregion



        #region GetUser TimeSheetforApproval 
        [HttpPost]
        // [Route("api/ECorner/{userid}/{fromdate}/{enddate}")]
        [Route("api/ECorner/GetUserTimeSheetforApproval")]
        public HttpResponseMessage GetUserTimeSheetforApproval([FromBody] TimesheetDetails TimeSheetobj)
        {
            ManagerDetails objmanagerApproval = new ManagerDetails();
            try
            {
                objmanagerApproval = objevolTime.GetUserTimeSheetforApproval(TimeSheetobj.userid, TimeSheetobj.startposition, TimeSheetobj.endPosition);

                if ((objmanagerApproval.timesheetsforapproval.Count > 0) || (objmanagerApproval.mytimesheets.Count > 0))
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objmanagerApproval);


                    objmanagerApproval.StatusCode = Convert.ToInt32(response.StatusCode);
                    objmanagerApproval.StatusMessage = "OK";
                }

                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, objmanagerApproval);
                    objmanagerApproval.StatusCode = Convert.ToInt32(response.StatusCode);
                    objmanagerApproval.StatusMessage = "NotFound";
                }
            }
            catch (System.Exception ex)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objmanagerApproval);
                objmanagerApproval.StatusCode = Convert.ToInt32(response.StatusCode);
                objmanagerApproval.StatusMessage = ex.InnerException.ToString();
            }
            return response;
        }
        #endregion



        #region Preview GetTimeSheetdetails by userid
        [HttpPost]
        [Route("api/ECorner/GetUserTimesheet/{UserID}/{TaskDate}")]
        public HttpResponseMessage GetUserTimesheet(string UserID, string TaskDate)
        {
            Timesheetlist objGetTimeSheetDetails = new Timesheetlist();
            try
            {
                objGetTimeSheetDetails = objevolTime.getUserTimesheet(UserID, TaskDate);

                if ((objGetTimeSheetDetails.timeSheetDetails.Count > 0) || (objGetTimeSheetDetails.timeSheetDetails != null))
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objGetTimeSheetDetails);
                    objGetTimeSheetDetails.StatusCode = Convert.ToInt32(response.StatusCode);
                    objGetTimeSheetDetails.StatusMessage = "OK";
                }

                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, objGetTimeSheetDetails);
                    objGetTimeSheetDetails.StatusCode = Convert.ToInt32(response.StatusCode);
                    objGetTimeSheetDetails.StatusMessage = "NotFound";
                }


            }
            catch (System.Exception ex)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objGetTimeSheetDetails);
                objGetTimeSheetDetails.StatusCode = Convert.ToInt32(response.StatusCode);
                objGetTimeSheetDetails.StatusMessage = ex.InnerException.ToString();

            }


            return response;
        }
        #endregion

        #region ChangePassword
        [HttpPost]
        [Route("api/ECorner/ChangePassword")]
        public HttpResponseMessage ChangeorUpdatePassword([FromBody]Login logindetails)
        {
            ChangePassworddetails objChangePassword = new ChangePassworddetails();

            //UserAuth obj = new UserAuth();

            try
            {
                objChangePassword = objevolTime.ChangeorUpdatePassword(logindetails);

                if (objChangePassword.StatusCode == 1)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objChangePassword);
                    objChangePassword.StatusCode = Convert.ToInt32(response.StatusCode);
                    objChangePassword.StatusMessage = "OK";

                    objChangePassword.message = new Message
                    {
                        message = "Password Changed Sucessfully",
                    };
                }
                else if (objChangePassword.StatusCode == 2)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, objChangePassword);

                    objChangePassword.StatusCode = Convert.ToInt32(response.StatusCode);
                    objChangePassword.StatusMessage = "BadRequest";

                    objChangePassword.message = new Message
                    {
                        message = "Please Enter Old Password to Change New Password",
                    };

                }

                else if (objChangePassword.StatusCode == 3)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, objChangePassword);

                    objChangePassword.StatusCode = Convert.ToInt32(response.StatusCode);
                    objChangePassword.StatusMessage = "BadRequest";

                    objChangePassword.message = new Message
                    {
                        message = "New Password should not be empty",
                    };

                }

            }
            catch (Exception ex)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objChangePassword);


                objChangePassword.StatusCode = Convert.ToInt32(response.StatusCode);
                objChangePassword.StatusMessage = ex.InnerException.ToString();

            }
            return response;

        }
        #endregion



        #region  TimeSheetManagerActions for Aprrove and Reject Timesheet Api 
        [HttpPost]
        [Route("api/ECorner/TimeSheetManagerActions")]
        public HttpResponseMessage TimeSheetManagerActions([FromBody]TotalTimeSheetInfo sheetObj)
        {
            //TotalTimeSheetInfo objTimeSheet = new TotalTimeSheetInfo();

            sheetObj = objevolTime.TimeSheetManagerActions(sheetObj);
            try
            {
                if (sheetObj.timeSheetActions.Transoutput == 1)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, sheetObj);
                    sheetObj.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "OK",
                    };
                }

                if (sheetObj.timeSheetActions.Transoutput == 2)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, sheetObj);
                    sheetObj.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "OK",
                    };


                }
                if (sheetObj.timeSheetActions.Transoutput == 3)
                {

                    response = Request.CreateResponse(HttpStatusCode.OK, sheetObj);
                    sheetObj.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "OK",
                    };

                }
                if (sheetObj.timeSheetActions.Transoutput == 4)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, sheetObj);
                    sheetObj.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "OK",
                    };

                }
                if (sheetObj.timeSheetActions.Transoutput == 104)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, sheetObj);
                    sheetObj.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "OK",
                    };

                }
                if (sheetObj.timeSheetActions.Transoutput == 105)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, sheetObj);
                    sheetObj.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "OK",
                    };

                }
                if (sheetObj.timeSheetActions.Transoutput == 900)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, sheetObj);
                    sheetObj.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "OK",
                    };

                }


                if (sheetObj.timeSheetActions.Transoutput == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, sheetObj);
                    sheetObj.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "BadRequest",
                    };

                }

            }
            catch (System.Exception ex)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, sheetObj);


            }
            return response;

        }
        #endregion



        #region  HolidayCalender based on Month and Year
        [HttpPost]
        [Route("api/ECorner/HolidayCalenderList/{UserID}/{AccountID}/{HolidaySelectionDate}")]
        public HttpResponseMessage HolidayCalenderList(String UserID, String AccountID, String HolidaySelectionDate)
        {

            HolidayListEntitiy objCalenderList = new HolidayListEntitiy();

            try
            {
                objCalenderList = objevolTime.GetCalenderList(UserID, AccountID, HolidaySelectionDate);
                if (objCalenderList.holidayDetails.Count > 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objCalenderList);
                    objCalenderList.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "OK",
                    };

                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, objCalenderList.status);
                    objCalenderList.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "NotFound",
                    };

                }


            }
            catch (System.Exception ex)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objCalenderList);
                objCalenderList.status = new Status
                {
                    StatusCode = Convert.ToInt32(response.StatusCode),
                    StatusMessage = ex.InnerException.ToString(),
                };

            }
            return response;
        }
        #endregion




        #region  AddOrEdit ProfileDetails
        [HttpPost]
        [Route("api/ECorner/AddOrEditUserProfileByUser")]
        public HttpResponseMessage EditUserProfile([FromBody]UserProfileDetails AddOrEditProfileObj)
        {
            UserProfileDetails objProfileDetails = new UserProfileDetails();
            try
            {
                objProfileDetails = objevolTime.AddOrEditUsersProfilebyUser(AddOrEditProfileObj);
                if (objProfileDetails.Trans_Output == 1)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objProfileDetails.status);
                    objProfileDetails.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "Profile Submitted Sucessfully",
                    };


                }

                if (objProfileDetails.Trans_Output == 2)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objProfileDetails.status);
                    objProfileDetails.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "Profile Updated Sucessfully",
                    };


                }
                if (objProfileDetails.Trans_Output == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, objProfileDetails.status);
                    objProfileDetails.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "Failed",
                    };

                }

            }
            catch (System.Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objProfileDetails.status);
                objProfileDetails.status = new Status
                {
                    StatusCode = Convert.ToInt32(response.StatusCode),
                    StatusMessage = ex.InnerException.ToString(),
                };

            }
            return response;
        }
        #endregion



        ///Users Profile Services//////
        #region GetUserProfileDetails by userid
        [HttpPost]
        [Route("api/ECorner/GetUserProfileDetails/{UserId}")]
        public HttpResponseMessage GetUserProfileDetails(string Userid)
        {
            UserProfilesInfo objUserPro = new UserProfilesInfo();
            try
            {
                objUserPro = objevolTime.getUserProfileDetails(Userid);
                if (objUserPro.Userprofilesdata.UsrP_UserID > 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objUserPro);
                    objUserPro.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "OK",
                    };


                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, objUserPro.status);
                    objUserPro.status = new Status
                    {
                        StatusCode = Convert.ToInt32(response.StatusCode),
                        StatusMessage = "NotFound",
                    };

                }


            }
            catch (System.Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objUserPro.status);
                objUserPro.status = new Status
                {
                    StatusCode = 500,
                    StatusMessage = ex.InnerException.ToString(),
                };

            }

            return response;


        }
        #endregion



        #region GetEmpLeaveCategories
        [HttpPost]
        [Route("api/ECorner/GetEmpLeaveCategories/{UserId}/{Year}")]
        public HttpResponseMessage GetEmpLeaveCategories(string UserId, string Year)
        {
            LeaveCategoriesEntity objLeaveCategoriesList = new LeaveCategoriesEntity();
            try
            {
                objLeaveCategoriesList = objevolTime.getEmpLeaveCategories(UserId, Year);
                if ((objLeaveCategoriesList.EmpLeaveCategory.Count > 0))
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objLeaveCategoriesList);
                    objLeaveCategoriesList.StatusCode = Convert.ToInt32(response.StatusCode);
                    objLeaveCategoriesList.StatusMessage = "OK";
                }

                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, objLeaveCategoriesList);
                    objLeaveCategoriesList.StatusCode = Convert.ToInt32(response.StatusCode);
                    objLeaveCategoriesList.StatusMessage = "NotFound";
                }


            }
            catch (System.Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objLeaveCategoriesList);
                objLeaveCategoriesList.StatusCode = Convert.ToInt32(response.StatusCode);
                objLeaveCategoriesList.StatusMessage = ex.InnerException.ToString();
            }
            return response;
        }
        #endregion



        


        ///////////////////////////////UserLeaves By Tharak///////////////////////////////


        //#region  InsertTimesheetSubmission
        //[HttpPost]
        //[Route("api/ECorner/AddUserLeaves")]
        //public HttpResponseMessage AddUserLeaves([FromBody]UserLeaves UserLeavesObj)
        //{
        //    UserLeaves objUserLeaves = new UserLeaves();

        //    try
        //    {
        //        objUserLeaves = objevolTime.addUserLeaves(UserLeavesObj);
        //        if (objUserLeaves.addLeaveStatus.appliedStatus.ErrorMessage == null && objUserLeaves.addLeaveStatus.appliedStatus.Message != null)
        //        {
        //            if (objUserLeaves.addLeaveStatus.appliedStatus.Message == "Leaves Submitted Sucessfully")
        //            {
        //                response = Request.CreateResponse(HttpStatusCode.OK, objUserLeaves.addLeaveStatus);

        //                objUserLeaves.addLeaveStatus.StatusCode = Convert.ToInt32(response.StatusCode);
        //                objUserLeaves.addLeaveStatus.StatusMessage = "OK";

        //            }
        //        }

        //        else if (objUserLeaves.addLeaveStatus.appliedStatus.ErrorMessage == "User Not Found")
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.NotFound,objUserLeaves.addLeaveStatus);

        //            objUserLeaves.addLeaveStatus.StatusCode = Convert.ToInt32(response.StatusCode);
        //            objUserLeaves.addLeaveStatus.StatusMessage = "User Not Found";

        //        }
        //        else if (objUserLeaves.addLeaveStatus.appliedStatus.ErrorMessage == "Already Exists")
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.OK, objUserLeaves.addLeaveStatus);

        //            objUserLeaves.addLeaveStatus.StatusCode = Convert.ToInt32(response.StatusCode);
        //            objUserLeaves.addLeaveStatus.StatusMessage = "OK";

        //        }
        //        else if (objUserLeaves.addLeaveStatus.appliedStatus.ErrorMessage == "LeaveTypeConsumed days are empty")
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.OK, objUserLeaves.addLeaveStatus);

        //            objUserLeaves.addLeaveStatus.StatusCode = Convert.ToInt32(response.StatusCode);
        //            objUserLeaves.addLeaveStatus.StatusMessage = "OK";
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        response = Request.CreateResponse(HttpStatusCode.InternalServerError, objUserLeaves.addLeaveStatus);

        //    }
        //    return response;
        //}
        //#endregion

        #region  InsertTimesheetSubmission
        [HttpPost]
        [Route("api/ECorner/AddUserLeaves")]
        public HttpResponseMessage AddUserLeaves([FromBody]userLeaveData UserLeavesObj)
        {
            UserLeaves objUserLeaves = new UserLeaves();

            try
            {
                objUserLeaves = objevolTime.addUserLeaves(UserLeavesObj);
                if (objUserLeaves.addLeaveStatus.appliedStatus.Message == "Leaves Submitted Sucessfully")
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objUserLeaves.addLeaveStatus);


                    objUserLeaves.addLeaveStatus.StatusCode = Convert.ToInt32(response.StatusCode);
                    objUserLeaves.addLeaveStatus.StatusMessage = "OK";

                }


                else if (objUserLeaves.addLeaveStatus.appliedStatus.Message == "Invalid UserID")
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objUserLeaves.addLeaveStatus);




                    objUserLeaves.addLeaveStatus.StatusCode = Convert.ToInt32(response.StatusCode);
                    objUserLeaves.addLeaveStatus.StatusMessage = "OK";


                }
            }


            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objUserLeaves.addLeaveStatus);

            }
            return response;




        }
        #endregion


        //#region  GetUserLeaves
        //[HttpPost]
        //[Route("api/ECorner/GetUserLeaves")]

        //#endregion

        #region User Leaves based on Year
        [HttpPost]
        [Route("api/ECorner/UserLeavesInfo/{UserID}/{DateofMonth}")]
        public HttpResponseMessage ApprovedUserLeaves(String UserID, String DateofMonth)
        {
            UserLeavesApprovedEntites objLeavesList = new UserLeavesApprovedEntites();
            try
            {
                objLeavesList = objevolTime.GetUserLeavesList(UserID, DateofMonth);
                if (objLeavesList.StatusCode == 200)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objLeavesList);
                    objLeavesList.StatusCode = Convert.ToInt32(response.StatusCode);
                    objLeavesList.StatusMessage = "OK";
                }

                else if ((objLeavesList.TeamLeaveDetails.Count > 0) || (objLeavesList.myLeaveDetails.Count > 0) || (objLeavesList.StatusCode != 200))
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, objLeavesList);
                    objLeavesList.StatusCode = Convert.ToInt32(response.StatusCode);
                    objLeavesList.StatusMessage = "OK";
                }
            }
            catch (System.Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, objLeavesList);
                objLeavesList.StatusCode = Convert.ToInt32(response.StatusCode);
                objLeavesList.StatusMessage = ex.InnerException.ToString();
            }
            return response;
        }
        #endregion
    }
}
