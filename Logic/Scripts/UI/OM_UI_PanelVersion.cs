// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelVersion
	// ===================================================================================
	public partial class OM_UI_PanelVersion : OM_UI_Panel {
		
		[Header("---------- Feedback Messages ----------")]
		public string msgVersionCheck 	= "Checking version...";
		public string msgVersionFail 	= "Your client version is out of date, please update your client.";
		
		[Header("---------- Required UI Elements ----------")]
	    public Text text;
	    public Button buttonQuit;
	    	   	
	   	public OM_UI_PanelMain panelMain;
		public OM_UI_PanelMessage panelMessage;
	   	
	    //--------------------------------------------------------------------------------
		// Show
		//--------------------------------------------------------------------------------    
	    public override void OnChildEnable() {
	    	
	    	if (!panelMain) 	panelMain 		= FindObjectOfType<OM_UI_PanelMain>();
			if (!panelMessage) 	panelMessage 	= FindObjectOfType<OM_UI_PanelMessage>();
			
	    	text.text = msgVersionCheck;
	    	
	    	Invoke("CheckVersion", 2f);
	    	
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
		// CheckVersion
		//--------------------------------------------------------------------------------    
	    private void CheckVersion() {
	    	if (text != null) {		
    			clientManager.ReqCheckVersion(new string[] { Tools.GetDeviceId, Tools.GetVersion }, CallbackCheckVersion);
    		} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}
	    }
	    
	    //--------------------------------------------------------------------------------
		// CallbackCheckVersion
		//--------------------------------------------------------------------------------		
		private void CallbackCheckVersion(string[] result) {
			
			if (result[0] != Constants.INT_FAILURE.ToString()) {
				Hide();
				panelMain.Show();
			} else {
				panelMessage.Show(msgVersionFail);
			}
			
		}
	    
	    //--------------------------------------------------------------------------------
		// ClickQuit
		//--------------------------------------------------------------------------------		
		public void ClickQuit() {
			TemporaryDisable(buttonQuit);
			Tools.Quit();
		}
		
	    //--------------------------------------------------------------------------------
	    
	}

}

// =======================================================================================