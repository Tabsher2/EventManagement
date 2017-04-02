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
using System.Security.Cryptography;

public partial class JSONServices_LoadRSO : System.Web.UI.Page
{

	public struct RSORequest
	{
		public int uniID;
	}
	
	public struct RSOResponse
	{
		public List<string> rsos;
		public string error;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		RSORequest request;
		RSOResponse response = new RSOResponse();
		response.error = String.Empty;

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
		//Retrieve all Student Organizations at this student's university
		SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
		try
		{
			connection.Open();
			
			response.rsos = new List<string>();
			string sql = String.Format("SELECT * FROM StudentOrg WHERE uniID=@un");
			SqlCommand command = new SqlCommand( sql, connection );
			command.Parameters.Add(new SqlParameter("@un", request.uniID));
			SqlDataReader reader = command.ExecuteReader();
			while ( reader.Read() )
			{
				response.rsos.Add(Convert.ToString( reader["name"] ));
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

	RSORequest GetRequestInfo()
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
		RSORequest request = JsonConvert.DeserializeObject<RSORequest>(strJson);

		return (request);
	}

	void SendInfoAsJson(RSOResponse response)
	{
		string strJson = JsonConvert.SerializeObject(response);
		Response.ContentType = "application/json; charset=utf-8";
		Response.Write(strJson);
		Response.End();
	}
}