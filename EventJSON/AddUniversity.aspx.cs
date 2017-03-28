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

public partial class JSONServices_AddUniversity : System.Web.UI.Page
{

	public struct UniversityRequest
	{
		public string name;
		public string location;
		public string description;
		public int numStudents;
		public int admin;
	}
	
	public struct UniversityResponse
	{
		public string message;
		public string error;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		UniversityRequest request;
		UniversityResponse response = new UniversityResponse();
		response.error = String.Empty;

		// Need passed in store id and number of requested results.
		// 1. Deserialize the incoming Json.
		try
		{
			request = GetRequestInfo();
			if( request.name == null || request.location == null || request.description == null || request.numStudents == null ){
				response.error = "University not created";
				SendInfoAsJson(response);
				
				return;
			}
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
			
			string sql = String.Format("SELECT * FROM University WHERE name=@un");
			SqlCommand command = new SqlCommand( sql, connection );
			command.Parameters.Add(new SqlParameter("@un", request.name));
			SqlDataReader reader = command.ExecuteReader();
			if( reader.Read() )
			{
				response.error = "1";
			}
			
			if( response.error != "")
			{
				SendInfoAsJson(response);
				return;
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
		
		try
		{
			connection.Open();
			string sql = String.Format("INSERT into University (name,location,description,numStudents, superAdmin) VALUES ('{0}','{1}','{2}', '{3}', '{4}')", request.name, request.location, request.description, request.numStudents, request.admin);
			SqlCommand command2 = new SqlCommand( sql, connection );
			command2.ExecuteNonQuery();
			response.message = "Successfully added university";

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

	UniversityRequest GetRequestInfo()
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
		UniversityRequest request = JsonConvert.DeserializeObject<UniversityRequest>(strJson);

		return (request);
	}

	void SendInfoAsJson(UniversityResponse response)
	{
		string strJson = JsonConvert.SerializeObject(response);
		Response.ContentType = "application/json; charset=utf-8";
		Response.Write(strJson);
		Response.End();
	}
}