// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelAccountForgotPassword
	// ===================================================================================
	public partial class OM_UI_PanelAccountForgotPassword : OM_UI_Panel {
		
		[Header("---------- [Required] Feedback Messages ----------")]
		public string msgError 			= "Missing or incorrect data provided!";
		public string msgFail 			= "Failed!";
		public string msgSuccess		= "Success!";
		
		[Header("---------- [Required] UI Elements ----------")]
	    public InputField inputAccountName;
		public Button buttonForgot;
		
		public OM_UI_PanelMain 			panelMain;
		public OM_UI_PanelMessage 		panelMessage;
		public OM_UI_PanelSecurityCode 	panelSecurityCode;
		
		// -------------------------------------------------------------------------------
		// OnChildEnable
		// -------------------------------------------------------------------------------
		public override void OnChildEnable() {
			if (!panelMain) 			panelMain 				= FindObjectOfType<OM_UI_PanelMain>();
			if (!panelMessage) 			panelMessage 			= FindObjectOfType<OM_UI_PanelMessage>();
			if (!panelSecurityCode) 	panelSecurityCode 		= FindObjectOfType<OM_UI_PanelSecurityCode>();
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
		// ClickForgotPassword
		// -------------------------------------------------------------------------------	
		public void ClickForgotPassword() {
		
			if (inputAccountName != null)
			{
				if (inputAccountName.text.validateName())
				{
					
					CallbackConfirmAccountForgotPassword();
    			
    			} else {
    				panelMessage.Show(msgError);
    			}
    		
    		} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}		
		
		}
		
		// -------------------------------------------------------------------------------
		// CallbackAccountForgotPassword
		// -------------------------------------------------------------------------------
		public void CallbackAccountForgotPassword(string[] result) {
Debug.Log("CallbackAccountForgotPassword"+result[0]);
			if (result[0] == Constants.INT_FAILURE.ToString())
			{
				panelMessage.Show(msgFail);
			}
			else if (result[0] == Constants.INT_CONFIRM.ToString())
			{
				panelSecurityCode.Init(Constants.AccountActionType.ForgotPassword, CallbackConfirmAccountForgotPassword);
			}
			else if (result[0] == Constants.INT_SUCCESS.ToString())
			{
				panelMessage.Show(msgSuccess);
				Hide();
				panelMain.Show();
			}
		}
		
		// -------------------------------------------------------------------------------
		// CallbackConfirmAccountForgotPassword
		// -------------------------------------------------------------------------------	
		public void CallbackConfirmAccountForgotPassword(bool bSuccess = true)
		{
Debug.Log("CallbackConfirmAccountForgotPassword");
			if (bSuccess)
			{
				string[] fields = new string[] { inputAccountName.text };
   			 	TemporaryDisable(buttonForgot);
    			clientManager.ReqAccountForgotPassword(fields, CallbackAccountForgotPassword);
			}
		}
		
		// -------------------------------------------------------------------------------
		// ClickCancel
		// -------------------------------------------------------------------------------
		public void ClickCancel() {
			Hide();
			panelMain.Show();
		}

		// -------------------------------------------------------------------------------
	    
	}

}

// =======================================================================================