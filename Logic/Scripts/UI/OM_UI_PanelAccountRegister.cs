// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelAccountRegister
	// ===================================================================================
	public partial class OM_UI_PanelAccountRegister : OM_UI_Panel {
		
		[Header("---------- [Required] Feedback Messages ----------")]
		public string msgRegisterSuccess = "Account registered!";
		public string msgRegisterConfirm = "Account registered, requires confirmation!";
		public string msgRegisterFail = "Failed to register account!";
		
	    [Header("---------- [Required] UI Elements ----------")]
	    public InputField inputUsername;
		public InputField inputEmail;
		public InputField inputPassword;
		public Button buttonRegister;
		
		public OM_UI_PanelMain panelMain;
		public OM_UI_PanelMessage panelMessage;
		
		//--------------------------------------------------------------------------------
		// OnChildEnable
		//--------------------------------------------------------------------------------
		public override void OnChildEnable() {
			if (!panelMain) 	panelMain 		= FindObjectOfType<OM_UI_PanelMain>();
			if (!panelMessage) 	panelMessage 	= FindObjectOfType<OM_UI_PanelMessage>();
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
		// ClickRegister
		//--------------------------------------------------------------------------------		
		public void ClickRegister() {
			
			if (inputUsername != null &&
				inputEmail != null &&
				inputPassword != null) {
				
				// We validate the input now:
				// - Name
				// - Password
				// - eMail (only if account confirm is required, otherwise its optional)
				if (inputUsername.text.validateName() &&
					(inputEmail.text.validateEmail() || !clientManager.bConfirmAccountCreate) &&
					inputPassword.text.validatePassword()
					) {
					
					string[] fields = new string[] { inputUsername.text, inputPassword.text, inputEmail.text, Tools.GetDeviceId };
    	
    				TemporaryDisable(buttonRegister);
    		
    				clientManager.ReqRegisterAccount(fields, CallbackRegisterAccount);
    			
    			} else {
    				panelMessage.Show(Constants.STR_ERROR);
    			}
    		
    		} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}
    		
		}
		
		//--------------------------------------------------------------------------------
		// CallbackRegisterAccount
		//--------------------------------------------------------------------------------		
		private void CallbackRegisterAccount(string[] result) {
			
			if (result[0] == Constants.INT_FAILURE.ToString()) {
				panelMessage.Show(msgRegisterFail);
			} else if (result[0] == Constants.INT_CONFIRM.ToString()) {
				Hide();
				panelMessage.Show(msgRegisterConfirm);
				panelMain.Show();
			} else if (result[0] == Constants.INT_SUCCESS.ToString()) {
				Hide();
				panelMessage.Show(msgRegisterSuccess);
				panelMain.Show();
			}
		
		}
		
		//--------------------------------------------------------------------------------
		// ClickCancel
		//--------------------------------------------------------------------------------
		public void ClickCancel() {
			Hide();
			panelMain.Show();
		}

		//--------------------------------------------------------------------------------

	}

}

// =======================================================================================