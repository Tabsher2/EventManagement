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

public partial class JSONServices_LoadUser : System.Web.UI.Page
{

	public struct GenericRequest
	{
		public string username;
		public string email, password;
		public string dob;
	}
	
	public struct GenericResponse
	{
		public List<string> users;
		public string error;
	}
	

	protected void Page_Load(object sender, EventArgs e)
	{
		GenericRequest request;
		GenericResponse response = new GenericResponse();
		response.error = String.Empty;

		request = GetRequestInfo();

		// Do stuff here.
		SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
		try
		{
			connection.Open();
			
			
			response.users = new List<string>();
			string sql = String.Format("SELECT * FROM users WHERE username=@un email=@em");
			SqlCommand command = new SqlCommand( sql, connection );
			command.Parameters.Add(new SqlParameter("@un", request.username));
			command.Parameters.Add(new SqlParameter("@em", request.email));
			SqlDataReader reader = command.ExecuteReader();
			if( reader.Read() )
			{
				response.error = "The username or email is taken";
			}
			else
			{
				
			}

			
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

	GenericRequest GetRequestInfo()
	{
		// Get the Json from the POST.
		string strJson = String.Empty;
		HttpContext context = HttpContext.Current;
		context.Request.InputStream.Position = 0;
		using (StreamReader inputStream = new StreamReader(context.Request.InputStream))
		{
			strJson = inputStream.ReadToEnd();
		}

		// Deserialize the Json.
		GenericRequest request = JsonConvert.DeserializeObject<GenericRequest>(strJson);

		return (request);
	}

	void SendInfoAsJson(GenericResponse response)
	{
		string strJson = JsonConvert.SerializeObject(response);
		Response.ContentType = "application/json; charset=utf-8";
		Response.Write(strJson);
		Response.End();
	}

}