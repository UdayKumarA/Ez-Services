using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using authtesting.Models;
using System.Data;

namespace authtesting
{
    public partial class TimeSheets : System.Web.UI.Page
    {
        string str = ConfigurationManager.ConnectionStrings["EvolutyzCornerDBEntities"].ConnectionString;
        SqlConnection Conn = new SqlConnection();
        Decript objDecript = new Decript();
        string Managerid = string.Empty, Timesheetid = string.Empty,
            ActionType = string.Empty, submittedflag = string.Empty, TimesheetMonth = string.Empty, Userid = string.Empty;
        ETimesheet objtimesheet = new ETimesheet();
        timesheet lstobjtime = new timesheet(); string Emailbody = string.Empty, EmailResponseBody = string.Empty;
        string ManagerLNames = string.Empty; string MailProjectid = string.Empty; TotalTimeSheetTimeDetails sheetObj = new TotalTimeSheetTimeDetails();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                Managerid = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["MID"]));
                Timesheetid = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["TID"]));
                ActionType = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["AT"]));
                TimesheetMonth = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["TD"]));
                Userid = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["Uid"]));
                submittedflag = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["F"]));
                MailProjectid = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["Pr"]));
                ManagerLNames = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["MNOL"]));
            }
            suceessEmail();

        }


        public void suceessEmail()
        {
            try
            {
                if (submittedflag == "2")
                {
                    sheetObj.timesheets = new timesheet()
                    {
                        TimesheetID = Convert.ToInt32(Timesheetid),
                        TaskDate = TimesheetMonth,
                        ManagerId = Managerid,
                        UserID = Convert.ToInt32(Userid),
                        SubmittedFlag = submittedflag,
                        EmailAppOrRejStatus = ActionType,
                        ProjectID = Convert.ToInt32(MailProjectid),
                        ManagerName1 = ManagerLNames.ToString(),
                    };

                    lstobjtime = TimeSheetManagerAction(sheetObj.timesheets);


                    if ((lstobjtime.Transoutput == 1) || (lstobjtime.Transoutput == 2) || (lstobjtime.Transoutput == 3) || (lstobjtime.Transoutput == 4))
                    {
                        sheetObj.timesheets.Transoutput = lstobjtime.Transoutput;
                        Emailbody = objtimesheet.SendMailsForApprovals(sheetObj);
                        lblEmailstatus.Text = lstobjtime.Message;
                        if ((lstobjtime.Transoutput == 1) || (lstobjtime.Transoutput == 3))
                        {
                            lblEmailstatus.Attributes.Add("style", "color: #00bd00");//
                        }
                        else if ((lstobjtime.Transoutput == 2) || (lstobjtime.Transoutput == 4))
                        {
                            lblEmailstatus.Attributes.Add("style", "color: #f44336");//
                        }
                    }
                    else
                    {
                        sheetObj.timesheets.Transoutput = lstobjtime.Transoutput;
                        Emailbody = objtimesheet.SendMailsForApprovals(sheetObj);
                        lblEmailstatus.Text = lstobjtime.Message;
                        lblEmailstatus.Attributes.Add("style", "color: #f44336");// 
                    }

                    divEmailid.InnerHtml = Emailbody;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region TimeSheetManagerActions
        public timesheet TimeSheetManagerAction(timesheet sheetObj)
        {
            timesheet objtimesheet = new timesheet();
            int Trans_Output = 0;


            try
            {
                Conn = new SqlConnection(str);
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[ManagerActionsfromEmail]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@UserID", sheetObj.UserID);
                objCommand.Parameters.AddWithValue("@TimesheetID", sheetObj.TimesheetID);
                objCommand.Parameters.AddWithValue("@Projectid", sheetObj.ProjectID);

                objCommand.Parameters.AddWithValue("@ManagerId", sheetObj.ManagerId);

                // objCommand.Parameters.AddWithValue("@Comments", sheetObj.timeSheetActions.Comments);
                objCommand.Parameters.AddWithValue("@SubmittedType", sheetObj.EmailAppOrRejStatus);
                objCommand.Parameters.AddWithValue("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
                if (Trans_Output == 0)
                {

                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        ManagerName1 = ManagerLNames.ToString(),
                        Message = "Timesheet is already rejected by" + " " + ManagerLNames,
                        SubmittedState = "Once",
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,

                    };



                }
                if (Trans_Output == 900)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "",
                        Message = "Managerid is incorrect",
                        SubmittedState = "Once",
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,

                    };


                }

                if (Trans_Output == 1)
                {


                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Approved by " + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,

                    };

                }

                if (Trans_Output == 2)
                {

                    lstobjtime = new timesheet()
                    {

                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Rejected by " + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,

                    };

                }
                if (Trans_Output == 3)
                {

                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Approved by " + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,

                    };
                }
                if (Trans_Output == 4)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Rejected by " + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,

                    };
                }

                if (Trans_Output == 104)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Timesheet is already approved by " + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,

                    };
                }

                if (Trans_Output == 106)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Timesheet is already rejected by  " + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,

                    };
                }
                if (Trans_Output == 105)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Timesheet is already approved by " + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,

                    };

                }
                if (Trans_Output == 107)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Timesheet is already rejected by " + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,

                    };
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

            return lstobjtime;
        }
        #endregion



        public void MailsByMailGun(timesheet lstobjtime, string Emailbody)
        {
            try
            {

                if (lstobjtime.SubmittedFlag == "2" && lstobjtime.EmailAppOrRejStatus == "1" && lstobjtime.ManagerID1 == lstobjtime.ManagerId)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Approved By Level-1 Manager')", true);
                    divEmailid.InnerHtml = Emailbody;
                }
                else if (lstobjtime.SubmittedFlag == "2" && lstobjtime.EmailAppOrRejStatus == "0" && lstobjtime.ManagerID1 == lstobjtime.ManagerId)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Rejected By Level-1 Manager')", true);
                }

                if (lstobjtime.SubmittedFlag == "2" && lstobjtime.EmailAppOrRejStatus == "1" && lstobjtime.ManagerID2 == lstobjtime.ManagerId)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Approved By Level-2 Manager')", true);
                }
                if (lstobjtime.SubmittedFlag == "2" && lstobjtime.EmailAppOrRejStatus == "0" && lstobjtime.ManagerID2 == lstobjtime.ManagerId)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Rejected By Level-2 Manager')", true);
                }

                divEmailid.InnerHtml = Emailbody;



            }


            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}