using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Web.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Security.Cryptography;
using System.Text;


namespace authtesting.Models
{
    public class ETimesheet
    {
        string str = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        public string htmlStr = "";
        public int timeSheetID = 0;
        SqlConnection Conn = new SqlConnection();
        SqlDataAdapter da = new SqlDataAdapter(); DataSet ds = new DataSet(); DataTable dt = new DataTable();
        List<AllUsersTimeSheetDetails> lsttimesheets = new List<AllUsersTimeSheetDetails>();
        EmailFormats objemail = new EmailFormats(); string StatusMsg = string.Empty;
        timesheet lstusers = new timesheet();
        DataSet ds1 = new DataSet(); DataTable dtheadings = new DataTable(); DataTable dtData = new DataTable();

        #region userAuth(GetUser for login Method without using firebase id) 
        public Logindetails userAuth(Login objlogin)
        {
            Logindetails objLogindetails = new Logindetails();
            Conn = new SqlConnection(str);
            try
            {
                // objLogindetails.EmpInfobyLogin = new UserAuth();
                // objLogindetails.Status = new List<statusMessage>();
                Conn.Open();
                SqlCommand cmd = new SqlCommand("[GetUsersDatabyLogin]", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", objlogin.username);
                cmd.Parameters.AddWithValue("@password", GetMD5(objlogin.password));

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                if (ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        objLogindetails.EmpInfobyLogin = new UserAuth();
                        {
                            objLogindetails.EmpInfobyLogin.userid = Convert.ToInt32(dr["Usr_UserID"]);
                            objLogindetails.EmpInfobyLogin.usertype = Convert.ToInt32(dr["Usr_UserTypeID"]);
                            objLogindetails.EmpInfobyLogin.UsTUserTypeID = Convert.ToInt32(dr["UsT_UserTypeID"]);
                            objLogindetails.EmpInfobyLogin.projectID = Convert.ToInt32(dr["Proj_ProjectID"]);
                            objLogindetails.EmpInfobyLogin.projectName = dr["Proj_ProjectName"].ToString();
                            objLogindetails.EmpInfobyLogin.TaskTypeID = Convert.ToInt32(dr["tsk_TaskID"]);
                            objLogindetails.EmpInfobyLogin.TaskName = dr["tsk_TaskName"].ToString();
                            objLogindetails.EmpInfobyLogin.userName = dr["Usr_Username"].ToString();
                            objLogindetails.EmpInfobyLogin.AccountType = dr["UsT_UserType"].ToString();
                            objLogindetails.EmpInfobyLogin.AccountID = dr["Acc_AccountID"].ToString();
                            objLogindetails.EmpInfobyLogin.Companylogopath = dr["Acc_CompanyLogo"].ToString();
                            objLogindetails.EmpInfobyLogin.RoleId = Convert.ToInt32(dr["Rol_RoleID"]);
                            objLogindetails.EmpInfobyLogin.RoleName = dr["Rol_RoleName"].ToString();
                            objLogindetails.EmpInfobyLogin.EmpId = dr["UsrP_EmployeeID"].ToString();
                            objLogindetails.EmpInfobyLogin.EmpEmailId = dr["UsrP_EmailID"].ToString();
                            objLogindetails.EmpInfobyLogin.DOJ = Convert.ToDateTime(dr["Usrp_DOJ"]).ToString("yyyy-MM-dd");
                            objLogindetails.EmpInfobyLogin.EmpFirstName = dr["UsrP_FirstName"].ToString();
                            objLogindetails.EmpInfobyLogin.EmpLastName = dr["UsrP_LastName"].ToString();
                            objLogindetails.EmpInfobyLogin.Trans_Output = 1;
                        }
                    };
                }
                else
                {
                    objLogindetails.StatusCode = 404;
                    objLogindetails.StatusMessage = "NotFound";
                }
            }
            catch (Exception ex)
            {
                //cls_Log objlog = new cls_Log();
                //objlog.AddtoLogFile("TimeSheetServices", "userAuth", ex.Message.ToString());

                //objLogindetails.status = new Status
                //{
                //    StatusCode = 500,
                //    StatusMessage = ex.Message.ToString(),
                //};
                objLogindetails.StatusCode = 500;
                objLogindetails.StatusMessage = ex.InnerException.ToString();
            }
            finally
            {
                Conn.Close();
            }
            return objLogindetails;
        }

        #endregion

        public string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                //lower  
                byte2String += targetData[i].ToString("x2");
                //upper  
                //byte2String += targetData[i].ToString("X2");  
            }
            return byte2String;
        }

        #region getTaskList  
        public TaskNames getTaskList(int UserID)
        {
            TaskNames getTaskNames = new TaskNames();
            Conn = new SqlConnection(str);
            try
            {
                getTaskNames.taskNames = new List<Tasks>();
                //  getTaskNames.Status = new List<statusMessage>();

                Conn.Open();
                SqlCommand cmd = new SqlCommand("[Sp_GetTasks]", Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Userid", UserID);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    getTaskNames.taskNames.Add(new Tasks
                    {
                        TaskId = Convert.ToInt32(dr["TaskId"]),
                        TaskName = dr["TaskName"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {
                cls_Log objlog = new cls_Log();
                objlog.AddtoLogFile("TimeSheetServices", "getTaskList", ex.Message.ToString());
            }
            finally
            {
                Conn.Close();
            }
            return getTaskNames;
        }
        #endregion


        #region getProjectList   
        public ProjectNames getProjectList()
        {
            ProjectNames getProject = new ProjectNames();
            System.Net.Http.HttpResponseMessage response = new System.Net.Http.HttpResponseMessage();

            Conn = new SqlConnection(str);
            try
            {
                getProject.projectName = new List<projects>();
                //  getProject.Status = new List<statusMessage>();

                Conn.Open();
                SqlCommand cmd = new SqlCommand("[Sp_GetProjects]", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    getProject.projectName.Add(new projects
                    {
                        ProjectId = Convert.ToInt32(dr["Proj_ProjectID"]),
                        ProjectName = dr["Proj_ProjectName"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {
                cls_Log objlog = new cls_Log();
                objlog.AddtoLogFile("TimeSheetServices", "getProjectList", ex.Message.ToString());
            }
            finally
            {
                Conn.Close();
            }
            return getProject;

        }
        #endregion

        #region getProjSpecificTaskList   
        public ProjSpecificTaskEntity GetProjSpecificList()
        {
            ProjSpecificTaskEntity objgetSpecificTasks = new ProjSpecificTaskEntity();


            Conn = new SqlConnection(str);
            try
            {
                objgetSpecificTasks.projSpecificTasks = new List<ProjSpecificTasks>();
                Conn.Open();
                SqlCommand cmd = new SqlCommand("[Sp_GetProjSpecificTasks]", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    objgetSpecificTasks.projSpecificTasks.Add(new ProjSpecificTasks
                    {
                        Proj_SpecificTaskId = Convert.ToInt32(dr["Proj_SpecificTaskId"]),
                        Proj_SpecificTaskName = dr["Proj_SpecificTaskName"].ToString(),
                    });

                }

            }
            catch (Exception ex)
            {
                cls_Log objlog = new cls_Log();
                objlog.AddtoLogFile("TimeSheetServices", "GetProjSpecificTasks", ex.Message.ToString());
            }
            finally
            {
                Conn.Close();
            }
            return objgetSpecificTasks;

        }
        #endregion

        #region // Insertion (addSubmitTimeSheet)
        //public TotalTimeSheetTimeDetails addSubmitTimeSheet(TotalTimeSheetTimeDetails sheetObj)
        //{
        //    TotalTimeSheetTimeDetails objTimeSheet = new TotalTimeSheetTimeDetails();
        //    AddTimesheetStatus objAddstatus = new AddTimesheetStatus();
        //    AddTMStatus objstatus = new AddTMStatus();
        //    //  objTimeSheet.Status = new List<statusMessage>();
        //    Conn = new SqlConnection(str);
        //    int Trans_Output = 0;
        //    try
        //    {
        //        if (Conn.State != System.Data.ConnectionState.Open)
        //            Conn.Open();
        //        SqlCommand objCommand = new SqlCommand("[AddSubmitTimesheet]", Conn);
        //        objCommand.CommandType = CommandType.StoredProcedure;
        //        objCommand.Parameters.AddWithValue("@UserID", sheetObj.timesheets.UserID);
        //        objCommand.Parameters.AddWithValue("@TimesheetMonth", Convert.ToDateTime(sheetObj.timesheets.TaskDate).ToShortDateString());
        //        objCommand.Parameters.AddWithValue("@Comments", sheetObj.timesheets.Comments);
        //        objCommand.Parameters.AddWithValue("@ProjectID", sheetObj.timesheets.ProjectID);
        //        objCommand.Parameters.AddWithValue("@SubmittedType", sheetObj.timesheets.SubmittedType);
        //        objCommand.Parameters.AddWithValue("@L1ApproverStatus", false);
        //        objCommand.Parameters.AddWithValue("@L2ApproverStatus", false);
        //        objCommand.Parameters.Add("@TimesheetID", SqlDbType.Int);
        //        objCommand.Parameters["@TimesheetID"].Direction = ParameterDirection.Output;
        //        objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
        //        objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
        //        objCommand.ExecuteNonQuery();
        //        timeSheetID = int.Parse(objCommand.Parameters["@TimesheetID"].Value.ToString());
        //        Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
        //        if (Trans_Output == 104)
        //        {
        //            //objTimeSheet.timesheets = new timesheet
        //            //{
        //            //    Transoutput = Trans_Output,
        //            //    Message = "Timesheet for this Month '" + Convert.ToDateTime(sheetObj.timesheets.TaskDate).ToShortDateString() + "' is Already Submitted",
        //            //    SubmittedState = "Repeated",

        //            //};

        //            objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
        //            {

        //                Transoutput = Trans_Output,
        //                Message = "Timesheet for this Month '" + Convert.ToDateTime(sheetObj.timesheets.TaskDate).ToShortDateString() + "' is Already Submitted",
        //                SubmittedState = "Repeated",
        //                SubmittedType = sheetObj.timesheets.SubmittedType,
        //            };



        //            return objTimeSheet;
        //        }

        //        if (timeSheetID > 0)
        //        {
        //            foreach (var item in sheetObj.listtimesheetdetails)
        //            {

        //                objCommand = new SqlCommand("[AddSubmitTaskDetails]", Conn);
        //                objCommand.CommandType = System.Data.CommandType.StoredProcedure;
        //                objCommand.Parameters.AddWithValue("@TimesheetID", timeSheetID);
        //                objCommand.Parameters.AddWithValue("@ProjectID", item.projectid);
        //                objCommand.Parameters.AddWithValue("@TaskId", item.taskid);
        //                objCommand.Parameters.AddWithValue("@HoursWorked", item.hoursWorked);
        //                objCommand.Parameters.AddWithValue("@TaskDate", Convert.ToDateTime(item.taskDate).ToShortDateString());
        //                objCommand.ExecuteNonQuery();

        //            }

        //        }
        //        Conn.Close();
        //        if (Trans_Output == 1)
        //        {

        //            if (sheetObj.timesheets.SubmittedType == "Submit")
        //            {
        //                objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
        //                {
        //                    Transoutput = Trans_Output,
        //                    SubmittedType = sheetObj.timesheets.SubmittedType,
        //                    Message = "Timesheet Submitted Successfully",
        //                    SubmittedState = "Once",
        //                };

        //            }

        //            else
        //            {
        //                objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
        //                {
        //                    Transoutput = Trans_Output,
        //                    SubmittedType = sheetObj.timesheets.SubmittedType,
        //                    Message = "Timesheet Submitted Successfully",
        //                    SubmittedState = "Once",
        //                };


        //            }
        //        }

        //        else
        //        {
        //            objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
        //            {
        //                Transoutput = 0,
        //                SubmittedType = sheetObj.timesheets.SubmittedType,
        //                Message = "Failed",
        //                SubmittedState = "Failed",
        //            };

        //        }
        //        return objTimeSheet;
        //    }

        //    catch (Exception ex)
        //    {
        //        cls_Log objlog = new cls_Log();
        //        objlog.AddtoLogFile("TimeSheetServices", "addSubmitTimeSheet", ex.Message.ToString());
        //    }
        //    finally
        //    {
        //        if (Conn != null)
        //        {
        //            if (Conn.State == ConnectionState.Open)
        //            {
        //                Conn.Close();
        //                Conn.Dispose();
        //            }
        //        }

        //    }
        //    return objTimeSheet;



        //}
        #endregion


        #region Insertion (addSubmitTimeSheet)
        public TotalTimeSheetTimeDetails addSubmitTimeSheet(TotalTimeSheetTimeDetails sheetObj)
        {
            TotalTimeSheetTimeDetails objTimeSheet = new TotalTimeSheetTimeDetails();
            AddTimesheetStatus objAddstatus = new AddTimesheetStatus();
            AddTMStatus objstatus = new AddTMStatus();
            objTimeSheet.status = new Status();
            //  objTimeSheet.Status = new List<statusMessage>();
            Conn = new SqlConnection(str);
            int Trans_Output = 0;
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[AddSubmitTimesheetWeb]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@UserID", sheetObj.timesheets.UserID);
                objCommand.Parameters.AddWithValue("@TimesheetMonth", Convert.ToDateTime(sheetObj.timesheets.TaskDate).ToShortDateString());
                objCommand.Parameters.AddWithValue("@Comments", sheetObj.timesheets.Comments);
                //  objCommand.Parameters.AddWithValue("@ProjectID", sheetObj.timesheets.ProjectID);
                objCommand.Parameters.AddWithValue("@SubmittedType", sheetObj.timesheets.SubmittedType);
                objCommand.Parameters.AddWithValue("@L1ApproverStatus", false);
                objCommand.Parameters.AddWithValue("@L2ApproverStatus", false);
                objCommand.Parameters.Add("@TimesheetID", SqlDbType.Int);
                objCommand.Parameters["@TimesheetID"].Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                timeSheetID = int.Parse(objCommand.Parameters["@TimesheetID"].Value.ToString());
                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
                if (timeSheetID > 0)
                {
                    foreach (var item in sheetObj.listtimesheetdetails)
                    {

                        if ((Trans_Output == 105) || (Trans_Output == 106))
                        {
                            objCommand = new SqlCommand("[EditSubmitTaskDetails]", Conn);
                            objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            objCommand.Parameters.AddWithValue("@TimesheetID", timeSheetID);
                            objCommand.Parameters.AddWithValue("@ProjectID", item.projectid);
                            objCommand.Parameters.AddWithValue("@TaskId", item.taskid);
                            objCommand.Parameters.AddWithValue("@HoursWorked", item.hoursWorked);

                            if (item.taskDate != null)
                            {
                                string dt = DateTime.Parse(item.taskDate.Trim()).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                                objCommand.Parameters.AddWithValue("@TaskDate", dt);
                            }
                            else
                            {
                            }

                            objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                            objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                            objCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            objCommand = new SqlCommand("[AddSubmitTaskDetails]", Conn);
                            objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            objCommand.Parameters.AddWithValue("@TimesheetID", timeSheetID);
                            objCommand.Parameters.AddWithValue("@ProjectID", item.projectid);
                            objCommand.Parameters.AddWithValue("@TaskId", item.taskid);
                            objCommand.Parameters.AddWithValue("@HoursWorked", item.hoursWorked);
                            if (item.taskDate != null)
                            {
                                string dt = DateTime.Parse(item.taskDate.Trim())
     .ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                                objCommand.Parameters.AddWithValue("@TaskDate", dt);
                            }
                            else
                            {
                            }
                            objCommand.ExecuteNonQuery();
                        }
                    }

                }

                Conn.Close();
                if (Trans_Output == 1)
                {
                    if (sheetObj.timesheets.SubmittedType == "Submit")
                    {

                        //StatusMsg = "TimeSheet for this particular Month '" + sheetObj.timesheets.TaskDate + "' is Submitted Sucessfully..";
                        SendMailsForApprovals(sheetObj);

                        objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
                        {
                            Transoutput = Trans_Output,
                            Message = "Timesheet for this Particular Month '" + Convert.ToDateTime(sheetObj.timesheets.TaskDate).ToShortDateString() + "' is Submitted Sucessfully..",
                            SubmittedState = "Once",
                            SubmittedType = sheetObj.timesheets.SubmittedType,
                        };
                    }

                    else
                    {
                        // StatusMsg = "TimeSheet for this particular Month '" + sheetObj.timesheets.TaskDate + "' is Saved Sucessfully..";

                        objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
                        {

                            Transoutput = Trans_Output,
                            Message = "Timesheet for this Particular Month '" + Convert.ToDateTime(sheetObj.timesheets.TaskDate).ToShortDateString() + "' is Saved Sucessfully..",
                            SubmittedState = "Once",
                            SubmittedType = sheetObj.timesheets.SubmittedType,
                        };
                    }
                }
                else
                {
                    if (Trans_Output == 104)
                    {
                        // StatusMsg = "TimeSheet for this particular Month '" + sheetObj.timesheets.TaskDate + "' is already exists for this User";

                        objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
                        {
                            Transoutput = Trans_Output,
                            Message = "Timesheet for this Particular Month '" + Convert.ToDateTime(sheetObj.timesheets.TaskDate).ToShortDateString() + "' is already exists for this User..",
                            SubmittedState = "Repeated",
                            SubmittedType = sheetObj.timesheets.SubmittedType,
                        };
                    }

                    else if (Trans_Output == 105)
                    {
                        //StatusMsg = "TimeSheet for this particular Month '" + sheetObj.timesheets.TaskDate + "' is Saved Sucessfully";
                        objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
                        {

                            Transoutput = Trans_Output,
                            Message = "Timesheet for this Particular Month '" + Convert.ToDateTime(sheetObj.timesheets.TaskDate).ToShortDateString() + "' is Saved Sucessfully..",
                            SubmittedState = "Once",
                            SubmittedType = sheetObj.timesheets.SubmittedType,
                        };
                    }
                    else if (Trans_Output == 106)
                    {
                        // StatusMsg = "TimeSheet for this particular Month '" + sheetObj.timesheets.TaskDate + "' is Submitted Sucessfully";
                        SendMailsForApprovals(sheetObj);
                        objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
                        {
                            Transoutput = Trans_Output,
                            Message = "Timesheet for this Particular Month '" + Convert.ToDateTime(sheetObj.timesheets.TaskDate).ToShortDateString() + "' is Submitted Sucessfully..",
                            SubmittedState = "Once",
                            SubmittedType = sheetObj.timesheets.SubmittedType,
                        };
                    }
                    else if (Trans_Output == 111)
                    {
                        // StatusMsg = "TimeSheet for this particular Month '" + sheetObj.timesheets.TaskDate + "' is Already  Submitted";

                        objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
                        {
                            Transoutput = Trans_Output,
                            Message = "Timesheet for this Particular Month '" + Convert.ToDateTime(sheetObj.timesheets.TaskDate).ToShortDateString() + "' is Already  Submitted..",
                            SubmittedState = "Repeated",
                            SubmittedType = sheetObj.timesheets.SubmittedType,
                        };
                    }
                    else if (Trans_Output == 0)
                    {
                        // StatusMsg = "Precondition Failed";

                        objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
                        {

                            Transoutput = Trans_Output,
                            Message = "Precondition Failed..",
                            SubmittedState = "",
                            SubmittedType = sheetObj.timesheets.SubmittedType,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                objTimeSheet.status = new Status
                {
                    StatusCode = 500,
                    StatusMessage = ex.Message.ToString(),
                };
            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
            }
            return objTimeSheet;
        }
        #endregion

        #region EditTimeSheetDetails  
        public TotalTimeSheetTimeDetails EditTimeSheetList(int Timesheetid, TotalTimeSheetTimeDetails EditsheetObj)
        {
            TotalTimeSheetTimeDetails objTimeSheet = new TotalTimeSheetTimeDetails();
            Status objStatus = new Status();
            // objTimeSheet.Status = new List<statusMessage>();
            Conn = new SqlConnection(str);
            int Trans_Output = 0;
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[EditSubmitTimeSheet]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@TimesheetID", Timesheetid);
                objCommand.Parameters.AddWithValue("@UserID", EditsheetObj.timesheets.UserID);
                objCommand.Parameters.AddWithValue("@Comments", EditsheetObj.timesheets.Comments);
                // objCommand.Parameters.AddWithValue("@ProjectID", EditsheetObj.timesheets.ProjectID);
                objCommand.Parameters.AddWithValue("@SubmittedType", EditsheetObj.timesheets.SubmittedType);
                objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
                if (Trans_Output == 111)
                {
                    objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
                    {
                        Transoutput = Trans_Output,
                        Message = "Timesheet is Submitted and cannot be Edited..",
                        SubmittedType = EditsheetObj.timesheets.SubmittedType,
                    };
                }



                if ((Trans_Output == 105) || (Trans_Output == 106))
                {
                    foreach (var item in EditsheetObj.listtimesheetdetails)
                    {
                        objCommand = new SqlCommand("[EditSubmitTaskDetails]", Conn);
                        objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        objCommand.Parameters.AddWithValue("@TimesheetID", Timesheetid);
                        objCommand.Parameters.AddWithValue("@ProjectID", item.projectid);
                        objCommand.Parameters.AddWithValue("@TaskId", item.taskid);
                        objCommand.Parameters.AddWithValue("@HoursWorked", item.hoursWorked);
                        objCommand.Parameters.AddWithValue("@TaskDate", Convert.ToDateTime(item.taskDate).ToShortDateString());
                        objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                        objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                        objCommand.ExecuteNonQuery();
                        Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
                    }
                    if (Trans_Output == 1)
                    {
                        if (EditsheetObj.timesheets.SubmittedType == "Submit")
                        {
                            // string msg=  SendMailsForApprovals(EditsheetObj);
                            SendMailsForApprovals(EditsheetObj);
                            objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
                            {
                                Transoutput = Trans_Output,
                                Message = "TimeSheet Submited and Updated Successfully",
                                SubmittedState = "Once",
                                SubmittedType = EditsheetObj.timesheets.SubmittedType,
                            };
                        }

                        else
                        {
                            objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
                            {
                                Transoutput = Trans_Output,
                                Message = "TimeSheet Saved and updated Successfully",
                                SubmittedState = "Once",
                                SubmittedType = EditsheetObj.timesheets.SubmittedType,
                            };
                        }
                    }
                }
                else if (Trans_Output == 0)
                {
                    objTimeSheet.addTimesheetStatus = new AddTimesheetStatus
                    {

                        Transoutput = Trans_Output,
                        Message = "Wrong Inputs",
                        // SubmittedState = "Once",
                        SubmittedType = EditsheetObj.timesheets.SubmittedType,
                    };
                }

                return objTimeSheet;

            }

            catch (Exception ex)
            {
                //cls_Log objlog = new cls_Log();
                //objlog.AddtoLogFile("TimeSheetServices", "editTimeSheetList", ex.Message.ToString());
                objTimeSheet.status = new Status
                {
                    StatusCode = 500,
                    StatusMessage = ex.Message.ToString(),
                };
            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
            }
            return objTimeSheet;

        }
        #endregion


        #region getUserTimeSheetList ForApproval by managers(GetAllUsers of TimeSheet Details)
        public ManagerDetails GetUserTimeSheetforApproval(string UserID, int StartPosition, int EndPosition)
        {

            ManagerDetails objmanagerdetails = new ManagerDetails(); string timesheetAprrovalStatus = string.Empty;

            Conn = new SqlConnection(str);
            try
            {
                objmanagerdetails.timesheetsforapproval = new List<ManagerTimesheet>();
                objmanagerdetails.mytimesheets = new List<AllUsersTimeSheetDetails>();

                Conn.Open();
                SqlCommand cmd = new SqlCommand("[GetTimeSheetforApproval]", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", UserID);
                cmd.Parameters.AddWithValue("@StartPosition", StartPosition);
                cmd.Parameters.AddWithValue("@EndPosition", EndPosition);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dr["ResultSubmitStatus"].ToString() == "Approved By L1 Manager and Pending at L2 Manager")
                        {
                            timesheetAprrovalStatus = "pea";
                        }

                        if (dr["ResultSubmitStatus"].ToString() == "Pending at L1 Manager and Approved by L2 Manager")
                        {
                            timesheetAprrovalStatus = "ora";
                        }

                        if (dr["ResultSubmitStatus"].ToString() == "Waiting for Approval at Both Managers")
                        {
                            timesheetAprrovalStatus = "tea";
                        }
                        if (dr["ResultSubmitStatus"].ToString() == "Approval At Both Managers")
                        {
                            timesheetAprrovalStatus = "gre";
                        }
                        if (dr["ResultSubmitStatus"].ToString() == "rejected")
                        {
                            timesheetAprrovalStatus = "red";
                        }

                        if (dr["ResultSubmitStatus"].ToString() == "Rejected at Level2")
                        {
                            timesheetAprrovalStatus = "red";
                        }
                        if (dr["ResultSubmitStatus"].ToString() == "Rejected at Level1")
                        {
                            timesheetAprrovalStatus = "red";
                        }

                        if (dr["ResultSubmitStatus"].ToString() == "Saved")
                        {
                            timesheetAprrovalStatus = "yel";
                        }
                        objmanagerdetails.mytimesheets.Add(new AllUsersTimeSheetDetails
                        {
                            TimesheetID = Convert.ToInt32(dr["TimesheetID"]),
                            Month_Year = dr["MonthYearName"].ToString(),
                            CompanyBillingHours = dr["Companyworkinghours"].ToString(),
                            ResourceWorkingHours = dr["WorkedHours"].ToString(),
                            TimesheetStatus = timesheetAprrovalStatus,
                        });
                    }
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow druser in ds.Tables[1].Rows)
                    {

                        if (druser["ResultSubmitStatus"].ToString() == "Approved By L1 Manager and Pending at L2 Manager")
                        {
                            timesheetAprrovalStatus = "pea";
                        }

                        if (druser["ResultSubmitStatus"].ToString() == "Pending at L1 Manager and Approved by L2 Manager")
                        {
                            timesheetAprrovalStatus = "ora";
                        }

                        if (druser["ResultSubmitStatus"].ToString() == "Waiting for Approval at Both Managers")
                        {
                            timesheetAprrovalStatus = "tea";
                        }
                        if (druser["ResultSubmitStatus"].ToString() == "Approval At Both Managers")
                        {
                            timesheetAprrovalStatus = "gre";
                        }
                        if (druser["ResultSubmitStatus"].ToString() == "rejected")
                        {
                            timesheetAprrovalStatus = "red";
                        }
                        if (druser["ResultSubmitStatus"].ToString() == "Rejected at Level1")
                        {
                            timesheetAprrovalStatus = "red";
                        }

                        if (druser["ResultSubmitStatus"].ToString() == "Rejected at Level2")
                        {
                            timesheetAprrovalStatus = "red";
                        }
                        objmanagerdetails.totalRow = new TotalRows
                        {
                            TotalCountforApproval = Convert.ToInt16(druser["TotalCount"]),
                        };

                        objmanagerdetails.timesheetsforapproval.Add(new ManagerTimesheet
                        {
                            Userid = Convert.ToInt16(druser["Userid"]),
                            Username = druser["username"].ToString(),
                            TimesheetID = Convert.ToInt16(druser["Timesheetid"]),
                            Month_Year = druser["monthyearname"].ToString(),
                            CompanyBillMyWorkingHours = Convert.ToInt16(druser["workedhours"]),
                            CompanyBillingHours = Convert.ToInt16(druser["Companyworkinghours"]),
                            TimesheetStatus = timesheetAprrovalStatus,
                        });
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
            }
            return objmanagerdetails;
        }
        #endregion



        #region (getUserTimesheet)Preview the Specific MonthData of  Specific userdata 
        public Timesheetlist getUserTimesheet(string UserID, string TaskDate)
        {
            Timesheetlist objGetTimeSheet = new Timesheetlist();
            objGetTimeSheet.timeSheetDetails = new List<TimeSheetDetails>();
            // objGetTimeSheet.Status = new List<statusMessage>();

            Conn = new SqlConnection(str);
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("getUserTimesheet", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", UserID);
                cmd.Parameters.AddWithValue("@Timesheetmonth", TaskDate);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        objGetTimeSheet.timeSheetDetails.Add(new TimeSheetDetails
                        {
                            UserID = Convert.ToInt32(dr["Usr_UserID"]),
                            UserName = dr["Usr_Username"].ToString(),
                            Taskid = Convert.ToInt32(dr["tsk_TaskID"]),
                            Taskname = dr["tsk_TaskName"].ToString(),
                            ProjectId = Convert.ToInt32(dr["ProjectID"]),
                            NoofHoursWorked = Convert.ToInt32(dr["HoursWorked"]),
                            Comments = dr["Comments"].ToString(),
                            TaskDate = Convert.ToDateTime(dr["TaskDate"]).ToString("yyyy-MM-dd"),
                            Submitted_Type = dr["SubmittedType"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                cls_Log objlog = new cls_Log();
                objlog.AddtoLogFile("TimeSheetServices", "getUserTimesheet", ex.Message.ToString());

            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
            }
            return objGetTimeSheet;
        }
        #endregion


        #region ChangeorUpdatePassword(ChangePassword for login Method) 
        public ChangePassworddetails ChangeorUpdatePassword(Login objlogin)
        {
            ChangePassworddetails objChangePassword = new ChangePassworddetails();
            //objChangePassword.EmpInfobyLogin = new List<UserAuth>();

            UserAuth obj = new UserAuth();
            Conn = new SqlConnection(str);
            int Flag = 0;
            try
            {
                if (objlogin.NewPassword != "")
                {
                    if (Conn.State != System.Data.ConnectionState.Open)
                        Conn.Open();
                    SqlCommand objCommand = new SqlCommand("[ChangeOrUpdatePasswd]", Conn);
                    objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    objCommand.Parameters.AddWithValue("@UserLoginId", objlogin.username);
                    objCommand.Parameters.AddWithValue("@CurrentPassword", objlogin.password);
                    objCommand.Parameters.AddWithValue("@NewPassword", GetMD5(objlogin.NewPassword));
                    objCommand.Parameters.Add("@Flag", SqlDbType.Int);
                    objCommand.Parameters["@Flag"].Direction = ParameterDirection.Output;
                    objCommand.ExecuteNonQuery();
                    Flag = int.Parse(objCommand.Parameters["@Flag"].Value.ToString());
                    Conn.Close();

                    if (Flag == 1)
                    {
                        objChangePassword.StatusCode = 1;

                    }

                    else if (Flag == 0)
                    {
                        objChangePassword.StatusCode = 2;

                    }
                }
                else
                {
                    objChangePassword.StatusCode = 3;
                }
                return objChangePassword;
            }
            catch (Exception ex)
            {
                cls_Log objlog = new cls_Log();
                objlog.AddtoLogFile("TimeSheetServices", "ChangeorUpdatePassword", ex.Message.ToString());
                //objChangePassword.status = new Status
                //{
                //    StatusCode = 500,
                //    StatusMessage = ex.InnerException.ToString(),
                //};
            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
            }
            return objChangePassword;
        }
        #endregion

        #region TimeSheetManagerActions
        public TotalTimeSheetInfo TimeSheetManagerActions(TotalTimeSheetInfo sheetObj)
        {
            TotalTimeSheetInfo objtime = new TotalTimeSheetInfo();

            int Trans_Output = 0;

            try
            {
                Conn = new SqlConnection(str);
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[TimesheetManagerActions]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@UserID", sheetObj.timeSheetActions.EmpUsrID);
                objCommand.Parameters.AddWithValue("@TimesheetID", sheetObj.timeSheetActions.TimesheetID);
                objCommand.Parameters.AddWithValue("@ManagerId", sheetObj.timeSheetActions.ManagerId);
                objCommand.Parameters.AddWithValue("@Comments", sheetObj.timeSheetActions.Comments);
                objCommand.Parameters.AddWithValue("@SubmittedType", sheetObj.timeSheetActions.SubmittedType);
                objCommand.Parameters.AddWithValue("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
                if (Trans_Output == 0)
                {
                    objtime.timeSheetActions = new TimeSheetTimeData
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Invalid Timesheet",
                        SubmittedState = "Once",
                        SubmittedType = sheetObj.timeSheetActions.SubmittedType,
                        TimesheetID = sheetObj.timeSheetActions.TimesheetID,
                        Comments = sheetObj.timeSheetActions.Comments,
                        EmpUsrID = sheetObj.timeSheetActions.EmpUsrID,
                        ManagerId = sheetObj.timeSheetActions.ManagerId,
                    };

                    objtime.status = new Status
                    {
                        StatusCode = 400,
                        StatusMessage = "Bad Request",
                    };
                }
                if (Trans_Output == 900)
                {
                    objtime.timeSheetActions = new TimeSheetTimeData
                    {
                        Transoutput = Trans_Output,
                        Position = "",
                        Message = "ManagerId is Incorrect",
                        SubmittedState = "Once",
                        SubmittedType = sheetObj.timeSheetActions.SubmittedType,
                        TimesheetID = sheetObj.timeSheetActions.TimesheetID,
                        Comments = sheetObj.timeSheetActions.Comments,
                        EmpUsrID = sheetObj.timeSheetActions.EmpUsrID,
                        ManagerId = sheetObj.timeSheetActions.ManagerId,
                    };


                    objtime.status = new Status
                    {
                        StatusCode = 400,
                        StatusMessage = "Bad Request",
                    };
                }

                if (Trans_Output == 1)
                {
                    objtime.timeSheetActions = new TimeSheetTimeData
                    {

                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Accepted by Level-1 Manager",
                        SubmittedType = sheetObj.timeSheetActions.SubmittedType,
                        TimesheetID = sheetObj.timeSheetActions.TimesheetID,
                        Comments = sheetObj.timeSheetActions.Comments,
                        EmpUsrID = sheetObj.timeSheetActions.EmpUsrID,
                        ManagerId = sheetObj.timeSheetActions.ManagerId,
                        SubmittedState = "Once",
                    };

                    objtime.status = new Status
                    {
                        StatusCode = 200,
                        StatusMessage = "OK",
                    };
                }

                if (Trans_Output == 2)
                {
                    objtime.timeSheetActions = new TimeSheetTimeData
                    {

                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Rejected by Level-1 Manager",
                        SubmittedType = sheetObj.timeSheetActions.SubmittedType,
                        TimesheetID = sheetObj.timeSheetActions.TimesheetID,
                        Comments = sheetObj.timeSheetActions.Comments,
                        EmpUsrID = sheetObj.timeSheetActions.EmpUsrID,
                        ManagerId = sheetObj.timeSheetActions.ManagerId,
                        SubmittedState = "",

                    };

                    objtime.status = new Status
                    {
                        StatusCode = 200,
                        StatusMessage = "OK",
                    };
                }
                if (Trans_Output == 3)
                {

                    objtime.timeSheetActions = new TimeSheetTimeData
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Accepted by Level-2 Manager",
                        SubmittedType = sheetObj.timeSheetActions.SubmittedType,
                        TimesheetID = sheetObj.timeSheetActions.TimesheetID,
                        Comments = sheetObj.timeSheetActions.Comments,
                        EmpUsrID = sheetObj.timeSheetActions.EmpUsrID,
                        ManagerId = sheetObj.timeSheetActions.ManagerId,
                        SubmittedState = "Once",

                    };


                    objtime.status = new Status
                    {
                        StatusCode = 200,
                        StatusMessage = "OK",
                    };

                }
                if (Trans_Output == 4)
                {
                    objtime.timeSheetActions = new TimeSheetTimeData
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Rejected by Level-2 Manager",
                        SubmittedType = sheetObj.timeSheetActions.SubmittedType,
                        TimesheetID = sheetObj.timeSheetActions.TimesheetID,
                        Comments = sheetObj.timeSheetActions.Comments,
                        EmpUsrID = sheetObj.timeSheetActions.EmpUsrID,
                        ManagerId = sheetObj.timeSheetActions.ManagerId,
                        SubmittedState = "",

                    };

                    objtime.status = new Status
                    {
                        StatusCode = 200,
                        StatusMessage = "OK",
                    };

                }

                if (Trans_Output == 104)
                {
                    if (sheetObj.timeSheetActions.SubmittedType.ToString() == "1")
                    {
                        objtime.timeSheetActions = new TimeSheetTimeData
                        {
                            Transoutput = Trans_Output,
                            Position = "L1",
                            Message = "Timesheet is Already Approved and can't be approved again!!!",
                            SubmittedType = sheetObj.timeSheetActions.SubmittedType,
                            TimesheetID = sheetObj.timeSheetActions.TimesheetID,
                            Comments = sheetObj.timeSheetActions.Comments,
                            EmpUsrID = sheetObj.timeSheetActions.EmpUsrID,
                            ManagerId = sheetObj.timeSheetActions.ManagerId,
                            SubmittedState = "",

                        };

                    }
                    else if (sheetObj.timeSheetActions.SubmittedType.ToString() == "0")
                    {
                        objtime.timeSheetActions = new TimeSheetTimeData
                        {
                            Transoutput = Trans_Output,
                            Position = "L1",
                            Message = "Timesheet is Already Approved and Cannot be Rejected again!!!",
                            SubmittedType = sheetObj.timeSheetActions.SubmittedType,
                            TimesheetID = sheetObj.timeSheetActions.TimesheetID,
                            Comments = sheetObj.timeSheetActions.Comments,
                            EmpUsrID = sheetObj.timeSheetActions.EmpUsrID,
                            ManagerId = sheetObj.timeSheetActions.ManagerId,
                            SubmittedState = "",

                        };
                    }
                    objtime.status = new Status
                    {
                        StatusCode = 200,
                        StatusMessage = "OK",
                    };

                }

                if (Trans_Output == 105)
                {
                    if (sheetObj.timeSheetActions.SubmittedType.ToString() == "1")
                    {
                        objtime.timeSheetActions = new TimeSheetTimeData
                        {
                            Transoutput = Trans_Output,
                            Position = "L2",
                            Message = "Timesheet is Already Approved",
                            SubmittedType = sheetObj.timeSheetActions.SubmittedType,
                            TimesheetID = sheetObj.timeSheetActions.TimesheetID,
                            Comments = sheetObj.timeSheetActions.Comments,
                            EmpUsrID = sheetObj.timeSheetActions.EmpUsrID,
                            ManagerId = sheetObj.timeSheetActions.ManagerId,
                            SubmittedState = "",

                        };
                    }
                    else if (sheetObj.timeSheetActions.SubmittedType.ToString() == "0")
                    {
                        objtime.timeSheetActions = new TimeSheetTimeData
                        {
                            Transoutput = Trans_Output,
                            Position = "L2",
                            Message = "Timesheet is Already Approved and Cannot be Rejected",
                            SubmittedType = sheetObj.timeSheetActions.SubmittedType,
                            TimesheetID = sheetObj.timeSheetActions.TimesheetID,
                            Comments = sheetObj.timeSheetActions.Comments,
                            EmpUsrID = sheetObj.timeSheetActions.EmpUsrID,
                            ManagerId = sheetObj.timeSheetActions.ManagerId,
                            SubmittedState = "",

                        };
                    }

                    objtime.status = new Status
                    {
                        StatusCode = 200,
                        StatusMessage = "OK",
                    };

                }



            }
            catch (Exception ex)
            {
                //cls_Log objlog = new cls_Log();
                //objlog.AddtoLogFile("TimeSheetServices", "TimeSheetManagerActions", ex.Message.ToString());
                objtime.status = new Status
                {
                    StatusCode = 500,
                    StatusMessage = ex.InnerException.ToString(),
                };


            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
            }
            return objtime;
        }
        #endregion



        #region Get CalenderHolidayList 
        public HolidayListEntitiy GetCalenderList(String UserID, String AccountID, String HolidaySelectionDate)
        {
            HolidayListEntitiy lstHolidayDetails = new HolidayListEntitiy();
            lstHolidayDetails.holidayDetails = new List<HolidayDetails>();

            Conn = new SqlConnection(str);
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("GetUserHolidayCalender", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@UserAccontID", AccountID);
                cmd.Parameters.AddWithValue("@HolidaySelectionDate", HolidaySelectionDate);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                if (ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {

                        lstHolidayDetails.holidayDetails.Add(new HolidayDetails
                        {
                            HolidayName = dr["HolidayName"].ToString(),
                            HolidayDate = Convert.ToDateTime(dr["HolidayDate"]).ToString("yyyy-MM-dd"),
                            isOptionalHoliday = dr["isOptionalHoliday"].ToString(),
                        });
                    }
                    lstHolidayDetails.status = new Status
                    {
                        StatusCode = 200,
                        StatusMessage = "OK",
                    };
                }
                else
                {
                    lstHolidayDetails.status = new Status
                    {
                        StatusCode = 404,
                        StatusMessage = "NotFound",
                    };
                }
            }
            catch (Exception ex)
            {
                cls_Log objlog = new cls_Log();
                objlog.AddtoLogFile("TimeSheetServices", "GetCalenderList", ex.Message.ToString());
                lstHolidayDetails.status = new Status
                {
                    StatusCode = 500,
                    StatusMessage = ex.InnerException.ToString(),
                };

            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
            }
            return lstHolidayDetails;
        }
        #endregion



        #region getProfileDetails  
        public UserProfilesInfo getUserProfileDetails(String ProfileUserId)
        {
            UserProfilesInfo objUserPro = new UserProfilesInfo();
            objUserPro.Userprofilesdata = new UserProfilesData();
            // objUserPro.Userprofilesdata = new List<UserProfilesData>();
            Conn = new SqlConnection(str);
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("GetProfileDetails", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", ProfileUserId);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows != null)
                {

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {

                        objUserPro.Userprofilesdata = new UserProfilesData
                        {
                            UsrP_UserProfileID = Convert.ToInt32(dr["UsrP_UserProfileID"]),
                            UsrP_UserID = Convert.ToInt32(dr["UsrP_UserID"]),
                            UsrPFullName = dr["UsrName"].ToString(),
                            UsrP_EmployeeID = dr["EmpID"].ToString(),
                            UsrP_EmailID = dr["Emailid"].ToString(),
                            UsrP_MobNum = dr["Mobile Number"].ToString(),
                            UsrP_PhoneNumber = dr["Usrp_PhoneNumber"].ToString(),
                            UsrP_DOJ = Convert.ToDateTime(dr["Dateofjoing"]).ToString("yyyy-MM-dd"),
                            UsrP_ProfilePicture = dr["ProfilePicture"].ToString(),
                        };

                    }
                }

                else
                {
                    objUserPro.status = new Status
                    {
                        StatusCode = 400,
                        StatusMessage = "Not Found",
                    };
                }

            }
            catch (Exception ex)
            {
                //cls_Log objlog = new cls_Log();
                //objlog.AddtoLogFile("TimeSheetServices", "getUserProfileDetails", ex.Message.ToString());
                objUserPro.status = new Status
                {
                    StatusCode = 500,
                    StatusMessage = ex.Message.ToString(),
                };

            }
            finally
            {
                Conn.Close();
            }
            return objUserPro;

        }
        #endregion


        #region UpdateProfileStatus  
        public UserProfileDetails AddOrEditUsersProfilebyUser(UserProfileDetails UsrPObj)
        {
            UserProfileDetails objProfileDetails = new UserProfileDetails();
            SqlConnection Conn = new SqlConnection(str);
            int UserProfileID = 0;
            int Trans_Output = 0;
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[AddorEditUsersProfileByUser]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@UserID", UsrPObj.UsrP_UserID);
                objCommand.Parameters.AddWithValue("@UsrFname", UsrPObj.UsrFstName);
                objCommand.Parameters.AddWithValue("@UsrLname", UsrPObj.UsrLstName);
                objCommand.Parameters.AddWithValue("@UsrEmpId", UsrPObj.UsrP_EmployeeID);
                objCommand.Parameters.AddWithValue("@UsrEmailId", UsrPObj.UsrP_EmailID);
                objCommand.Parameters.AddWithValue("@UserMobilenum", UsrPObj.UsrP_MobNum);
                objCommand.Parameters.AddWithValue("@PhoneNum", UsrPObj.UsrP_PhoneNumber);
                objCommand.Parameters.AddWithValue("@User_ProfilePath", UsrPObj.UsrP_ProfilePicture);
                objCommand.Parameters.AddWithValue("@UsrDob", UsrPObj.UsrP_DOB);
                objCommand.Parameters.AddWithValue("@UsrDoj", UsrPObj.UsrP_DOJ);
                objCommand.Parameters.Add("@UserProfileID", SqlDbType.Int);
                objCommand.Parameters["@UserProfileID"].Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                UserProfileID = int.Parse(objCommand.Parameters["@UserProfileID"].Value.ToString());
                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
                if (Trans_Output == 1)
                {
                    objProfileDetails = new UserProfileDetails
                    {
                        Trans_Output = Trans_Output,
                    };

                    objProfileDetails.status = new Status
                    {
                        StatusCode = 200,
                        StatusMessage = "OK and Inserted Sucessfully",
                    };
                }
                else if (Trans_Output == 2)
                {
                    objProfileDetails = new UserProfileDetails
                    {
                        Trans_Output = Trans_Output,
                    };

                    objProfileDetails.status = new Status
                    {
                        StatusCode = 200,
                        StatusMessage = "OK and Updated Sucessfully",
                    };

                    //return "Updated Successfully";
                }
                else if (Trans_Output == 0)
                {
                    objProfileDetails.status = new Status
                    {
                        StatusCode = 400,
                        StatusMessage = "BadRequest",
                    };
                }
                
            }
            catch (Exception ex)
            {
                //cls_Log objlog = new cls_Log();
                //objlog.AddtoLogFile("TimeSheetServices", "AddOrEditUsersProfilebyUser", ex.Message.ToString());
                objProfileDetails.status = new Status
                {
                    StatusCode = 500,
                    StatusMessage = ex.Message.ToString(),
                };

            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
            }
            return objProfileDetails;

        }
        #endregion
        /////////////////////////Email////////////////////////////
        //email//
        //email//
        #region Emails for Timesheet Aprrovals by Manager Levels

        #region SendMails
        public string SendMailsForApprovals(TotalTimeSheetTimeDetails sheetObj)
        {
            Conn = new SqlConnection(str);
            DataTable dtaccMgr = new DataTable();

            List<manageremails> objManagerlist = new List<manageremails>();
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[GetManagerEmailIds]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@UserID", sheetObj.timesheets.UserID);
                da = new SqlDataAdapter(objCommand);
                da.Fill(ds);
                dt = ds.Tables[0];
                dtaccMgr = ds.Tables[1];
                SqlDataReader dr = objCommand.ExecuteReader();

                if (dt != null)
                {
                    lstusers = new timesheet()
                    {
                        ManagerEmail1 = dt.Rows[0]["ManagerLevel1"].ToString(),
                        ManagerEmail2 = dt.Rows[0]["ManagerLevel2"].ToString(),
                        AccManagerEmail = dt.Rows[0]["Acc_EmailID"].ToString(),
                        UserName = dt.Rows[0]["UserName"].ToString(),
                        UserEmailId = dt.Rows[0]["UserEmailid"].ToString(),
                        ManagerID1 = dt.Rows[0]["L1_ManagerID"].ToString(),
                        ManagerID2 = dt.Rows[0]["L2_ManagerID"].ToString(),
                        ManagerId = sheetObj.timesheets.ManagerId,
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        SubmittedFlag = sheetObj.timesheets.SubmittedFlag,
                        ManagerName1 = dt.Rows[0]["L1_ManagerName"].ToString(),
                        ManagerName2 = dt.Rows[0]["L2_ManagerName"].ToString(),
                    };
                }

                foreach (DataRow accmanager in ds.Tables[1].Rows)
                {
                    objManagerlist.Add(new manageremails
                    {
                        manageremail = accmanager["Usr_LoginId"].ToString(),
                    });
                }

                Conn.Close();
                Conn.Open();
                objCommand = new SqlCommand("[getTimeSheetEmailDetails]", Conn);
                objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@userid", sheetObj.timesheets.UserID.ToString());
                string date = Convert.ToDateTime(sheetObj.timesheets.TaskDate).ToString("yyyy-MM-dd");
                objCommand.Parameters.AddWithValue("@Timesheetmonth", date);
                ds1 = new DataSet();
                da = new SqlDataAdapter(objCommand);
                da.Fill(ds1);
                dtheadings = new DataTable();
                dtData = new DataTable();
                dtheadings = ds1.Tables[0];
                dtData = ds1.Tables[1];
                string subject = string.Empty;
                if ((!string.IsNullOrEmpty(sheetObj.timesheets.SubmittedType)) && (sheetObj.timesheets.SubmittedType == "Submit"))
                {
                    //if (lstusers.ManagerEmail1 != null && lstusers.ManagerEmail2 != null && flag == 1)
                    //{
                    string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "1"));
                    SendMailsByMailGun(lstusers, objManagerlist, dtheadings, body, 1, "");

                    // }
                }

                if ((!string.IsNullOrEmpty(sheetObj.timesheets.SubmittedFlag)) && sheetObj.timesheets.SubmittedFlag.ToString() == "2")
                {
                    if ((sheetObj.timesheets.Transoutput == 1) || (sheetObj.timesheets.Transoutput == 2) || (sheetObj.timesheets.Transoutput == 3) || (sheetObj.timesheets.Transoutput == 4))
                    {
                        string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "2"));
                        StatusMsg = SendMailsByMailGun(lstusers, objManagerlist, dtheadings, body, 2, sheetObj.timesheets.ActionType);
                        return StatusMsg;
                    }
                    else
                    {
                        string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "2"));
                        return body;
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return StatusMsg;
        }
        #endregion

        #region SendMailsByMailGun
        public static string SendMailsByMailGun(timesheet lstusers, List<manageremails> objManagerlist, DataTable dtHeading, string body, int flag, string ActionType)
        {


            string[] ToMuliId = new string[0];
            string subject = string.Empty;
            string ManagersIDs = string.Empty; bool flagforemail = false;
            RestClient client = new RestClient();
            string href = string.Empty; string ManagerID = string.Empty;
            string LevelManagerID = string.Empty, TimesheetId = string.Empty, Timesheetmonth = string.Empty,
            UserID = string.Empty; string disbledstr = string.Empty; string Projectid = string.Empty;
            string ManagerNameL1 = string.Empty; string ManagerNameL2 = string.Empty;

            HttpStatusCode statusCode; int numericStatusCode; RestRequest request = new RestRequest();
            string host = System.Web.HttpContext.Current.Request.Url.Host;
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
            string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];

            string UrlEmailAddress = string.Empty;

            if (host == "localhost")
            {
                UrlEmailAddress = host + ":" + port;
            }
            else
            {
                UrlEmailAddress = port1;
            }


            int j = 0;
            try
            {
                client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                client.Authenticator = new HttpBasicAuthenticator("api", "key-25b8e8c23e4a09ef67b7957d77eb7413");
                //request.AddHeader

                if (flag == 1)
                {

                    Encrypt objEncrypt;
                   // ToMuliId = new string[2] { "sreelakshmi.evolutyz@gmail.com", "" };

                    ToMuliId = new string[2] { lstusers.ManagerEmail1.ToString().Trim(), lstusers.ManagerEmail2.ToString().Trim()};


                    for (int i = 0; i < ToMuliId.Length; i++)
                    {
                        request = new RestRequest();
                        request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
                        request.Resource = "{domain}/messages";
                        request.AddParameter("from", "Evolutyz IT Services<mailgun@evolutyzstaging.com>");
                        if (i == 0 && flagforemail == false)
                        {
                            request.AddParameter("to", ToMuliId[0]);
                            ManagerID = lstusers.ManagerID1.ToString();
                            string LevelManagerID1 = string.Empty;
                            subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  submitted  and waiting for your approval";
                            objEncrypt = new Encrypt();
                            LevelManagerID1 = objEncrypt.Encryption(ManagerID).ToString();
                            TimesheetId = objEncrypt.Encryption(dtHeading.Rows[0]["TimesheetId"].ToString());
                            Timesheetmonth = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["TimesheetMonthforMail"]));
                            UserID = objEncrypt.Encryption(dtHeading.Rows[0]["Usr_UserID"].ToString());
                            Projectid = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["ProjectID"].ToString()));
                            ManagerNameL1 = objEncrypt.Encryption(lstusers.ManagerName1.ToString());
                            ManagerNameL2 = objEncrypt.Encryption(lstusers.ManagerName2.ToString());
                            if (body.Contains("ApproveID"))
                            {
                                j = 1;
                                if (ManagerID != "0")
                                {
                                    href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID1 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL1 + "'";
                                    body = body.Replace("href", href);
                                }
                            }
                            if (body.Contains("RejectId"))
                            {
                                j = 0;
                                href = string.Empty;
                                if (ManagerID != "0")
                                {
                                    href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID1 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL1 + "'";
                                    body = body.Replace("title", href);
                                }
                                //  ping to title
                            }
                        }
                        if (i == 1 && flagforemail == true)
                        {
                            request.AddParameter("to", ToMuliId[1]);
                            ManagerID = string.Empty;
                            ManagerID = lstusers.ManagerID2.ToString();
                            subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  submitted  and waiting for your approval";
                            string LevelManagerID2 = string.Empty;
                            objEncrypt = new Encrypt();
                            LevelManagerID2 = objEncrypt.Encryption(ManagerID).ToString();
                            TimesheetId = objEncrypt.Encryption(dtHeading.Rows[0]["TimesheetId"].ToString());
                            Timesheetmonth = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["TimesheetMonthforMail"]));
                            UserID = objEncrypt.Encryption(dtHeading.Rows[0]["Usr_UserID"].ToString());
                            Projectid = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["ProjectID"].ToString()));
                            ManagerNameL1 = objEncrypt.Encryption(lstusers.ManagerName1.ToString());
                            ManagerNameL2 = objEncrypt.Encryption(lstusers.ManagerName2.ToString());
                            if (body.Contains("ApproveID"))
                            {
                                j = 1;
                                href = string.Empty;
                                body = body.Replace("name", href);
                                if (ManagerID != "0")
                                {
                                    href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL2 + "'";
                                    body = body.Replace("name", href);
                                }
                            }

                            if (body.Contains("RejectId"))
                            {

                                j = 0;

                                href = string.Empty;
                                body = body.Replace("rev", href);
                                if (ManagerID != "0")
                                {
                                    href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL2 + "'";
                                    body = body.Replace("rev", href);

                                }
                            }

                        }


                        var emailcontent = "<html>" +
                                                          "<body />" +
                                                          "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                          " <tr style='background: linear-gradient(#0a639a,#0689bd);background-color: #6cb0c9;'>" +
                                                          "<td style=' padding: 8px 4px; '>" +
                                                          "<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
                                                          " </td>" +
                                                          "</tr>" +
                                                          " <tr>" +
                                                          "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                          body +
                                                          // "HI" +
                                                          " </td>" +
                                                          "</tr>" +
                                                          " <tr>" +
                                                          "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                          //"<b>" + msg + "</b> " +
                                                          " </td>" +
                                                          "</tr>" +
                                                          " <tr style='background-color: #6cb0c9;'>" +
                                                          "<td style='font-size: 9px;text-align:center; '>" +
                                                          "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                          " </td>" +
                                                          "</tr>" +
                                                          "</table>" +
                                                          "</body>" +
                                                          "</html>";


                        request.AddParameter("subject", subject);
                        request.AddParameter("html", emailcontent);
                        request.Method = Method.POST;
                        var a = client.Execute(request);
                        statusCode = a.StatusCode;
                        numericStatusCode = (int)statusCode;

                        if (numericStatusCode == 200)
                        {
                            flagforemail = true;


                        }
                        else
                        {
                            flagforemail = true;
                        }


                    }


                    for (int AcctMgrid = 0; AcctMgrid < objManagerlist.Count; AcctMgrid++)//Account Managerids
                    {

                        request = new RestRequest();
                        request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
                        request.Resource = "{domain}/messages";
                        request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
                        request.AddParameter("to", objManagerlist[AcctMgrid].manageremail);
                        subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is submitted to managers levels";
                        //disbledstr = "disabled='true'";

                        disbledstr = "display:none;";

                        if (body.Contains("ApproveID"))
                        {
                            body = body.Replace("display:block", disbledstr);
                        }

                        if (body.Contains("RejectId"))
                        {
                            body = body.Replace("display:block", disbledstr);

                        }

                        var emailcontent = "<html>" +
                                                          "<body />" +
                                                          "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                          " <tr style='background: linear-gradient(#0a639a,#0689bd);background-color: #6cb0c9;'>" +
                                                          "<td style=' padding: 8px 4px; '>" +
                                                          //"<img src= " + UrlEmailAddress + "'/Content/images/CompanyLogos/502.png'  style='display: block; max-width: 100%; height: 40px;'>" +
                                                          "<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
                                                          " </td>" +
                                                          "</tr>" +
                                                          " <tr>" +
                                                          "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                          body +
                                                          // "HI" +
                                                          " </td>" +
                                                          "</tr>" +
                                                          " <tr>" +
                                                          "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                          //"<b>" + msg + "</b> " +
                                                          " </td>" +
                                                          "</tr>" +
                                                          " <tr style='background-color: #6cb0c9;'>" +
                                                          "<td style='font-size: 9px;text-align:center; '>" +
                                                          "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                          " </td>" +
                                                          "</tr>" +
                                                          "</table>" +
                                                          "</body>" +
                                                          "</html>";


                        request.AddParameter("subject", subject);
                        request.AddParameter("html", emailcontent);
                        request.Method = Method.POST;
                        var a1 = client.Execute(request);

                        statusCode = a1.StatusCode;
                        numericStatusCode = (int)statusCode;

                        if (numericStatusCode == 200)
                        {
                            flagforemail = true;


                        }
                        else
                        {
                            flagforemail = true;
                        }
                    }


                }


                if (flag == 2)
                {

                    if (lstusers.ManagerId.ToString() == lstusers.ManagerID1.ToString())
                    {

                    //   ToMuliId = new string[2] { "sreelakshmi.evolutyz@gmail.com", "" };
                          ToMuliId = new string[2] { lstusers.ManagerEmail2.ToString().Trim(), lstusers.UserEmailId.ToString().Trim() };

                        if (lstusers.EmailAppOrRejStatus.ToString() == "1")
                        {
                            subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  approved by level_1 manager (" + lstusers.ManagerName1 + ")";
                        }
                        else if (lstusers.EmailAppOrRejStatus.ToString() == "0")
                        {
                            subject = "Timesheet of " + lstusers.UserName + " for Month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  rejected by level_1 manager " + lstusers.ManagerName1 + "";
                        }

                        for (int i = 0; i < ToMuliId.Length; i++)
                        {
                            request = new RestRequest();
                            request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
                            request.Resource = "{domain}/messages";
                            request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
                            ManagerID = lstusers.ManagerID1.ToString();


                            if (i == 0 && flagforemail == false)
                            {
                                request.AddParameter("to", ToMuliId[0]);

                            }
                            if (i == 1 && flagforemail == true)
                            {
                                request.AddParameter("to", ToMuliId[1]);

                                //if (lstusers.EmailAppOrRejStatus.ToString() == "0")
                                //{
                                //    subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  rejected by level_1 manager and resubmit the timesheet for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + "";
                                //}
                            }
                            var emailcontent2 = "<html>" +
                                                      "<body />" +
                                                      "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                      " <tr style='background: linear-gradient(#0a639a,#0689bd);background-color: #6cb0c9;'>" +
                                                      "<td style=' padding: 8px 4px; '>" +
                                                      "<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
                                                      " </td>" +
                                                      "</tr>" +
                                                      " <tr>" +
                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                      body +
                                                      // "HI" +
                                                      " </td>" +
                                                      "</tr>" +
                                                      " <tr>" +
                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                      //"<b>" + msg + "</b> " +
                                                      " </td>" +
                                                      "</tr>" +
                                                      " <tr style='background-color: #6cb0c9;'>" +
                                                      "<td style='font-size: 9px;text-align:center; '>" +
                                                      "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                      " </td>" +
                                                      "</tr>" +
                                                      "</table>" +
                                                      "</body>" +
                                                      "</html>";


                            request.AddParameter("subject", subject);
                            request.AddParameter("html", emailcontent2);
                            request.Method = Method.POST;
                            var a6 = client.Execute(request);
                            statusCode = a6.StatusCode;
                            numericStatusCode = (int)statusCode;

                            if (numericStatusCode == 200)
                            {
                                flagforemail = true;
                            }
                            else
                            {
                                flagforemail = true;
                            }
                        }


                        for (int AcctMgrid = 0; AcctMgrid < objManagerlist.Count; AcctMgrid++)//Account Managerids
                        {

                            request = new RestRequest();
                            request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
                            request.Resource = "{domain}/messages";
                            request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
                            request.AddParameter("to", objManagerlist[AcctMgrid].manageremail);
                            //disbledstr = "disabled='true'";

                            disbledstr = "display:none;";

                            if (body.Contains("ApproveID"))
                            {
                                body = body.Replace("display:block", disbledstr);
                            }

                            if (body.Contains("RejectId"))
                            {
                                body = body.Replace("display:block", disbledstr);

                            }

                            var emailcontent = "<html>" +
                                                              "<body />" +
                                                              "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                              " <tr style='background: linear-gradient(#0a639a,#0689bd);background-color: #6cb0c9;'>" +
                                                              "<td style=' padding: 8px 4px; '>" +
                                                              //"<img src= " + UrlEmailAddress + "'/Content/images/CompanyLogos/502.png'  style='display: block; max-width: 100%; height: 40px;'>" +
                                                              "<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              body +
                                                              // "HI" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              //"<b>" + msg + "</b> " +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr style='background-color: #6cb0c9;'>" +
                                                              "<td style='font-size: 9px;text-align:center; '>" +
                                                              "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              "</table>" +
                                                              "</body>" +
                                                              "</html>";


                            request.AddParameter("subject", subject);
                            request.AddParameter("html", emailcontent);
                            request.Method = Method.POST;
                            var a1 = client.Execute(request);

                            statusCode = a1.StatusCode;
                            numericStatusCode = (int)statusCode;

                            if (numericStatusCode == 200)
                            {
                                flagforemail = true;


                            }
                            else
                            {
                                flagforemail = true;
                            }
                        }

                    }

                    if (lstusers.ManagerId.ToString() == lstusers.ManagerID2.ToString())
                    {

                       // ToMuliId = new string[2] { "", "" };
                         ToMuliId = new string[2] { lstusers.ManagerEmail1.ToString().Trim(), lstusers.UserEmailId.ToString().Trim() };
                        if (lstusers.EmailAppOrRejStatus.ToString() == "1")
                        {

                            subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  approved by level_2 manager " + lstusers.ManagerName2 + "";

                        }
                        else if (lstusers.EmailAppOrRejStatus.ToString() == "0")
                        {
                            subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  rejected by level_2 manager " + lstusers.ManagerName2 + "";
                        }
                        for (int i = 0; i < ToMuliId.Length; i++)
                        {
                            request = new RestRequest();
                            request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
                            request.Resource = "{domain}/messages";
                            request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
                            ManagerID = lstusers.ManagerID1.ToString();


                            if (i == 0 && flagforemail == false)
                            {
                                request.AddParameter("to", ToMuliId[0]);

                            }
                            if (i == 1 && flagforemail == true)
                            {
                                request.AddParameter("to", ToMuliId[1]);
                                //if (lstusers.EmailAppOrRejStatus.ToString() == "0")
                                //{
                                //    subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  rejected by level_2 manager and resubmit the timesheet for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + "";
                                //}
                            }

                            var emailcontent = "<html>" +
                                                              "<body />" +
                                                              "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                              " <tr style='background: linear-gradient(#0a639a,#0689bd);background-color: #6cb0c9;'>" +
                                                              "<td style=' padding: 8px 4px; '>" +
                                                              //"<img src=" + UrlEmailAddress + "'/Content/images/CompanyLogos/502.png'  style='display: block; max-width: 100%; height: 40px;'>" +
                                                              "<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              body +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr style='background-color: #6cb0c9;'>" +
                                                              "<td style='font-size: 9px;text-align:center; '>" +
                                                              "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              "</table>" +
                                                              "</body>" +
                                                              "</html>";


                            request.AddParameter("subject", subject);
                            request.AddParameter("html", emailcontent);
                            request.Method = Method.POST;
                            var a5 = client.Execute(request);
                            statusCode = a5.StatusCode;
                            numericStatusCode = (int)statusCode;
                            if (numericStatusCode == 200)
                            {
                                flagforemail = true;
                            }
                            else
                            {
                                flagforemail = true;

                            }


                        }


                        for (int AcctMgrid = 0; AcctMgrid < objManagerlist.Count; AcctMgrid++)//Account Managerids
                        {

                            request = new RestRequest();
                            request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
                            request.Resource = "{domain}/messages";
                            request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
                            request.AddParameter("to", objManagerlist[AcctMgrid].manageremail);

                            var emailcontent = "<html>" +
                                                              "<body />" +
                                                              "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                              " <tr style='background: linear-gradient(#0a639a,#0689bd);background-color: #6cb0c9;'>" +
                                                              "<td style=' padding: 8px 4px; '>" +
                                                              //"<img src= " + UrlEmailAddress + "'/Content/images/CompanyLogos/502.png'  style='display: block; max-width: 100%; height: 40px;'>" +
                                                              "<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              body +
                                                              // "HI" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              //"<b>" + msg + "</b> " +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr style='background-color: #6cb0c9;'>" +
                                                              "<td style='font-size: 9px;text-align:center; '>" +
                                                              "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              "</table>" +
                                                              "</body>" +
                                                              "</html>";


                            request.AddParameter("subject", subject);
                            request.AddParameter("html", emailcontent);
                            request.Method = Method.POST;
                            var a1 = client.Execute(request);

                            statusCode = a1.StatusCode;
                            numericStatusCode = (int)statusCode;

                            if (numericStatusCode == 200)
                            {
                                flagforemail = true;


                            }
                            else
                            {
                                flagforemail = true;
                            }
                        }

                    }
                    //objTA.MailsByMailGun(lstusers, body);

                    return body;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "Success";
        }
        #endregion

        #region ConvertDataTableToHTML 
        public static string ConvertDataTableToHTML(DataTable dtHeading, DataTable TimeSheetdt, string flag)
        {
            string html = ""; string html1 = string.Empty; string ApproveBtn = string.Empty, RejectBtn = string.Empty;
            if (flag == "1")
            {
                ApproveBtn = "block";
                RejectBtn = "block";
            }
            else
            {
                ApproveBtn = "none";
                RejectBtn = "none";
            }
            var totalRows = TimeSheetdt.Rows.Count;
            int halfway = totalRows / 2;
            var firstHalfdt = TimeSheetdt.AsEnumerable().Take(halfway).CopyToDataTable();
            var secondHalfdt = TimeSheetdt.AsEnumerable().Skip(halfway).CopyToDataTable();


            html += "<form style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;background-color:#ffffff;'>" +
             "<div style='-webkit-box-sizing: border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:0 10px;'> " +
           " <table style='width:100%;border-spacing:0px 20px;border-collapse:separate;border:0;-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;'>" +
          "<tbody style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>" +
          "<tr style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>" +
          "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;'> Employee Name: </ th > " +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;'>" + dtHeading.Rows[0]["Employee Name"].ToString() + "</td>" +
          "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;'> Client Name: </th> " +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;'>" + dtHeading.Rows[0]["Proj_ClientName"].ToString() + " </ td > " +
          "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;'> Project Name: </th> " +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;'>" + dtHeading.Rows[0]["ProjectName"].ToString() + " </td> " +
          "</tr><tr style='-webkit-box-sizing: border-box;-moz-box-sizing:border-box; box-sizing: border-box'> " +
          "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;'>Timesheet Cycle: </ th >" +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;'> Monthly </ td >" +
          "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;'> Duration : </ th >" +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;'>" + dtHeading.Rows[0]["TimesheetMonth"].ToString() + "</td>" +
           "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;'> Submitted Date: </th>" +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;' > " + dtHeading.Rows[0]["SubmittedDate"].ToString() + "</td>" +
           "</tr></tbody></table></div>";
            //fisrt table

            html += "<table id='tblvertical' style='width:100%;table-layout: fixed;'><tr><td style='vertical-align: top;'>";
            html += "<div style='overflow-x:auto;'><section style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;position:relative;min-height:1px;' > " +
           "<table style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;border-collapse:collapse;border-spacing:0;width:100%;border:1px solid #ddd;background-color: #fff;' >" +
           "<thead style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>" +
           "<tr style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'> " +
           "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;'>Date</th> " +
           "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;'> Task </ th > " +
           "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;'> Hours </ th >" +
           "</tr></thead><tbody style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>";

            for (int i = 0; i < firstHalfdt.Rows.Count; i++)
            {
                html += "<tr style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;background-color:#ffffff;'>";

                for (int j = 0; j < firstHalfdt.Columns.Count; j++)
                {
                    html += "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;'>" + firstHalfdt.Rows[i][j].ToString() + "</td>";

                }
                html += "</tr>";
            }

            html += "</tbody></table></section></div></td>";
            //second datatable
            html += "<td style='vertical-align: top;'><div style='overflow-x:auto;'><section style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;position:relative;min-height:1px;' > " +
              "<table style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;border-collapse:collapse;border-spacing:0;width:100%;border:1px solid #ddd;background-color: #fff;' >" +
              "<thead style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>" +
              "<tr style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'> " +
              "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;'>Date</th> " +
              "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;'> Task </ th > " +
              "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;'> Hours </ th >" +
              "</tr></thead><tbody style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>";

            for (int i = 0; i < secondHalfdt.Rows.Count; i++)
            {
                html += "<tr style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;background-color:#ffffff;'>";


                for (int j = 0; j < secondHalfdt.Columns.Count; j++)
                {
                    html += "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;'>" + secondHalfdt.Rows[i][j].ToString() + "</td>";

                }
                html += "</tr>";
            }

            html += "</tbody></table></section></div></td></tr></table>";
            // comments tab
            html += "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:10px;' > " +
   "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;'>" +
   "<label style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;margin:0010px;font-weight:700;display:none;' for='comment'> Comments :</label>" +
   "<textarea style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;border-radius:5px;box-shadow:none;border-color:#d2d6de;font-family:sans-serif;-webkit-appearance:none;-moz-appearance:none;appearance:none;width:100%;display:none;' rows='3' id='comment'></textarea></div></div>";


            //start add Approve and Reject Buttons

            html += "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:" +
                "border-box;overflow-x:auto;text-align:center;padding:10px;'>" +
                 "<section id='secIdApp'  style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;" +
                 "text-align:right;width:20%;float:left;position:relative;padding:0 15px;min-height:1px;'>" +
                 "<a id='ApproveID'   runat='server'  style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;width:100%;display:" + ApproveBtn + ";padding:6px 12px;margin-bottom:0;font-size:14px;font-weight:400;line-height:1.42857143;" +
                 "text-align:center;white-space:nowrap;vertical-align:middle;-ms-touch-action:manipulation;touch-action:manipulation;cursor:pointer;-webkit-user-select:none; -moz-user-select:none;-ms-user-select:none;" +
                 "user-select:none;border:1px solid #090;border-radius:4px;color:#fff;background-color:#090;" +
                 "text-decoration:underline;'  href name  target='_blank'>Approve</a></section> " +

                 "<section  style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;" +
                 "width:30%;float:left;position:relative;padding:0 15px;min-height:1px;'>" +
                 "<a id='RejectId' title style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;" +
                 "display:" + RejectBtn + ";padding:6px 12px;margin-bottom:0; font-size:14px; font-weight:400; " +
                 "line-height:1.42857143;text-align:center;white-space:nowrap;vertical-align:middle;-ms-touch-action:manipulation; " +
                 "touch-action: manipulation;cursor:pointer;-webkit-user-select:none;-moz-user-select: none;" +
                 "-ms-user-select:none;user-select:none;border:1px solid #C30;" +
                 "border-radius:4px;color:#fff;background-color:#C30;text-decoration:underline;" +
                 "' rev  target='_blank'>Reject</a></section>";

            //end Approve and Reject Buttons


            html += "<section style='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;width: 50%; float: left; position: relative; padding: 0 15px; min-height: 1px;overflow: auto;'><table style ='-webkit-box-sizing: border-box;-moz-box-sizing:border-box;box-sizing:border-box;table-layout: fixed;" +
              "border-collapse:collapse;border-spacing:0;width:100%;border:1px solid #ddd;background-color:#fff;'>" +
              "<tbody style ='-webkit-box-sizing: border-box;-moz-box-sizing:border-box;box-sizing:border-box;'>" +
              "<tr style ='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing: border-box'>" +
              "<th style ='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;" +
              " font-size:18px; text-align:center; padding: 8px;border: 1px solid #ccc;background: #666; color: white;'> " +
              "Total Working Hours :</th>" +
              "<td style ='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;font-weight:600;width:75px;" +
              "font-size:18px;text-align:center;padding:8px;border:1px solid #ccc;'> " + dtHeading.Rows[0]["TotalWorkingHours"].ToString() + " </ td > " +
              "</tr></tbody></table></section></div></form> </div></section></body></html>";

            return html;

        }
        #endregion
        #endregion
        //#region Push notifications
        //public void Getdata(string Title, string Message, string userid)
        //{

        //    var SendMessage = Message;
        //    var Messagetitle = Title;
        //    var Selectgrade = lstusers;
        //    Conn = new SqlConnection(str);
        //    //  conn.ConnectionString = "Data Source=192.168.75.11;Initial Catalog=PushNotifications;User ID=Eits1664;Password=Evolutyz@1664";
        //    Conn.Open();

        //    using (SqlCommand cmd2 = new SqlCommand("Select TokenID from [UserDevicesToken] where UserId = '" + userid + "'", Conn))
        //    {
        //        SqlDataAdapter adapter1 = new SqlDataAdapter(cmd2);
        //        DataTable dt = new DataTable();
        //        adapter1.Fill(dt);
        //        try
        //        {
        //            if (dt.Rows.Count > 0)
        //            {
        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    var deviceid = Convert.ToString(dt.Rows[i]["TokenID"]);
        //                    string SERVER_API_KEY = "AAAAuP0gfzU:APA91bFP-UKu97svEZy-b42JVuxMjRyxs8RWt_ndXgRgIatjZoKN5ehNf0yznMy2iu-y_4zE_b0IRV3P-k33aFHYis_Ory73GX7iu1b3jA8MRvh9rjtPOELA_WvXOfdpz5oisCPktoEk";
        //                    string SENDER_ID = "794520747829";
        //                    string device_Id = deviceid;
        //                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        //                    tRequest.Method = "post";
        //                    tRequest.ContentType = "application/json";
        //                    var data = new
        //                    {
        //                        to = device_Id,
        //                        notification = new
        //                        {
        //                            body = Message,
        //                            title = Title,
        //                            sound = "Enabled"
        //                        }
        //                    };

        //                    var serializer = new JavaScriptSerializer();
        //                    var json = serializer.Serialize(data);
        //                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
        //                    tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
        //                    tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
        //                    tRequest.ContentLength = byteArray.Length;

        //                    using (Stream dataStream = tRequest.GetRequestStream())
        //                    {
        //                        dataStream.Write(byteArray, 0, byteArray.Length);
        //                        using (WebResponse tResponse = tRequest.GetResponse())
        //                        {
        //                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
        //                            {
        //                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
        //                                {
        //                                    String sResponseFromServer = tReader.ReadToEnd();
        //                                    string str = sResponseFromServer;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            string str = ex.Message;
        //        }
        //    }

        //}
        //#endregion

        #region getUserTimeSheetList ForApproval by managers(GetAllUsers of TimeSheet Details)
        public LeaveCategoriesEntity getEmpLeaveCategories(string UserID, string Year)
        {

            LeaveCategoriesEntity objLeaveCategorieslist = new LeaveCategoriesEntity(); string timesheetAprrovalStatus = string.Empty;

            Conn = new SqlConnection(str);
            try
            {
                objLeaveCategorieslist.EmpLeaveCategory = new List<LeaveCategoriesDetails>();
                Conn.Open();
                SqlCommand cmd = new SqlCommand("[GetLeaveCategoreries]", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", UserID);
                cmd.Parameters.AddWithValue("@Year", Year);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {

                        objLeaveCategorieslist.EmpLeaveCategory.Add(new LeaveCategoriesDetails
                        {
                            leaveType = dr["LTyp_LeaveType"].ToString(),
                            totLeavesCount = dr["leavecount"].ToString(),
                            totHolidaysUsed = dr["HolidaysUsed"].ToString(),
                            leaveTypeID = Convert.ToInt32(dr["LTyp_LeaveTypeID"]),

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
            }
            return objLeaveCategorieslist;
        }
        #endregion

        public Logindetails UserAuthenticateusingToken(Login objlogin)
        {
            Logindetails objLogindetails = new Logindetails();
            try
            {
                string loginType = string.Empty;
                int messageFlag = 1;
                // loginType = "ln";
                SqlConnection Conn = new SqlConnection(str);
                Conn.Open();
                SqlCommand cmd = new SqlCommand("UserTkn", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", objlogin.username);
                cmd.Parameters.AddWithValue("@password", GetMD5(objlogin.password));
                cmd.Parameters.AddWithValue("@token", objlogin.token);
                // cmd.Parameters.AddWithValue("@loginType", loginType);
                cmd.Parameters.AddWithValue("@Message", SqlDbType.Int);
                cmd.ExecuteNonQuery();
                cmd.Parameters["@Message"].Direction = ParameterDirection.Output;
                SqlDataReader dr = cmd.ExecuteReader();
                if (cmd.Parameters["@Message"].Value != null)
                    messageFlag = Int32.Parse(cmd.Parameters["@Message"].Value.ToString());

                if (dr.Read())
                {

                    objLogindetails.EmpInfobyLogin = new UserAuth();
                    {
                        objLogindetails.EmpInfobyLogin.userid = Convert.ToInt32(dr["Usr_UserID"]);
                        objLogindetails.EmpInfobyLogin.usertype = Convert.ToInt32(dr["Usr_UserTypeID"]);
                        objLogindetails.EmpInfobyLogin.UsTUserTypeID = Convert.ToInt32(dr["UsT_UserTypeID"]);
                        objLogindetails.EmpInfobyLogin.projectID = Convert.ToInt32(dr["Proj_ProjectID"]);
                        objLogindetails.EmpInfobyLogin.projectName = dr["Proj_ProjectName"].ToString();
                        objLogindetails.EmpInfobyLogin.TaskTypeID = Convert.ToInt32(dr["tsk_TaskID"]);
                        objLogindetails.EmpInfobyLogin.TaskName = dr["tsk_TaskName"].ToString();
                        objLogindetails.EmpInfobyLogin.userName = dr["Usr_Username"].ToString();
                        objLogindetails.EmpInfobyLogin.AccountType = dr["UsT_UserType"].ToString();
                        objLogindetails.EmpInfobyLogin.AccountID = dr["Acc_AccountID"].ToString();
                        objLogindetails.EmpInfobyLogin.Companylogopath = dr["Acc_CompanyLogo"].ToString();
                        objLogindetails.EmpInfobyLogin.RoleId = Convert.ToInt32(dr["Rol_RoleID"]);
                        objLogindetails.EmpInfobyLogin.RoleName = dr["Rol_RoleName"].ToString();
                        objLogindetails.EmpInfobyLogin.EmpId = dr["UsrP_EmployeeID"].ToString();
                        objLogindetails.EmpInfobyLogin.EmpEmailId = dr["UsrP_EmailID"].ToString();
                        objLogindetails.EmpInfobyLogin.DOJ = Convert.ToDateTime(dr["Usrp_DOJ"]).ToString("yyyy-MM-dd");
                        objLogindetails.EmpInfobyLogin.EmpFirstName = dr["UsrP_FirstName"].ToString();
                        objLogindetails.EmpInfobyLogin.EmpLastName = dr["UsrP_LastName"].ToString();
                        objLogindetails.EmpInfobyLogin.Trans_Output = messageFlag;
                        objLogindetails.EmpInfobyLogin.Message = "Login Successful";
                    }
                }
                else
                {
                    string errorMessage = string.Empty;

                    if (messageFlag == 2)
                    {
                        objLogindetails.StatusCode = 404;
                        objLogindetails.StatusMessage = "NotFound";
                        objLogindetails.EmpInfobyLogin = new UserAuth();
                        {
                            objLogindetails.EmpInfobyLogin.Trans_Output = messageFlag;
                            objLogindetails.EmpInfobyLogin.Message = "Invalid EmailID or Password";
                        }
                    }
                }
                dr.Close();
                Conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objLogindetails;
        }

        #region Insertion (InsertUserLeaves)

        //public UserLeaves addUserLeaves(UserLeaves UserLeavesObj)
        //{
        //    //int messageFlag = 1;

        //    UserLeaves objuserLeaves = new UserLeaves();
        //    objuserLeaves.addLeaveStatus = new AddLeaveStatus();
        //    int UserLeaveID = 0;
        //    string msg = "";
        //    int CountConsumedbyUser = UserLeavesObj.leavesConsumed.Count;
        //    Conn = new SqlConnection(str);
        //    // int Trans_Output = 0;
        //    try
        //    {
        //        if (CountConsumedbyUser == 0)
        //        {
        //            //objuserLeaves.Message = "Unsuccess";
        //            objuserLeaves.addLeaveStatus.appliedStatus = new AddStatus()
        //            {

        //                ErrorMessage = "LeaveType days are empty",
        //            };
        //        }
        //        else
        //        {
        //            if (Conn.State != System.Data.ConnectionState.Open)
        //                Conn.Open();

        //            SqlCommand objCommand = new SqlCommand("[InsertUserLeaves]", Conn);
        //            objCommand.CommandType = CommandType.StoredProcedure;
        //            objCommand.Parameters.AddWithValue("@UserID", UserLeavesObj.applyLeave.Usrl_UserId);
        //            objCommand.Parameters.AddWithValue("@LeaveStartDate", Convert.ToDateTime(UserLeavesObj.applyLeave.LeaveStartDate).ToShortDateString());
        //            objCommand.Parameters.AddWithValue("@LeaveEndDate", Convert.ToDateTime(UserLeavesObj.applyLeave.LeaveEndDate).ToShortDateString());
        //            objCommand.Parameters.AddWithValue("@Comments", UserLeavesObj.applyLeave.Comments);
        //            objCommand.Parameters.Add("@UserLeaveId", SqlDbType.Int);
        //            objCommand.Parameters["@UserLeaveId"].Direction = ParameterDirection.Output;
        //            //objCommand.Parameters.Add("@Message", SqlDbType.VarChar,50);
        //            //objCommand.Parameters["@Message"].Direction = ParameterDirection.Output;
        //            objCommand.ExecuteNonQuery();
        //            //UserLeaveID = Int32.Parse(objCommand.Parameters["@UserLeaveId"].Value.ToString());
        //            UserLeaveID = objCommand.Parameters["@UserLeaveId"].Value.ToString()==""?0:Convert.ToInt32(objCommand.Parameters["@UserLeaveId"].Value.ToString());
        //            //msg = objCommand.Parameters["@Message"].Value.ToString();

        //            if (UserLeaveID > 0)
        //            {
        //                foreach (var item in UserLeavesObj.leavesConsumed)
        //                {
        //                    objCommand = new SqlCommand("[Insert_UserLeavesConsumed]", Conn);
        //                    objCommand.CommandType = System.Data.CommandType.StoredProcedure;
        //                    objCommand.Parameters.AddWithValue("@UserLeaveId", UserLeaveID);
        //                    objCommand.Parameters.AddWithValue("@No_Of_Days", item.No_Of_Days);
        //                    objCommand.Parameters.AddWithValue("@LeaveTypeID", item.LeaveTypeId);
        //                    objCommand.Parameters.AddWithValue("@UserId", UserLeavesObj.applyLeave.Usrl_UserId);
        //                    objCommand.ExecuteNonQuery();
        //                }

        //                objuserLeaves.addLeaveStatus.appliedStatus = new AddStatus()
        //                {

        //                    Message = "Leaves Submitted Sucessfully",
        //                };

        //                Conn.Close();
        //            }

        //            else if (UserLeaveID == 0)
        //            {
        //                objuserLeaves.addLeaveStatus.appliedStatus = new AddStatus()
        //                {

        //                    ErrorMessage = msg,
        //                };

        //            }

        //        }
        //    }

        //    catch (System.Exception ex)
        //    {
        //        objuserLeaves.addLeaveStatus.appliedStatus = new AddStatus()
        //        {

        //            //  Transoutput = Trans_Output,
        //            ErrorMessage = ex.Message.ToString(),
        //        };
        //    }
        //    finally
        //    {
        //        if (Conn != null)
        //        {
        //            if (Conn.State == ConnectionState.Open)
        //            {
        //                Conn.Close();
        //                Conn.Dispose();
        //            }
        //        }

        //    }
        //    return objuserLeaves;
        //}


        public UserLeaves addUserLeaves(userLeaveData UserLeavesObj)
        {
            //int messageFlag = 1;

            UserLeaves objuserLeaves = new UserLeaves();
            objuserLeaves.addLeaveStatus = new AddLeaveStatus();
            int UserLeaveID = 0;
            Conn = new SqlConnection(str);
            // int Trans_Output = 0;
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                SqlCommand objCommand = new SqlCommand("[Insert_UserLeaves]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@UserID", UserLeavesObj.Usrl_UserId);
                objCommand.Parameters.AddWithValue("@StartDate", Convert.ToDateTime(UserLeavesObj.LeaveStartDate).ToShortDateString());
                objCommand.Parameters.AddWithValue("@EndDate", Convert.ToDateTime(UserLeavesObj.LeaveEndDate).ToShortDateString());
                objCommand.Parameters.Add("@Message", SqlDbType.Int);
                objCommand.Parameters["@Message"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                int message = Int32.Parse(objCommand.Parameters["@Message"].Value.ToString());

                if (message == 1)
                {
                    objuserLeaves.addLeaveStatus.appliedStatus = new AddStatus()
                    {
                        Message = "Leaves Submitted Sucessfully",
                    };
                    Conn.Close();
                }

                else if (message == 0)
                {
                    objuserLeaves.addLeaveStatus.appliedStatus = new AddStatus()
                    {

                        Message = "Invalid UserID",
                    };

                }

            }

            catch (System.Exception ex)
            {
                objuserLeaves.addLeaveStatus.appliedStatus = new AddStatus()
                {

                    //  Transoutput = Trans_Output,
                    Message = ex.Message.ToString(),
                };
            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }

            }
            return objuserLeaves;



        }
        #endregion




        //#region Insertion (InsertUserWorkFromHome)

        //public Userworkfromhome addUserworkfromhome(Userworkfromhome userworkfromhomeobj)
        //{

        //}



        //#endregion



        #region getUserLeavesList (Getdata of UserLeaveList Details)       
        public UserLeavesApprovedEntites GetUserLeavesList(string UserID, string DateofMonth)
        {
            string getdate = string.Empty;
            UserLeavesApprovedEntites lstUserLeaveDetails = new UserLeavesApprovedEntites();
            TeamLeaves teamleavedetails = new TeamLeaves();
            lstUserLeaveDetails.myLeaveDetails = new List<MyLeaveDetails>();
            lstUserLeaveDetails.TeamLeaveDetails = new List<TeamLeaveDetails>();
            string Employeename = string.Empty;
            int Employeeid = 0;
            //List<TeamLeaves1> teamleavedetails1= new List<TeamLeaves1>();
            Conn = new SqlConnection(str);
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("Get_LeavesCount", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserID);
                cmd.Parameters.AddWithValue("@DateOfMonth", DateofMonth);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                    {

                        //string emp = Employeename;
                        //int emno = Employeeid;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            //lstUserLeaveDetails.Employeename = Employeename;
                            //lstUserLeaveDetails.Employeeid = Employeeid;
                            //Employeename = dr["UserName"].ToString();
                            //Employeeid = Convert.ToInt32(dr["USerID"]);
                            lstUserLeaveDetails.myLeaveDetails.Add(new MyLeaveDetails
                            {

                                EmployeeID = Convert.ToInt32(dr["UProj_UserID"]),
                                EmployeeName = dr["UsrP_FirstName"].ToString(),
                                LeaveDate = Convert.ToDateTime(dr["LeaveStartDate"]).ToShortDateString(),
                                EndDate = Convert.ToDateTime(dr["LeaveEndDate"]).ToShortDateString(),

                            });
                        }
                    }
                }
                else
                {
                    lstUserLeaveDetails.StatusCode = 200;
                    lstUserLeaveDetails.StatusMessage = "OK";
                }


                if (ds.Tables[1].Rows != null && ds.Tables[1].Rows.Count > 0)
                {

                    //teamleavedetails.LeaveDate = getdate;
                    //foreach (DataRow dr in ds.Tables[1].Rows)
                    //{

                    //    //getdate = Convert.ToDateTime(dr["LeaveStartDate"]).ToString("yyyy-MM-dd");

                    //    lstUserLeaveDetails.teamleavedetails.Add(new TeamLeaves
                    //    {

                    //        EmployeeID = Convert.ToInt32(dr["UProj_UserID"]),
                    //        EmployeeName = dr["UsrP_FirstName"].ToString(),
                    //        LeaveDate = Convert.ToDateTime(dr["LeaveStartDate"]).ToString("yyyy-MM-dd"),
                    //        EndDate = Convert.ToDateTime(dr["LeaveEndDate"]).ToString("yyyy-MM-dd"),

                    //    });
                    //}


                    var teamleavedates = (from dates in ds.Tables[1].AsEnumerable()
                                          select new { LeaveDate = dates["LeaveStartDate"].ToString() }).Distinct().ToList();

                    lstUserLeaveDetails.TeamLeaveDetails = (from date in teamleavedates
                                                            select new TeamLeaveDetails
                                                            {
                                                                LeaveDate = Convert.ToDateTime(date.LeaveDate).ToShortDateString(),
                                                                Empdetails = (from emp in ds.Tables[1].AsEnumerable()
                                                                              where emp["LeaveStartDate"].ToString() == date.LeaveDate
                                                                              select new TeamLeaves
                                                                              {
                                                                                  EmployeeID = (int)emp["UProj_UserID"],
                                                                                  EmployeeName = emp["UsrP_FirstName"].ToString(),
                                                                                  StartDate = Convert.ToDateTime(emp["LeaveStartDate"]).ToShortDateString(),
                                                                                  EndDate = Convert.ToDateTime(emp["LeaveEndDate"]).ToShortDateString()

                                                                              }).ToList()
                                                            }).ToList();
                }

                else
                {

                    lstUserLeaveDetails.StatusCode = 400;
                    lstUserLeaveDetails.StatusMessage = "Not Found";
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
            }
            return lstUserLeaveDetails;
        }

        #endregion
    }
}