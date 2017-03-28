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
		public string name;
		public string email;
		public string subject;
		public string body;
	}
	
	public struct GenericResponse
	{
		public int value;
		public string text;
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
		string text = System.IO.File.ReadAllText(@"C:\SwishGameRoot\JSONServices\email.config");
		int startPos = text.IndexOf(">");
		int endPos = text.LastIndexOf("<");
		string pass = text.Substring(startPos+1, endPos-7);
		MailMessage mail = new MailMessage();
		
		//Setting From , To and CC
		mail.From = new MailAddress("swishgamehelp@gmail.com", request.name);
		mail.To.Add(new MailAddress("swishgamehelp@gmail.com", "Swish Team"));
		mail.Body = "Name: " + request.name + "\nEmail: " + request.email + "\nSubject: " + request.subject + "\nBody: " + request.body;
		mail.Sender = new MailAddress(request.email, request.name);
		mail.Subject = request.subject;
		mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

		
		try
		{
			SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
			smtpClient.EnableSsl = true;
			smtpClient.UseDefaultCredentials = false;
			smtpClient.Credentials = new System.Net.NetworkCredential("swishgamehelp@gmail.com", pass);
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			
			smtpClient.Send(mail.From.ToString(), mail.To.ToString(), mail.Subject, mail.Body);
		}
		catch(SmtpException ex)
		{
			string msg = "Mail cannot be sent: ";
			msg += ex.Message;
			response.error = msg;
			SendInfoAsJson(response);
			
			return;
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