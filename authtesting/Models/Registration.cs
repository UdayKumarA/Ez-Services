using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace authtesting.Models
{
    public class Registration
    {
        public string UserRegister(UserProperties objuser)
        {
            string str = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
            try
            {
                SqlConnection con = new SqlConnection(str);
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into Employee values(@firstname,@lastname,@position,@team)", con);
                cmd.Parameters.AddWithValue("@firstname", objuser.Firstname);
                cmd.Parameters.AddWithValue("@lastname", objuser.Lastname);
                cmd.Parameters.AddWithValue("@position", objuser.Position);
                cmd.Parameters.AddWithValue("@team", objuser.Team);
                int i = cmd.ExecuteNonQuery();
                if(i>0)
                {
                    return "Registered Successfully";
                }
                else
                {
                    return "Not Registered";
                }

            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public string UserLogin(UserLogin objuser)
        {
            string str = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
            try
            {
                SqlConnection con = new SqlConnection(str);
                con.Open();
                SqlCommand cmd = new SqlCommand("GetUserAuth", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", objuser.Username);
                cmd.Parameters.AddWithValue("@password", objuser.Password);
                int count =Convert.ToInt32(cmd.ExecuteScalar());
                //SqlDataAdapter sdr = cmd.ExecuteScalar();
                if (count > 0)
                {
                    return "success";
                }
                else
                {
                    return "Failed";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}