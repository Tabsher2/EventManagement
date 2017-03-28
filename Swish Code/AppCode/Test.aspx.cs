using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

public partial class JSONServices_Test : System.Web.UI.Page
{


	public struct LoginResponse
	{
		public string message;
		public string error;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		LoginResponse response = new LoginResponse();
		response.error = String.Empty;

		// Need passed in store id and number of requested results.
		// 1. Deserialize the incoming Json.
		
		// Done deserializing...

		
		
		// Do stuff here.
		SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
		try
		{
			connection.Open();

			string sql = String.Format("SELECT * FROM users WHERE username='MartyMcSoar'" );
			SqlCommand command = new SqlCommand( sql, connection );
			//command.Parameters.Add(new SqlParameter("@un", request.uname));
			//command.Parameters.Add(new SqlParameter("@pw", request.pw));
			SqlDataReader reader = command.ExecuteReader();
			if( reader.Read() )
			{
				response.message = Convert.ToString( reader["username"] );
			}
			else
			{
				response.error = "No matching record";
			}
			reader.Close();
		}
		catch (Exception ex)
		{
			response.error = ex.Message.ToString();
		}
		finally
		{
			if (connection.State == ConnectionState.Open)
			{
				connection.Close();
			}
		}

		SendInfoAsJson(response);
	}

	void SendInfoAsJson(LoginResponse response)
	{
		string strJson = JsonConvert.SerializeObject(response);
		Response.ContentType = "application/json; charset=utf-8";
		Response.Write(strJson);
		Response.End();
	}

}