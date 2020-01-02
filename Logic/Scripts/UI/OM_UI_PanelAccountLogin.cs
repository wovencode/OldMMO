// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelAccountLogin
	// ===================================================================================
	public partial class OM_UI_PanelAccountLogin : OM_UI_Panel {
		
		[Header("---------- [Required] Feedback Messages ----------")]
		public string msgError 		= "Missing or incorrect data provided!";
		public string msgFail 			= "Login failed!";
		public string msgFailConfirm 	= "Failed - confirm your account first!";
		
		[Header("---------- [Required] UI Elements ----------")]
	    public InputField inputUsername;
		public InputField inputPassword;
		public Button buttonLogin;
		
		public OM_UI_PanelActorSelect 					panelActorList;
		public OM_UI_PanelMain 							panelMain;
		public OM_UI_PanelMessage 						panelMessage;
		public OM_UI_PanelSecurityCode 					panelSecurityCode;
		public OM_UI_PanelOverlay						panelOverlay;
		
		//--------------------------------------------------------------------------------
		// OnChildEnable
		//--------------------------------------------------------------------------------
		public override void OnChildEnable() {
		
			if (!panelActorList) 		panelActorList 			= FindObjectOfType<OM_UI_PanelActorSelect>();
			if (!panelMain) 			panelMain 				= FindObjectOfType<OM_UI_PanelMain>();
			if (!panelMessage) 			panelMessage 			= FindObjectOfType<OM_UI_PanelMessage>();
			if (!panelSecurityCode) 	panelSecurityCode 		= FindObjectOfType<OM_UI_PanelSecurityCode>();
			if (!panelOverlay)			panelOverlay 			= FindObjectOfType<OM_UI_PanelOverlay>();
			
			inputUsername.text = "";
			inputPassword.text = "";
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
		// ClickLogin
		//--------------------------------------------------------------------------------		
		public void ClickLogin() {
		
			if (inputUsername != null &&
				inputPassword != null
				) {
			
				if (inputUsername.text.validateName() &&
					inputPassword.text.validatePassword() ) {

					CallbackConfirmLogin();
    			
    			} else {
    				panelMessage.Show(msgError);
    			}
    		
    		} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}		

		}
		
		//--------------------------------------------------------------------------------
		// CallbackLogin
		//--------------------------------------------------------------------------------		
		private void CallbackLogin(string[] result) {
		
			if (result[0] == Constants.INT_FAILURE.ToString())
			{
				panelMessage.Show(msgFail);
			}
			else if (result[0] == Constants.INT_CONFIRM.ToString())
			{
				panelMessage.Show(msgFailConfirm);
				panelSecurityCode.Init(Constants.AccountActionType.ConfirmAccount, CallbackConfirmLogin);
			}
			else if (result[0] == Constants.INT_RECONFIRM.ToString())
			{
				panelMessage.Show(msgFailConfirm);
				panelSecurityCode.Init(Constants.AccountActionType.LoginAccount, CallbackConfirmLogin);
			}
			else if (result[0] == Constants.INT_SUCCESS.ToString())
			{
				panelOverlay.Show();
				panelActorList.Show();
				Hide();
			}
			
		}
		
		//--------------------------------------------------------------------------------
		// CallbackConfirm
		//--------------------------------------------------------------------------------		
		private void CallbackConfirmLogin(bool bSuccess = true)
		{
			if (bSuccess)
			{
				string[] fields = new string[] { inputUsername.text, inputPassword.text, Tools.GetDeviceId };
   			 	TemporaryDisable(buttonLogin);
    			clientManager.ReqAccountLogin(fields, CallbackLogin);
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