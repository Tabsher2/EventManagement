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

public partial class JSONServices_UploadImages : System.Web.UI.Page
{

	public struct ImageRequest
	{
		public string name;
		public string picture;
	}
	
	public struct ImageResponse
	{
		public string message;
		public string error;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		ImageRequest request;
		ImageResponse response = new ImageResponse();
		response.error = String.Empty;
		int uniID = 0;

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
			
			string sql = String.Format("SELECT * FROM University WHERE name=@un");
			SqlCommand command = new SqlCommand( sql, connection );
			command.Parameters.Add(new SqlParameter("@un", request.name));
			SqlDataReader reader = command.ExecuteReader();
			if( reader.Read() )
			{
				uniID = Convert.ToInt32( reader["uniID"] );
			}
			else
			{
				response.error = "No university found";
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

		if (response.error != "")
		{
			SendInfoAsJson(response);
			return;
		}
		
		//Insert image
		try
		{
			connection.Open();

			byte[] img = null;
			FileStream fs = new FileStream(request.picture, FileMode.Open, FileAccess.Read);
			BinaryReader br = new BinaryReader(fs);
			img = br.ReadBytes((int)fs.Length);

			string sql = String.Format("INSERT into uniToImage (uniID, picture) VALUES ('{0}','{1}')", uniID, img);
			SqlCommand command2 = new SqlCommand( sql, connection );
			command2.ExecuteNonQuery();

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

		response.message = "Successfully added images";
		
		
		SendInfoAsJson(response);
	}

	ImageRequest GetRequestInfo()
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
		ImageRequest request = JsonConvert.DeserializeObject<ImageRequest>(strJson);

		return (request);
	}

	void SendInfoAsJson(ImageResponse response)
	{
		string strJson = JsonConvert.SerializeObject(response);
		Response.ContentType = "application/json; charset=utf-8";
		Response.Write(strJson);
		Response.End();
	}
}