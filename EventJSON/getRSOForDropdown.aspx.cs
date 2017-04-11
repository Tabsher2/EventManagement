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
using System.Net.Mail;

public partial class JSONServices_Contact : System.Web.UI.Page
{

	public struct GenericRequest
	{
		public int userID;
	}
	
	public struct RSO
	{
		public string name;
		public int id;
		
		public RSO(string s, int i){
			name = s;
			id = i;
		}
	}
	
	public struct GenericResponse
	{
		public int value;
		public List<RSO> rsos;
		public string error;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		GenericRequest request;
		GenericResponse response = new GenericResponse();
		response.error = String.Empty;

		// Need passed in store id and number of requested results.
		// 1. Deserialize the incoming Json.
		try
		{
			request = GetRequestInfo();
		}
		catch (Exception ex)
		{
			response.error = ex.Message.ToString();

			// Return the results as Json.
			SendInfoAsJson(response);

			return;
		}

		// Do stuff here.
		
		SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
		try
		{
			connection.Open();
			
			response.rsos = new List<RSO>();
			string sql = String.Format("SELECT S.name, S.rsoID FROM StudentOrg S, rsoToUsers R WHERE R.userID=@un AND S.rsoID=R.rsoID");
			SqlCommand command = new SqlCommand( sql, connection );
			command.Parameters.Add(new SqlParameter("@un", request.userID));
			SqlDataReader reader = command.ExecuteReader();
			while( reader.Read() )
			{
				response.rsos.Add(new RSO(Convert.ToString(reader["name"]),Convert.ToInt32(reader["rsoID"])));
			}

			
			/*
#if CRAP	
			response.error = sql;
			SendInfoAsJson(response);
			return;
#endif			
			*/
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