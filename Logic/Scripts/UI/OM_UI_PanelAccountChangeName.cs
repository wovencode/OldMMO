// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelAccountChangeName
	// ===================================================================================
	public partial class OM_UI_PanelAccountChangeName : OM_UI_Panel {
		
		[Header("---------- [Required] Feedback Messages ----------")]
		public string msgError 			= "Missing or incorrect data provided!";
		public string msgFail 			= "Failed!";
		public string msgSuccess		= "Success!";
		
		[Header("---------- [Required] UI Elements ----------")]
		public InputField inputAccountNameOld;
	    public InputField inputAccountName;
		public InputField inputPassword;
		public Button buttonChange;
		
		public OM_UI_PanelMain 			panelMain;
		public OM_UI_PanelMessage 		panelMessage;
		public OM_UI_PanelSecurityCode 	panelSecurityCode;
		
		protected string sCurrentAccountName;
		protected string sNewAccountName;
		
		//--------------------------------------------------------------------------------
		// OnChildEnable
		//--------------------------------------------------------------------------------
		public override void OnChildEnable() {
		
			if (!panelMain) 			panelMain 				= FindObjectOfType<OM_UI_PanelMain>();
			if (!panelMessage) 			panelMessage 			= FindObjectOfType<OM_UI_PanelMessage>();
			if (!panelSecurityCode) 	panelSecurityCode 		= FindObjectOfType<OM_UI_PanelSecurityCode>();
		
			if (inputAccountNameOld != null) {
				inputAccountNameOld.text = sCurrentAccountName;
    		} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}
    		
    		sCurrentAccountName			= clientManager.clientAccount.sName;
    		inputAccountNameOld.text 	= sCurrentAccountName;
    		inputAccountName.text 		= "";
    		inputPassword.text 			= "";
		}
		
		//--------------------------------------------------------------------------------
		// OnInvokeRepeating
		//--------------------------------------------------------------------------------
		public override void OnInvokeRepeating() {}
		
		//--------------------------------------------------------------------------------
		// SlowUpdate
		//--------------------------------------------------------------------------------
		public override void SlowUpdate() {}
		
		//--------------------------------------------------------------------------------
		// ClickChangeName
		//--------------------------------------------------------------------------------		
		public void ClickChangeName() {
		
			if (inputAccountName != null &&
				inputPassword != null) {
			
				if (inputAccountName.text.validateName() &&
					inputPassword.text.validatePassword() &&
					inputAccountName.text != sCurrentAccountName
					) {
					
					CallbackConfirmAccountChangeName();
    			
    			} else {
    				panelMessage.Show(msgError);
    			}
    		
    		} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}		
		
		}
		
		//--------------------------------------------------------------------------------
		// CallbackAccountChangeName
		//--------------------------------------------------------------------------------		
		private void CallbackAccountChangeName(string[] result) {
			if (result[0] == Constants.INT_FAILURE.ToString())
			{
				panelMessage.Show(msgFail);
			}
			else if (result[0] == Constants.INT_CONFIRM.ToString())
			{
				panelSecurityCode.Init(Constants.AccountActionType.ChangeName, CallbackConfirmAccountChangeName);
			}
			else if (result[0] == Constants.INT_SUCCESS.ToString())
			{
				clientManager.clientAccount.sName = sNewAccountName;
				inputAccountNameOld.text 	= sNewAccountName;
    			inputAccountName.text 		= "";
    			inputPassword.text 			= "";
				panelMessage.Show(msgSuccess);
				Hide();
			}
		}
		
		//--------------------------------------------------------------------------------
		// CallbackConfirmAccountChangeName
		//--------------------------------------------------------------------------------		
		private void CallbackConfirmAccountChangeName(bool bSuccess = true)
		{
			if (bSuccess)
			{
				sNewAccountName = inputAccountName.text;
				string[] fields = new string[] { inputAccountName.text, inputPassword.text };
   			 	TemporaryDisable(buttonChange);
    			clientManager.ReqAccountChangeName(fields, CallbackAccountChangeName);
			}
		}
		
		//--------------------------------------------------------------------------------
		// ClickCancel
		//--------------------------------------------------------------------------------
		public void ClickCancel() {
			Hide();
		}

		//--------------------------------------------------------------------------------	
	    
	}

}

// =======================================================================================