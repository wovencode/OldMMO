// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelAccountChangeMail
	// ===================================================================================
	public partial class OM_UI_PanelAccountChangeMail : OM_UI_Panel {
		
		[Header("---------- [Required] Feedback Messages ----------")]
		public string msgError 			= "Missing or incorrect data provided!";
		public string msgFail 			= "Failed!";
		public string msgSuccess		= "Success!";
		
		[Header("---------- [Required] UI Elements ----------")]
	    public InputField inputEmailOld;
	    public InputField inputEmail;
		public InputField inputPassword;
		public Button buttonChange;
		
		public OM_UI_PanelMain 			panelMain;
		public OM_UI_PanelMessage 		panelMessage;
		public OM_UI_PanelSecurityCode 	panelSecurityCode;
		
		protected string sCurrentMail;
		protected string sNewMail;
		
		// -------------------------------------------------------------------------------
		// OnChildEnable
		// -------------------------------------------------------------------------------
		public override void OnChildEnable() {
			
			if (!panelMain) 			panelMain 				= FindObjectOfType<OM_UI_PanelMain>();
			if (!panelMessage) 			panelMessage 			= FindObjectOfType<OM_UI_PanelMessage>();
			if (!panelSecurityCode) 	panelSecurityCode 		= FindObjectOfType<OM_UI_PanelSecurityCode>();
			
			sCurrentMail			= clientManager.clientAccount.sMail;
			inputEmailOld.text 		= sCurrentMail;
			inputEmail.text 		= "";
			inputPassword.text 		= "";
		}
		
		// -------------------------------------------------------------------------------
		// OnInvokeRepeating
		// -------------------------------------------------------------------------------
		public override void OnInvokeRepeating() {}
		
		// -------------------------------------------------------------------------------
		// SlowUpdate
		// -------------------------------------------------------------------------------
		public override void SlowUpdate() {}
		
		// -------------------------------------------------------------------------------
		// ClickChangeMail
		// -------------------------------------------------------------------------------	
		public void ClickChangeMail() {
			
			if (inputEmail != null &&
				inputPassword != null) {
			
				if (inputEmail.text.validateEmail() &&
					inputPassword.text.validatePassword() &&
					inputEmail.text != sCurrentMail
					) {
					
					CallbackConfirmAccountChangeMail();
    			
    			} else {
    				panelMessage.Show(msgError);
    			}
    		
    		} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}
    		
		}
		
		// -------------------------------------------------------------------------------
		// CallbackAccountChangeMail
		// -------------------------------------------------------------------------------
		public void CallbackAccountChangeMail(string[] result) {
		
			if (result[0] == Constants.INT_FAILURE.ToString())
			{
				panelMessage.Show(msgFail);
			}
			else if (result[0] == Constants.INT_CONFIRM.ToString())
			{
				panelSecurityCode.Init(Constants.AccountActionType.ChangeMail, CallbackConfirmAccountChangeMail);
			}
			else if (result[0] == Constants.INT_SUCCESS.ToString())
			{
				clientManager.clientAccount.sMail = sNewMail;
				inputEmailOld.text 		= sNewMail;
				inputEmail.text 		= "";
				inputPassword.text 		= "";
				panelMessage.Show(msgSuccess);
				Hide();
			}
		}
		
		// -------------------------------------------------------------------------------
		// CallbackConfirmAccountChangeMail
		// -------------------------------------------------------------------------------	
		public void CallbackConfirmAccountChangeMail(bool bSuccess = true)
		{
			if (bSuccess)
			{
				sNewMail = inputEmail.text;
				string[] fields = new string[] { inputEmail.text, inputPassword.text };
   			 	TemporaryDisable(buttonChange);
    			clientManager.ReqAccountChangeMail(fields, CallbackAccountChangeMail);
			}
		}
		
		// -------------------------------------------------------------------------------
		// ClickCancel
		// -------------------------------------------------------------------------------
		public void ClickCancel() {
			Hide();
		}

		// -------------------------------------------------------------------------------
	    
	}

}

// =======================================================================================