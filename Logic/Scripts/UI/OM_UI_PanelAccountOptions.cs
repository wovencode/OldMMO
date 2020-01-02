// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelAccountOptions
	// ===================================================================================
	public partial class OM_UI_PanelAccountOptions : OM_UI_Panel {
		
		[Header("---------- Required UI Elements ----------")]
		public Button buttonQuit;
		public Button buttonAccountChangeName;
		public Button buttonAccountChangePassword;
		public Button buttonAccountDelete;
		
		//--------------------------------------------------------------------------------
		// OnChildEnable
		//--------------------------------------------------------------------------------
		public override void OnChildEnable() {
		
			if (clientManager.bConfirmAccountChangeName && String.IsNullOrWhiteSpace(clientManager.clientAccount.sMail))
				buttonAccountChangeName.interactable = false;
			else
				buttonAccountChangeName.interactable = true;
			
			if (clientManager.bConfirmAccountChangePassword && String.IsNullOrWhiteSpace(clientManager.clientAccount.sMail))
				buttonAccountChangePassword.interactable = false;
			else
				buttonAccountChangePassword.interactable = true;
		
			if (clientManager.bConfirmAccountDelete && String.IsNullOrWhiteSpace(clientManager.clientAccount.sMail))
				buttonAccountDelete.interactable = false;
			else
				buttonAccountDelete.interactable = true;
		
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
			Hide();
			FindObjectOfType<OM_UI_PanelAccountChangeName>().Show();
		}
		
	   	//--------------------------------------------------------------------------------
		// ClickChangeMail
		//--------------------------------------------------------------------------------		
		public void ClickChangeMail() {
			Hide();
			FindObjectOfType<OM_UI_PanelAccountChangeMail>().Show();
		}
		
		//--------------------------------------------------------------------------------
		// ClickChangePassword
		//--------------------------------------------------------------------------------		
		public void ClickChangePassword() {
			Hide();
			FindObjectOfType<OM_UI_PanelAccountChangePassword>().Show();
		}
		
	   	//--------------------------------------------------------------------------------
		// ClickDeleteAccount
		//--------------------------------------------------------------------------------		
		public void ClickDeleteAccount() {
			Hide();
			FindObjectOfType<OM_UI_PanelAccountDeleteAccount>().Show();
		}
				
		//--------------------------------------------------------------------------------
		// ClickQuit
		//--------------------------------------------------------------------------------		
		public void ClickQuit() {
			TemporaryDisable(buttonQuit);
			Tools.Quit();
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