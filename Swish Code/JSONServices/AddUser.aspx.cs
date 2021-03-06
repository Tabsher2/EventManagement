﻿using System;
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

public partial class JSONServices_AddUser : System.Web.UI.Page
{

	public struct GenericRequest
	{
		public string username;
		public string email;
		public string pw;
		public string dob;
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
			if( request.email == null || request.username == null || request.pw == null || request.dob == null ){
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
			
			string sql = String.Format("SELECT * FROM users WHERE username=@un and email=@em");
			SqlCommand command = new SqlCommand( sql, connection );
			command.Parameters.Add(new SqlParameter("@un", request.username));
			command.Parameters.Add(new SqlParameter("@em", request.email));
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
			string salt = CreateSalt(15);
			request.pw = GenerateSHA256Hash(request.pw, salt);
			string sql = String.Format("INSERT into users (email,username,password,birthday,salt) VALUES ('{0}','{1}','{2}', '{3}', '{4}')", request.email, request.username, request.pw, request.dob, salt );
			SqlCommand command2 = new SqlCommand( sql, connection );
			command2.ExecuteNonQuery();
			response.message = "Successfully created account";

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
	
	private String CreateSalt(int size)
    {
        RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        byte[] buffer = new byte[size];
        rng.GetBytes(buffer);

        return Convert.ToBase64String(buffer);

    }

    private String GenerateSHA256Hash(String input, String salt)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input + salt);
        System.Security.Cryptography.SHA256Managed hashString = new System.Security.Cryptography.SHA256Managed();
        byte[] hash = hashString.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }

}