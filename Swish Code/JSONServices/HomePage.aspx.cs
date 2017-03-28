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

public partial class JSONServices_HomePage : System.Web.UI.Page
{
	
	public struct Individual
	{
		public string username;
		public int score;
		
		public Individual(String u, int s){
			this.username = u;
			this.score = s;
		}
	}
	
	public struct LoginResponse
	{
		public List<Individual> indivs;
		public string error;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		LoginResponse response = new LoginResponse();
		response.error = String.Empty;
		

		// Do stuff here.
		SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
		try
		{
			connection.Open();

			response.indivs = new List<Individual>();
			string sql = String.Format("SELECT currentWinStreak,username FROM users ORDER BY currentWinStreak DESC" );
			SqlCommand command = new SqlCommand( sql, connection );
			SqlDataReader reader = command.ExecuteReader();
			int i = 0;
			while( reader.Read() )
			{
				if( i >= 10 )
					break;
				string curUsername = (Convert.ToString( reader["username"]));
				int curScore =  (Convert.ToInt32( reader["currentWinStreak"]));
				response.indivs.Add(new Individual(curUsername,curScore));
				i++;
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


	void SendInfoAsJson(LoginResponse response)
	{
		string strJson = JsonConvert.SerializeObject(response);
		Response.ContentType = "application/json; charset=utf-8";
		Response.Write(strJson);
		Response.End();
	}

}