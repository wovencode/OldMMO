// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelMain
	// ===================================================================================
	public partial class OM_UI_PanelMain : OM_UI_Panel {
		
		[Header("---------- [Required] Camera & Positions ----------")]
		public string cameraPositionObject;
		public string cameraTargetObject;
		
		[Header("---------- [Required] Labels & Messages ----------")]
		public string labelButtonRegister 				= "Register ({0} left)";
		
		[Header("---------- [Required] UI Elements ----------")]
		public Button 							buttonRegister;
		
		public OM_UI_PanelAccountLogin					panelLogin;
		public OM_UI_PanelAccountRegister				panelRegister;
		public OM_UI_PanelAccountResendConfirmation		panelResendConfirmation;
		public OM_UI_PanelAccountForgotPassword			panelForgotPassword;
						
		//--------------------------------------------------------------------------------
		// OnChildEnable
		//--------------------------------------------------------------------------------
		public override void OnChildEnable() {
		
			if (!panelLogin) 				panelLogin 				= FindObjectOfType<OM_UI_PanelAccountLogin>();
			if (!panelRegister)				panelRegister			= FindObjectOfType<OM_UI_PanelAccountRegister>();
			if (!panelResendConfirmation)	panelResendConfirmation = FindObjectOfType<OM_UI_PanelAccountResendConfirmation>();
			if (!panelForgotPassword)		panelForgotPassword 	= FindObjectOfType<OM_UI_PanelAccountForgotPassword>();
			
			if (buttonRegister != null) {
				buttonRegister.GetComponentInChildren<Text>().text = String.Format(labelButtonRegister, clientManager.accountsRemaining);
				if (clientManager.accountsRemaining < 1)
					buttonRegister.interactable = false;
			} else {
				Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
			}
			
			if (!_cameraPositionObject)		_cameraPositionObject	= GameObject.Find(cameraPositionObject);
			if (!_cameraTargetObject)		_cameraTargetObject		= GameObject.Find(cameraTargetObject);
			
        	Camera.main.transform.position = _cameraPositionObject.transform.position;
        	
        	iTween.LookTo(Camera.main.gameObject, _cameraTargetObject.transform.position, 0f);
        	
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
			panelLogin.Show();
			Hide();
		}

	   	//--------------------------------------------------------------------------------
		// ClickRegister
		//--------------------------------------------------------------------------------		
		public void ClickRegister() {
			panelRegister.Show();
			Hide();
		}
		
	   	//--------------------------------------------------------------------------------
		// ClickResendConfirmation
		//--------------------------------------------------------------------------------		
		public void ClickResendConfirmation() {
			panelResendConfirmation.Show();
			Hide();
		}
		
	   	//--------------------------------------------------------------------------------
		// ClickForgotPassword
		//--------------------------------------------------------------------------------		
		public void ClickForgotPassword() {
			panelForgotPassword.Show();
			Hide();
		}
		
		//--------------------------------------------------------------------------------
		// ClickQuit
		//--------------------------------------------------------------------------------		
		public void ClickQuit() {
			Hide();
			Tools.Quit();
		}

	   	//--------------------------------------------------------------------------------
	    
	}

}

// =======================================================================================