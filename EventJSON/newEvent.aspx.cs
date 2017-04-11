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

public partial class JSONServices_newEvent : System.Web.UI.Page
{

	public struct GenericRequest
	{
		public int rso;
		public string name;
		public string category;
		public string description;
		public string time;
		public string date;
		public string loc;
		public string phone;
		public string email;
		public int privacy;
	}
	
	public struct GenericResponse
	{
		public string message;
		public string error;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		GenericRequest request;
		GenericResponse response = new GenericResponse();
		response.error = String.Empty;
		
#if CRAP		
		string strJson = String.Empty;
		HttpContext context = HttpContext.Current;
		context.Request.InputStream.Position = 0;
		using (StreamReader inputStream = new StreamReader(context.Request.InputStream))
		{
			strJson = inputStream.ReadToEnd();
		}
		response.error = strJson;
		SendInfoAsJson(response);
		return;
#endif

		// Need passed in store id and number of requested results.
		// 1. Deserialize the incoming Json.
		try
		{
			request = GetRequestInfo();
			if( request.rso == null || request.name == null || request.category == null || request.description == null || request.time == null || request.date == null || request.loc == null || request.phone == null || request.email == null || request.privacy == null ){
				response.error = "Account not created";
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
			
			string sql = String.Format("INSERT into EventTable (rsoID, name, category, description, eventTime, eventDate, location, contactPhone, contactEmail, eventPrivacy) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
										request.rso, request.name, request.category, request.description, request.time, request.date, request.loc, request.phone, request.email, request.privacy);
			SqlCommand command = new SqlCommand( sql, connection );
			command.ExecuteNonQuery();

			
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