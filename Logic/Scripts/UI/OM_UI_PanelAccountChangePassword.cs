// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelAccountChangePassword
	// ===================================================================================
	public partial class OM_UI_PanelAccountChangePassword : OM_UI_Panel {
		
		[Header("---------- [Required] Feedback Messages ----------")]
		public string msgError 			= "Missing or incorrect data provided!";
		public string msgFail 			= "Failed!";
		public string msgSuccess		= "Success!";
		
		[Header("---------- [Required] UI Elements ----------")]
		public InputField inputPasswordOld;
	    public InputField inputPassword;
	    public InputField inputPasswordRepeat;
		public Button buttonChange;
		
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
		
			inputPasswordOld.text 		= "";
	    	inputPassword.text 			= "";
	    	inputPasswordRepeat.text 	= "";
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
		// ClickChangePassword
		// -------------------------------------------------------------------------------
		public void ClickChangePassword() {
			
			if (inputPasswordOld != null &&
				inputPassword != null &&
				inputPasswordRepeat != null) {
			
				if (inputPasswordOld.text.validatePassword() &&
					inputPassword.text.validatePassword() &&
					inputPasswordRepeat.text.validatePassword() &&
					inputPasswordOld.text != inputPassword.text
					) {
					
					CallbackConfirmAccountChangePassword();
    			
    			} else {
    				panelMessage.Show(msgError);
    			}
    		
    		} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}
    		
		}
		
		// -------------------------------------------------------------------------------
		// CallbackAccountChangePassword
		// -------------------------------------------------------------------------------
		private void CallbackAccountChangePassword(string[] result) {
			if (result[0] == Constants.INT_FAILURE.ToString())
			{
				panelMessage.Show(msgFail);
			}
			else if (result[0] == Constants.INT_CONFIRM.ToString())
			{
				panelSecurityCode.Init(Constants.AccountActionType.ChangePassword, CallbackConfirmAccountChangePassword);
			}
			else if (result[0] == Constants.INT_SUCCESS.ToString())
			{
				inputPasswordOld.text 		= "";
	    		inputPassword.text 			= "";
	    		inputPasswordRepeat.text 	= "";
				panelMessage.Show(msgSuccess);
				Hide();
			}
		}
		
		// -------------------------------------------------------------------------------
		// CallbackConfirmAccountChangePassword
		// -------------------------------------------------------------------------------	
		private void CallbackConfirmAccountChangePassword(bool bSuccess = true)
		{
			if (bSuccess)
			{
				string[] fields = new string[] { inputPasswordOld.text, inputPassword.text };
   			 	TemporaryDisable(buttonChange);
    			clientManager.ReqAccountChangePassword(fields, CallbackAccountChangePassword);
			}
		}
		
		// -------------------------------------------------------------------------------
		// ClickCancel
		// -------------------------------------------------------------------------------	
		public void ClickCancel() {
			Hide();
		}

		//--------------------------------------------------------------------------------	
	    
	}

}

// =======================================================================================