using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Data.SqlClient;

namespace authtesting
{
    public class cls_Log
    {
        string str = ConfigurationManager.ConnectionStrings["EvolutyzCornerDBEntities"].ConnectionString;
       // string str = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        

        bool EnableLogs = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableLogs"].ToString());
        string LogPath = @"C:\TimeSheet_test\";// ConfigurationManager.AppSettings["Logpath"].ToString();
        //string LogPath = @"C:Evo\TimeSheet_test\";
        string filename = "TimeSheetServices.log";
        public void AddtoLogFile(string Source, string Method, string Errormessage)
        {
            if (EnableLogs)
            {
                string filepath = LogPath + filename;
                if (File.Exists(filepath))
                {
                    using (StreamWriter writer = new StreamWriter(filepath, true))
                    {
                        writer.WriteLine("-------------------START-------------" + DateTime.Now);
                        writer.WriteLine("Source :" + Source);
                        writer.WriteLine("Method :" + Method);
                        writer.WriteLine("Error :" + Errormessage);
                        writer.WriteLine("-------------------END-------------" + DateTime.Now);
                    }
                }
                else
                {
                    StreamWriter writer = File.CreateText(filepath);
                    writer.WriteLine("-------------------START-------------" + DateTime.Now);
                    writer.WriteLine("Source :" + Source);
                    writer.WriteLine("Method :" + Method);
                    writer.WriteLine("Error :" + Errormessage);
                    writer.WriteLine("-------------------END-------------" + DateTime.Now);
                    writer.Close();
                }


            }
            SqlConnection con = new SqlConnection(str);
            con.Open();
            SqlCommand cmd = new SqlCommand("InsertErrorLogs", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@sourcename", Source);
            cmd.Parameters.AddWithValue("@methodname", Method);
            cmd.Parameters.AddWithValue("@errormessage", Errormessage);
            cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }

}