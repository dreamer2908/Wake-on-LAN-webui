using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class common
    {
        // query the database, return the number of rows affected, and output a datatable
        public static int queryDatabase(SqlCommand cmd, out DataTable dt)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            cmd.Connection = con;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }
    }
}