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

public partial class JSONServices_JoinRSO : System.Web.UI.Page
{

	public struct RSORequest
	{
		public int userID;
		public string rsoName;
	}
	
	public struct RSOResponse
	{
		public string message;
		public string error;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		RSORequest request;
		RSOResponse response = new RSOResponse();
		string email = String.Empty;
		int rsoID = -1;
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
		//Get the User's email
		SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
		try
		{
			connection.Open();
			
			string sql = String.Format("SELECT * FROM EventUser WHERE userID=@un");
			SqlCommand command = new SqlCommand( sql, connection );
			command.Parameters.Add(new SqlParameter("@un", request.userID));
			SqlDataReader reader = command.ExecuteReader();
			if( reader.Read() )
			{
				email = Convert.ToString( reader["email"] );
			}
			
			if( response.error != "")
			{
				SendInfoAsJson(response);
				return;
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

		//Get the rso's ID
		try
		{
			connection.Open();
			
			string sql = String.Format("SELECT * FROM StudentOrg WHERE name=@rs");
			SqlCommand command2 = new SqlCommand( sql, connection );
			command2.Parameters.Add(new SqlParameter("@rs", request.rsoName));
			SqlDataReader reader2 = command2.ExecuteReader();
			if( reader2.Read() )
			{
				rsoID = Convert.ToInt32( reader2["rsoID"] );
			}
			
			if( response.error != "")
			{
				SendInfoAsJson(response);
				return;
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

		//Add this user to the rsoToUsers table but not as a full member yet
		try
		{
			connection.Open();
			
			string sql = String.Format("INSERT into rsoToUsers (rsoID, userID, groupMember, userEmail) VALUES ('{0}', '{1}', '{2}', '{3}')", rsoID, request.userID, 0, email);
			SqlCommand command3 = new SqlCommand( sql, connection );
			command3.ExecuteNonQuery();
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
		
		response.message = "Request sent!";
		
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