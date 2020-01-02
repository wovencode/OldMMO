// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using UnityEngine;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// MailManager
	/// <summary>
	/// This server-side class has only one purspoe: It is responsible for sending out
	/// confirmation emails to the users. If and how intensive this class is used, depends
	/// on the "Confirmation Settings" in the AccountManager of the server. This class does
	/// nothing else on it's own and is not linked to or dependant of any other Manager.
	/// </summary>
	// ===================================================================================
	[DisallowMultipleComponent]
	public partial class MailManager : BaseServerBehaviour
	{
		
		public enum MailTemplateType { ForgotPassword, SecurityCode }
		
		[Header("---------- eMail Credentials ----------")]
		public string eMailAddress;
		public string eMailPassword;
		public string smtpServerHost;
		public int smtpPort;
		public bool smtpEnableSSL;
		
		[Header("---------- eMail Settings ----------")]
		public string eMailSubject			= "Service Mail from OpenMMO";
				
		[Header("---------- eMail Templates ----------")]
		public string filePath		= "eMail/";
		public MailTemplate[] mailTemplates;
		
		protected Dictionary<int, TextAsset> dictMailTemplates = new Dictionary<int, TextAsset>();
				
		// -------------------------------------------------------------------------------
		// Awake
		// -------------------------------------------------------------------------------
		void Awake()
		{
#if OM_SERVER
			Startup();
#endif
		}
		
		// -------------------------------------------------------------------------------
		// Startup
		// -------------------------------------------------------------------------------
		public void Startup()
		{
			AutoLoadTemplates();
		}
		
		// -------------------------------------------------------------------------------
		// AutoLoadTemplates
		// -------------------------------------------------------------------------------
		protected void AutoLoadTemplates()
		{
			foreach (MailTemplate mailTemplate in mailTemplates)
			{
				int idx = (int)Enum.Parse(typeof(MailTemplateType), mailTemplate.template.ToString());
				dictMailTemplates.Add(idx, Resources.Load<TextAsset>(filePath+mailTemplate.name));
			}
		}
		
		// -------------------------------------------------------------------------------
		// GetMailBody
		// -------------------------------------------------------------------------------
		protected string GetMailBody(MailTemplateType templateType, int nCode)
		{
			
			TextAsset textAsset;
			string sBody = "";
			
			if (dictMailTemplates.TryGetValue((int)templateType, out textAsset))
			{
				sBody = textAsset.ToString();
				sBody = sBody.Replace("{0}", nCode.ToString());
			}
			
			return sBody;
			
		}
		
		// -------------------------------------------------------------------------------
		// SendMail
		// -------------------------------------------------------------------------------
		protected void SendMail(string sRecipientEmailAddress, MailTemplateType templateType, int nCode)
		{
			
			MailMessage message = new MailMessage();
 
			message.To.Add(sRecipientEmailAddress);
			message.From 				= new MailAddress(eMailAddress);
			message.Subject 			= eMailSubject;
			message.IsBodyHtml 			= true;
			message.Body 				= GetMailBody(templateType, nCode);

			SmtpClient smtp = new SmtpClient();
			
			smtp.UseDefaultCredentials 	= false;
			smtp.Host 					= smtpServerHost;
			smtp.Port 					= smtpPort;
			smtp.EnableSsl 				= smtpEnableSSL;
			smtp.Credentials 			= new NetworkCredential(eMailAddress, eMailPassword) as ICredentialsByHost;
			
			smtp.SendAsync(message, message);

		}
		
		// -------------------------------------------------------------------------------
		// SendMailForgotPassword
		// -------------------------------------------------------------------------------
		public void SendMailForgotPassword(string sRecipientEmailAddress, int nCode)
		{
			SendMail(sRecipientEmailAddress, MailTemplateType.ForgotPassword, nCode);
		}
		
		// -------------------------------------------------------------------------------
		// SendMailSecurityCode
		// -------------------------------------------------------------------------------
		public void SendMailSecurityCode(string sRecipientEmailAddress, int nCode)
		{
			SendMail(sRecipientEmailAddress, MailTemplateType.SecurityCode, nCode);
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}