using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Security.Cryptography;
using System.Text;





namespace authtesting.Models
{
    public class AlexaAuthentication
    {
        
        string str = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        SqlConnection Conn = new SqlConnection(); SqlDataAdapter da = new SqlDataAdapter();
        DataSet ds = new DataSet(); DataTable dt = new DataTable();
        timesheet lstusers = new timesheet(); 
        DataSet ds1 = new DataSet(); DataTable dtheadings = new DataTable(); DataTable dtData = new DataTable();
        EmailFormats objemail = new EmailFormats(); string StatusMsg = string.Empty;
        public List<Authentication> GetAuthUsers(int Userid)
        {
            using (SqlConnection con = new SqlConnection(str))
            {
                List<Authentication> lstusers = new List<Authentication>();

                con.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("AlexaGetUsers", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", Userid);
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            lstusers.Add(new Authentication
                            {
                                Userid = Convert.ToInt16(sdr["Usr_UserID"]),
                                Firstname = sdr["UsrP_FirstName"].ToString(),
                                Lastname = sdr["UsrP_LastName"].ToString()
                            });
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return lstusers;
            }

        }
        public Authentication GetUsers(string deviceid)
        {
            Authentication lstusers = new Authentication();
            SqlConnection con = new SqlConnection(str);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("AlexaGetUserDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@deviceid", deviceid);
               
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        lstusers = new Authentication
                        {
                            Userid = Convert.ToInt16(sdr["Usr_UserID"]),
                            Firstname = sdr["UsrP_FirstName"].ToString(),
                            Lastname = sdr["UsrP_LastName"].ToString(),
                           
                        };
                    }
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstusers;
        }
        public GetHolidays GetHours(int month, string deviceid/*string phonenumber,*/)
        {

            using (SqlConnection con = new SqlConnection(str))
            {
                GetHolidays objholidays = new GetHolidays();


                con.Open();
                try
                {
                    objholidays.lstholidays = new List<HolidayCalender>();

                    SqlCommand cmd = new SqlCommand("AlexaGetWorkingHoursDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@deviceid", deviceid);
                   
                    // cmd.Parameters.AddWithValue("@pin", pin);
                    //cmd.Parameters.AddWithValue("@status", SqlDbType.Int);
                    cmd.ExecuteNonQuery();
                    //cmd.Parameters["@status"].Direction = ParameterDirection.Output;


                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    //if (cmd.Parameters["@status"].Value != null)
                    //    objholidays.Status = Int32.Parse(cmd.Parameters["@status"].Value.ToString());

                    //if (cmd.Parameters["@status"].Value != null)
                    //    objholidays.Status = Convert.ToInt32(cmd.Parameters["@status"].Value);

                    //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                   
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        objholidays.FirstName = dr["UsrP_FirstName"].ToString();
                    }

                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        //    DateTime dt = DateTime.ParseExact( dr["HolidayDate"].ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                        //  string s = dt.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
                        objholidays.lstholidays.Add(new HolidayCalender
                        {
                            Holidayname = dr["Holidayname"].ToString(),
                            // HolidayDate=Convert.ToDateTime(dr["HolidayDate"])
                            //    HolidayDate = s
                            HolidayDate = Convert.ToDateTime(dr["HolidayDate"]).ToShortDateString()
                        });

                    }
                    foreach (DataRow dr in ds.Tables[2].Rows)
                    {

                        objholidays.HolidaysCount = Convert.ToInt16(dr["holidayscount"]);
                    }
                    foreach (DataRow dr in ds.Tables[3].Rows)
                    {

                        objholidays.Workingdays = Convert.ToInt16(dr["WorkingDays"]);
                    }

                    foreach (DataRow dr in ds.Tables[4].Rows)
                    {
                        objholidays.Workinghours = Convert.ToInt16(dr["WorkingHours"]);
                    }
                    foreach (DataRow dr in ds.Tables[5].Rows)
                    {

                        objholidays.TimesheetMonth = Convert.ToDateTime(dr["TimesheetMonth"]).ToShortDateString();
                        //objholidays.TimesheetMonth = DateTime.da
                    }

                    //if(objholidays.Status==0)
                    //{
                    //    objholidays.Status = 0;
                    //}
                    //if (objholidays.Status == 1)
                    //{
                    //    objholidays.Status = 1;
                    //}
                    //if (objholidays.Status == 2)
                    //{
                    //    objholidays.Status = 2;
                    //}
                    //if (objholidays.Status == 3)
                    //{
                    //    objholidays.Status = 3;
                    //}
                }

                catch (Exception ex)
                {
                    throw ex;
                }
                return objholidays;
            }
        }

        public SubmitTimesheet GetTimeSheet(int month, string deviceid /*string phonenumber,*/)
        {
            using (SqlConnection con = new SqlConnection(str))
            { 
                con.Open();
                SubmitTimesheet objtimesheet = new SubmitTimesheet();
                string TimesheetMonth = string.Empty;
                try
                {
                    SqlCommand cmd = new SqlCommand("AlexaSubmitTimesheetdetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@deviceid", deviceid);
                    
                    // cmd.Parameters.AddWithValue("@pin", pin);
                    cmd.Parameters.AddWithValue("@timesheetSubmitted", SqlDbType.Int);
                    cmd.Parameters.AddWithValue("@workinghours", SqlDbType.Int);

                    cmd.Parameters["@timesheetSubmitted"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@workinghours"].Direction = ParameterDirection.Output;
                    SqlParameter outputParm = new SqlParameter
                    {
                        Direction = System.Data.ParameterDirection.Output,
                        ParameterName = "@TimesheetMonthDate",
                        SqlDbType = System.Data.SqlDbType.Date
                    };
                    cmd.Parameters.Add(outputParm);
                    cmd.ExecuteNonQuery();
                    //SqlDataReader sdr = cmd.ExecuteReader();
                    if (cmd.Parameters["@timesheetSubmitted"].Value != null)
                        objtimesheet.Timesheetstatus = Int32.Parse(cmd.Parameters["@timesheetSubmitted"].Value.ToString());
                    if (cmd.Parameters["@workinghours"].Value != null)
                        objtimesheet.GetWorkingHours = Int32.Parse(cmd.Parameters["@workinghours"].Value.ToString());
                    if (cmd.Parameters["@TimesheetMonthDate"].Value != null)
                        // TimesheetMonth  = (cmd.Parameters["@TimesheetMonthDate"].Value.ToString("yyyy-MM-dd"));
                        TimesheetMonth = Convert.ToDateTime(cmd.Parameters["@TimesheetMonthDate"].Value).ToString("yyyy-MM-dd");


                    if (objtimesheet.Timesheetstatus== 3)
                    {
                        SendMailsForApprovals(TimesheetMonth, deviceid, "1");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return objtimesheet;
            }
        }




        #region Emails for Timesheet Aprrovals by Manager Levels

        #region SendMails
        public string SendMailsForApprovals(string month,string deviceid, string SubmittedType)
        {
            Conn = new SqlConnection(str);
            DataTable dtaccMgr = new DataTable();
           

            List<manageremails> objManagerlist = new List<manageremails>();
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[Ax_GetManagerEmailIds]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@deviceid", deviceid);
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
                        ManagerId = dt.Rows[0]["L2_ManagerID"].ToString(),
                        EmailAppOrRejStatus = dt.Rows[0]["L2_ManagerID"].ToString(),
                        SubmittedFlag = dt.Rows[0]["L2_ManagerID"].ToString(),
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
                objCommand = new SqlCommand("[Ax_getTimeSheetEmailDetails]", Conn);
                objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@deviceid", deviceid);
                objCommand.Parameters.AddWithValue("@Timesheetmonth", Convert.ToDateTime(month).ToString("yyyy-MM-dd"));
                ds1 = new DataSet();
                da = new SqlDataAdapter(objCommand);
                da.Fill(ds1);
                dtheadings = new DataTable();
                dtData = new DataTable();
                dtheadings = ds1.Tables[0];
                dtData = ds1.Tables[1];
                string subject = string.Empty;
                if ((!string.IsNullOrEmpty(SubmittedType)) && (SubmittedType == "1"))
                {
                    
                    string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "1"));
                    SendMailsByMailGun(lstusers, objManagerlist, dtheadings, body, 1, "");

                }


                if ((!string.IsNullOrEmpty(SubmittedType)) && SubmittedType == "2")
                {
                    
                        string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "2"));
                        StatusMsg = SendMailsByMailGun(lstusers, objManagerlist, dtheadings, body, 2, "2");
                        return StatusMsg;
                   
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

                       // ToMuliId = new string[2] { "sreelakshmi.evolutyz@gmail.com", "" };
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

                      //  ToMuliId = new string[2] { "", "" };
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






        public StatusFlags CheckStatus(string Deviceid)
        {

            using (SqlConnection con = new SqlConnection(str))
            {
                StatusFlags objflags = new StatusFlags();

                int flag = -1;
                con.Open();
                try
                {
                   

                    SqlCommand cmd = new SqlCommand("AlexaCheckStatus", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@deviceid", Deviceid);

                    // cmd.Parameters.AddWithValue("@pin", pin);
                    cmd.Parameters.AddWithValue("@status", SqlDbType.Int);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters["@status"].Direction = ParameterDirection.Output;


                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    if (cmd.Parameters["@status"].Value != null)
                        flag = Int32.Parse(cmd.Parameters["@status"].Value.ToString());
                    if(flag==0)
                    {
                        objflags.Response = "0";
                    }
                    if(flag==1)
                    {
                        objflags.Response = "1";
                    }
                    if (flag == 2)
                    {
                        objflags.Response = "2";
                    }
                    if (flag == 3)
                    {
                        objflags.Response = "3";
                    }

                    //if (cmd.Parameters["@status"].Value != null)
                    //    objholidays.Status = Convert.ToInt32(cmd.Parameters["@status"].Value);

                    //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");


                }

                catch (Exception ex)
                {
                    throw ex;
                }
                return objflags;
              
            }
        }
        public Devices InsertDeviceId(string deviceId)
        {
            using (SqlConnection con = new SqlConnection(str))
            {


                Devices device = new Devices();
                try
                {
                    SqlCommand cmd = new SqlCommand("getdeviceID", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    con.Open();
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return device;
            }
        }
        public FetchUsers FetchData(string deviceid)
        {
            using (SqlConnection con = new SqlConnection(str))
            {
                FetchUsers objusers = new FetchUsers();
                try
                {
                    SqlCommand cmd = new SqlCommand("AlexaWakeUpProc", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@deviceid", deviceid);
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    // if(sdr.Read())
                    {
                        return (sdr.Read()) ? new FetchUsers
                        {
                            Name = sdr["Titleprefix"].ToString() + " " + sdr["UsrP_FirstName"].ToString() + " " + sdr["UsrP_LastName"].ToString(),
                            Mobilenumber = sdr["Usrp_MobileNumber"].ToString(),
                            Status = (sdr != null) ? 1 : 0
                        } : new FetchUsers
                        {
                            Name = string.Empty,
                            Mobilenumber = string.Empty,
                            Status = 0
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                // return objusers;
            }
        }
        public HelloMessage ValidateUser(string deviceid, string phonenumber/*,string pin*/)
        {

            using (SqlConnection con = new SqlConnection(str))
            {
                HelloMessage objmessage = new HelloMessage();
                int status = 0;
                try
                {
                    SqlCommand cmd = new SqlCommand("AlexaValidateUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@deviceid", deviceid);
                    cmd.Parameters.AddWithValue("@phnumber", phonenumber);
                    // cmd.Parameters.AddWithValue("@pin", pin);
                    cmd.Parameters.AddWithValue("@message", SqlDbType.Int);
                    cmd.Parameters["@message"].Direction = ParameterDirection.Output;
                    int i = cmd.ExecuteNonQuery();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (cmd.Parameters["@message"].Value != null)
                        status = Int32.Parse(cmd.Parameters["@message"].Value.ToString());

                    if (status == 1)
                    {
                        objmessage.Message = ResponseCodes.Exits;
                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return objmessage;
            }
        }
        public ValidatePhoneClass ValidatePhone(string deviceid, string phonenumber)
        {

            using (SqlConnection con = new SqlConnection(str))
            {
                ValidatePhoneClass objmessage = new ValidatePhoneClass();
                int status = 0;
                try
                {
                    SqlCommand cmd = new SqlCommand("AlexavalidatePhone", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@deviceid", deviceid);
                    cmd.Parameters.AddWithValue("@phnumber", phonenumber);
                    cmd.Parameters.AddWithValue("@message", SqlDbType.Int);
                    cmd.Parameters["@message"].Direction = ParameterDirection.Output;
                    // int i = cmd.ExecuteNonQuery();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (cmd.Parameters["@message"].Value != null)
                        status = Int32.Parse(cmd.Parameters["@message"].Value.ToString());

                    //if (status == 1)
                    //{
                    //    objmessage.Phone = phonenumber;
                    //    objmessage.Flag = ResponseCodes.Exits;
                    //}
                    //else
                    //{
                    //    objmessage.Phone = phonenumber;
                    //    objmessage.Flag = ResponseCodes.NotExists;
                    //}

                    return objmessage = (status == 1) ? new ValidatePhoneClass
                    {
                        Phone = phonenumber,
                        Flag = ResponseCodes.Exits
                    } : new ValidatePhoneClass
                    {
                        Phone = phonenumber,
                        Flag = ResponseCodes.NotExists
                    };

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //return objmessage;
            }
        }

        public Validatepinclass Validatepin(string deviceid, int pin)
        {

            using (SqlConnection con = new SqlConnection(str))
            {
                Validatepinclass objmessage = new Validatepinclass();
                int status = 0;
                try
                {
                    SqlCommand cmd = new SqlCommand("AlexavalidatePin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@deviceid", deviceid);
                    cmd.Parameters.AddWithValue("@pin", pin);
                    cmd.Parameters.AddWithValue("@message", SqlDbType.Int);
                    cmd.Parameters["@message"].Direction = ParameterDirection.Output;
                    // int i = cmd.ExecuteNonQuery();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (cmd.Parameters["@message"].Value != null)
                        status = Convert.ToInt32(cmd.Parameters["@message"].Value);

                    //if (status == 1)
                    //{
                    //    objmessage.Phone = phonenumber;
                    //    objmessage.Flag = ResponseCodes.Exits;
                    //}
                    //else
                    //{
                    //    objmessage.Phone = phonenumber;
                    //    objmessage.Flag = ResponseCodes.NotExists;
                    //}

                    return objmessage = (status == 1) ? new Validatepinclass
                    {
                        pin = pin,
                        Flag = ResponseCodes.Exits
                    } : new Validatepinclass
                    {
                        pin = pin,
                        Flag = ResponseCodes.NotExists
                    };

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //return objmessage;
            }
        }
        public HelloMessage ValidatePasscode(string deviceid, string phonenumber, string pin)
        {

            using (SqlConnection con = new SqlConnection(str))
            {
                HelloMessage objmessage = new HelloMessage();
                int status = 0;
                try
                {
                    SqlCommand cmd = new SqlCommand("AlexaValidatePasscode", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@deviceid", deviceid);
                    cmd.Parameters.AddWithValue("@phnumber", phonenumber);
                    cmd.Parameters.AddWithValue("@pin", pin);
                    cmd.Parameters.AddWithValue("@message", SqlDbType.Int);
                    cmd.Parameters["@message"].Direction = ParameterDirection.Output;
                    int i = cmd.ExecuteNonQuery();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (cmd.Parameters["@message"].Value != null)
                        status = Int32.Parse(cmd.Parameters["@message"].Value.ToString());

                    if (status == 1)
                    {
                        objmessage.Message = ResponseCodes.Exits;

                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return objmessage;
            }
        }


    }
}