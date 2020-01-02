// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelAccountDeleteAccount
	// ===================================================================================
	public partial class OM_UI_PanelAccountDeleteAccount : OM_UI_Panel {
		
		[Header("---------- [Required] Feedback Messages ----------")]
		public string msgError 			= "Missing or incorrect data provided!";
		public string msgFail 			= "Failed!";
		public string msgSuccess		= "Success!";
		
		[Header("---------- [Required] UI Elements ----------")]
	    public InputField inputAccountName;
		public InputField inputPassword;
		public Button buttonDelete;
		
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
			
			inputAccountName.text 		= "";
	    	inputPassword.text 			= "";
	    	
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
		// ClickDeleteAccount
		// -------------------------------------------------------------------------------	
		public void ClickDeleteAccount() {
		
			if (inputAccountName != null &&
				inputPassword != null) {
			
				if (inputAccountName.text.validateName() &&
					inputPassword.text.validatePassword()
					) {
					
					CallbackConfirmAccountDelete();
    			
    			} else {
    				panelMessage.Show(msgError);
    			}
    		
    		} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}		
		
		}
		
		// -------------------------------------------------------------------------------
		// CallbackAccountDelete
		// -------------------------------------------------------------------------------
		private void CallbackAccountDelete(string[] result) {
			if (result[0] == Constants.INT_FAILURE.ToString())
			{
				panelMessage.Show(msgFail);
			}
			else if (result[0] == Constants.INT_CONFIRM.ToString())
			{
				panelSecurityCode.Init(Constants.AccountActionType.DeleteAccount, CallbackConfirmAccountDelete);
			}
			else if (result[0] == Constants.INT_SUCCESS.ToString())
			{
				inputAccountName.text 		= "";
	    		inputPassword.text 			= "";
				panelMessage.Show(msgSuccess);
				Hide();
			}
		}
		
		// -------------------------------------------------------------------------------
		// CallbackConfirmAccountDelete
		// -------------------------------------------------------------------------------	
		private void CallbackConfirmAccountDelete(bool bSuccess = true)
		{
			if (bSuccess)
			{
				string[] fields = new string[] { inputAccountName.text, inputPassword.text };
   			 	TemporaryDisable(buttonDelete);
    			clientManager.ReqAccountDelete(fields, CallbackAccountDelete);
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