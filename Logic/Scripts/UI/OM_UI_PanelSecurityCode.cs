// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelSecurityCode
	// ===================================================================================
	public partial class OM_UI_PanelSecurityCode : OM_UI_Panel {
		
		[Header("---------- [Required] Feedback Messages ----------")]
		public string msgConfirmError 		= "Missing or incorrect data provided!";
		public string msgConfirmFail 		= "Code confirm failed!";
		public string msgConfirmSuccess 	= "Code confirmed!";
		
		[Header("---------- [Required] UI Elements ----------")]
	    public InputField inputCode;
		public Button buttonCancel;
		public Button buttonConfirm;
		
		public OM_UI_PanelMessage panelMessage;
		
		protected int nAction;
		protected Action<bool> callbackFunction;
		
		//--------------------------------------------------------------------------------
		// OnChildEnable
		//--------------------------------------------------------------------------------
		public override void OnChildEnable() {
			if (!panelMessage) 			panelMessage 			= FindObjectOfType<OM_UI_PanelMessage>();
			
			inputCode.text = "";
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
		// Init
		//--------------------------------------------------------------------------------
		public void Init(Constants.AccountActionType accountActionType, Action<bool> _callbackFunction) {
			nAction = (int)accountActionType;
			callbackFunction = _callbackFunction;
			Show();
		}
		
		//--------------------------------------------------------------------------------
		// ClickConfirmCode
		//--------------------------------------------------------------------------------		
		public void ClickConfirmCode() {
		
			if (inputCode != null) {
			
				if (inputCode.text.validateName()
					) {
					
					string[] fields = new string[] { clientManager.clientAccount.sName, inputCode.text, nAction.ToString() };
    	
   			 		TemporaryDisable(buttonConfirm);
    		
    				clientManager.ReqCodeConfirm(fields, CallbackConfirmCode);
    			
    			} else {
    				panelMessage.Show(msgConfirmError);
    			}
    		
    		} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}		
		
		}
		
		//--------------------------------------------------------------------------------
		// CallbackConfirmCode
		//--------------------------------------------------------------------------------		
		public void CallbackConfirmCode(string[] result) {
			if (result[0] == Constants.INT_SUCCESS.ToString()) {
				Hide();
				panelMessage.Show(msgConfirmSuccess);
				callbackFunction(true);
			} else {
				panelMessage.Show(msgConfirmFail);
				callbackFunction(false);
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