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

public partial class JSONServices_AddRSO : System.Web.UI.Page
{

	public struct RSORequest
	{
		public string name;
		public string description;
		public string member1;
		public string member2;
		public string member3;
		public string member4;
		public string member5;
		public int admin;
	}
	
	public struct RSOResponse
	{
		public string message;
		public string error;
	}

	public struct User
	{
		public int adminID;
		public int member1ID;
		public int member2ID;
		public int member3ID;
		public int member4ID;
		public int member5ID;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		RSORequest request;
		RSOResponse response = new RSOResponse();
		User idStorage = new User();
		int selectedUniversity = -1;
		int rsoID = -1;
		string adminEmail = String.Empty;
		response.error = String.Empty;

		// Need passed in store id and number of requested results.
		// 1. Deserialize the incoming Json.
		try
		{
			request = GetRequestInfo();
			if( request.name == null ||  request.description == null || request.member1 == null || request.member2 == null
				|| request.member3 == null || request.member4 == null || request.member5 == null || request.admin == null) {
				response.error = "RSO not created";
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
		//Ensure the Student Organization isn't already taken
		SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
		try
		{
			connection.Open();
			
			string sql = String.Format("SELECT * FROM StudentOrg WHERE name=@un");
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

		//Retrieve the student IDs based on the e-mails provided
		try
		{
			connection.Open();
			
			string sql = String.Format("SELECT * FROM EventUser");
			SqlCommand command2 = new SqlCommand( sql, connection );
			SqlDataReader reader2 = command2.ExecuteReader();
			while( reader2.Read() )
			{
				string matchEmail;
				int thisID;
				matchEmail = Convert.ToString( reader2["email"] );
				thisID = Convert.ToInt32( reader2["userID"]);
				if (matchEmail.Equals(request.member1))
					idStorage.member1ID = thisID;
				else if(matchEmail.Equals(request.member2))
					idStorage.member2ID = thisID;
				else if(matchEmail.Equals(request.member3))
					idStorage.member3ID = thisID;
				else if(matchEmail.Equals(request.member4))
					idStorage.member4ID = thisID;
				else if(matchEmail.Equals(request.member5))
					idStorage.member5ID = thisID;
				else if (request.admin == thisID)
					adminEmail = matchEmail;
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

		//Create the student organization in the database
		try
		{
			connection.Open();
			
			string sql = String.Format("INSERT into StudentOrg (name, description, admin) VALUES ('{0}','{1}','{2}')", request.name, request.description, request.admin);
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

		//Find out what University the students go to
		try
		{
			connection.Open();
			
			string sql = String.Format("SELECT * from uniToUser WHERE userID=@ai");
			SqlCommand command4 = new SqlCommand( sql, connection );
			command4.Parameters.Add(new SqlParameter("@ai", request.admin));
			SqlDataReader reader4 = command4.ExecuteReader();
			if ( reader4.Read() )
			{
				selectedUniversity = Convert.ToInt32( reader4["uniID"] );
			}
			else
				response.error = "You are not registered to any University. Failed to register organization.";
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

		//Retrieve what ID we gave the RSO
		try
		{
			connection.Open();
			
			string sql = String.Format("SELECT * FROM StudentOrg WHERE name=@on");
			SqlCommand command5 = new SqlCommand( sql, connection );
			command5.Parameters.Add(new SqlParameter("@on", request.name));
			SqlDataReader reader5 = command5.ExecuteReader();
			if ( reader5.Read() )
			{
				rsoID = Convert.ToInt32( reader5["rsoID"] );
			}
			else
				response.error = "Failed to retrieve Student Organization id.";
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

		//Assign the rso to the school
		try
		{
			connection.Open();
			
			string sql = String.Format("INSERT into uniToRso (uniID, rsoID) VALUES ('{0}', '{1}')", selectedUniversity, rsoID);
			SqlCommand command5 = new SqlCommand( sql, connection );
			command5.ExecuteNonQuery();
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

		//Assign the students to the rso
		try
		{
			connection.Open();
			
			string sql = String.Format("INSERT into rsoToUsers (rsoID, userID, groupMember, userEmail) VALUES ('{0}', '{1}', '{2}', '{3}'), ('{4}', '{5}', '{6}', '{7}'), ('{8}', '{9}', '{10}', '{11}'), ('{12}', '{13}', '{14}', '{15}'), ('{16}', '{17}', '{18}', '{19}'), ('{20}', '{21}', '{22}', '{23}')",
										rsoID, request.admin, 1, adminEmail, rsoID, idStorage.member1ID, 1, request.member1, rsoID, idStorage.member2ID, 1, request.member2, rsoID, idStorage.member3ID, 1, request.member3, rsoID, idStorage.member4ID, 1, request.member4,
										 rsoID, idStorage.member5ID, 1, request.member5);
			SqlCommand command6 = new SqlCommand( sql, connection );
			command6.ExecuteNonQuery();
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

		response.message = "Successfully registered Student Organization!";
		
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