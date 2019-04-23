using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.IO;
using RestSharp;
using RestSharp.Authenticators;
using System.Data;

namespace authtesting.Models
{
    public class EmailFormats
    {
        timesheet lstusers = new timesheet();
        string[] arr = new string[4];
        string resultbody1 = string.Empty, resultbody2 = string.Empty;


        private string createEmailBody(timesheet lstusers, string bodylist)
        {
            string body = string.Empty;

            //using streamreader for reading my htmltemplate   

            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/email.html")))
            {

                if (lstusers.ManagerEmail1 != "0" && lstusers.ManagerEmail2 != "0")
                {
                    body += reader.ReadToEnd();
                }
            }
            // return body + "$" + resultbody1 + "$" + resultbody2;
            return body + bodylist;
        }


        public string SendEmail(timesheet lstusers, string bodydata)

        {
            //calling for creating the email body with html template   

            string body = this.createEmailBody(lstusers, bodydata);
            // var IRestResponse = SendMailsByMailGun(lstusers, subject, bodydata, flag, ActionType);
            return body;
        }

        public bool SendHtmlFormattedEmail(timesheet lstusers, string MonthName, string body, int flag, string ActionType)
        {
            string[] ToMuliId = new string[0];
            string HostAdd = ConfigurationManager.AppSettings["HostName"].ToString();
            string mail = ConfigurationManager.AppSettings["username"];
            string value = ConfigurationManager.AppSettings["password"];
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailmessage = new MailMessage();
            NetworkCredential networkCredential = new NetworkCredential();
            try
            {
                if (flag == 1)
                {
                    ToMuliId = new string[2] { lstusers.ManagerEmail1.ToString(), lstusers.AccManagerEmail };
                    if (body.Contains('$'))
                    {
                    }
                    for (int i = 0; i < ToMuliId.Length; i++)
                    {
                        mailmessage = new MailMessage(mail, ToMuliId[i]);

                        if (i == 0)
                        {
                            mailmessage.Subject = "TimeSheet of " + lstusers.UserName + " for Month " + MonthName + " is  Submitted  and Waiting for your approval";

                            mailmessage.Body = resultbody1;

                        }

                        if (i == 1)
                        {
                            mailmessage.Subject = "TimeSheet of " + lstusers.UserName + "  for Month " + MonthName + " is Submitted To Managers Levels";
                            mailmessage.Body = resultbody2;

                        }

                        //mailmessage.Body = body;
                        mailmessage.IsBodyHtml = true;
                        smtpClient.Host = "smtp.gmail.com";
                        smtpClient.EnableSsl = true;
                        networkCredential = new NetworkCredential(mail, value);
                        smtpClient.UseDefaultCredentials = true;
                        smtpClient.Credentials = networkCredential;
                        smtpClient.Port = 587;
                        smtpClient.Send(mailmessage);
                    }
                }
                if (flag == 2)
                {

                    ToMuliId = new string[3] { lstusers.ManagerEmail1.ToString(), lstusers.AccManagerEmail, lstusers.UserEmailId };
                    if (body.Contains('$'))
                    {
                        string[] arr = body.Split('$');
                        resultbody1 = arr[0] + arr[1];
                        resultbody2 = arr[0] + arr[2];

                    }
                    for (int i = 0; i < ToMuliId.Length; i++)
                    {
                        mailmessage = new MailMessage(mail, ToMuliId[i]);
                        if (ActionType == "A")
                        {
                            mailmessage.Subject = "TimeSheet of " + lstusers.UserName + " for Month " + MonthName + " is  Accepted by Level_1 Manager";
                        }
                        else
                        {
                            mailmessage.Subject = "TimeSheet of " + lstusers.UserName + " for Month " + MonthName + " is  Rejected  by Level_1 Manager";
                        }

                        if (i == 0)
                        {

                            mailmessage.Body = resultbody1;

                        }

                        if (i == 1 || i == 3)
                        {

                            mailmessage.Body = resultbody2;

                        }

                        //mailmessage.Body = body;
                        mailmessage.IsBodyHtml = true;
                        smtpClient.Host = "smtp.gmail.com";
                        smtpClient.EnableSsl = true;
                        networkCredential = new NetworkCredential(mail, value);
                        smtpClient.UseDefaultCredentials = true;
                        smtpClient.Credentials = networkCredential;
                        smtpClient.Port = 587;
                        smtpClient.Send(mailmessage);
                    }
                }
                else if (flag == 3)
                {
                    ToMuliId = new string[3] { lstusers.ManagerEmail2, lstusers.AccManagerEmail, lstusers.UserEmailId };
                    if (body.Contains('$'))
                    {
                        string[] arr = body.Split('$');
                        resultbody1 = arr[0] + arr[1];
                        resultbody2 = arr[0] + arr[2];

                    }
                    for (int i = 0; i < ToMuliId.Length; i++)
                    {
                        mailmessage = new MailMessage(mail, ToMuliId[i]);


                        if (i == 0)
                        {
                            mailmessage.Subject = "TimeSheet of " + lstusers.UserName + " for Month " + MonthName + " is  Accepted by Level-1 Manager";

                            mailmessage.Body = resultbody1;

                        }

                        if ((i == 1) || (i == 2))
                        {
                            mailmessage.Subject = "TimeSheet of " + lstusers.UserName + "  for Month " + MonthName + " is  Accepted by Level-1 Manager";
                            mailmessage.Body = resultbody2;

                        }

                        //mailmessage.Body = body;
                        mailmessage.IsBodyHtml = true;
                        smtpClient.Host = "smtp.gmail.com";
                        smtpClient.EnableSsl = true;
                        networkCredential = new NetworkCredential(mail, value);
                        smtpClient.UseDefaultCredentials = true;
                        smtpClient.Credentials = networkCredential;
                        smtpClient.Port = 587;
                        smtpClient.Send(mailmessage);
                    }
                }
                return true;
            }


            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;

            }
        }

    }
}