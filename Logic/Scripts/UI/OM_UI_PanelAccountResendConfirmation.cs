// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelAccountResendConfirmation
	// ===================================================================================
	public partial class OM_UI_PanelAccountResendConfirmation : OM_UI_Panel {
		
		[Header("---------- [Required] Feedback Messages ----------")]
		public string msgError 			= "Missing or incorrect data provided!";
		public string msgFail 			= "Failed!";
		public string msgSuccess		= "Success!";
		
		[Header("---------- [Required] UI Elements ----------")]
	    public InputField inputAccountName;
		public Button buttonResend;
		
		public OM_UI_PanelMain 			panelMain;
		public OM_UI_PanelMessage 		panelMessage;
		
		//--------------------------------------------------------------------------------
		// OnChildEnable
		//--------------------------------------------------------------------------------
		public override void OnChildEnable() {
			if (!panelMain) 			panelMain 				= FindObjectOfType<OM_UI_PanelMain>();
			if (!panelMessage) 			panelMessage 			= FindObjectOfType<OM_UI_PanelMessage>();
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
		// ClickResendConfirmation
		//--------------------------------------------------------------------------------		
		public void ClickResendConfirmation() {
		
			if (inputAccountName != null)
			{
				if (inputAccountName.text.validateName() )
					{
					
    				string[] fields = new string[] { inputAccountName.text };
   			 		TemporaryDisable(buttonResend);
    				clientManager.ReqAccountResendConfirmation(fields, CallbackResendConfirmation);
    				
    			} else {
    				panelMessage.Show(msgError);
    			}
    		
    		} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}		

		}
		
		//--------------------------------------------------------------------------------
		// CallbackResendConfirmation
		//--------------------------------------------------------------------------------		
		public void CallbackResendConfirmation(string[] result) {

			if (result[0] == Constants.INT_SUCCESS.ToString())
			{
				panelMessage.Show(msgSuccess);
			}
			else
			{
				panelMessage.Show(msgFail);
			}
		}
		
		//--------------------------------------------------------------------------------
		// ClickCancel
		//--------------------------------------------------------------------------------		
		public void ClickCancel() {
			Hide();
			FindObjectOfType<OM_UI_PanelMain>().Show();
		}

		//--------------------------------------------------------------------------------	
	    
	}

}

// =======================================================================================