// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelPrompt
	// ===================================================================================
	public partial class OM_UI_PanelPrompt : OM_UI_Panel {
		
		[Header("---------- [Required] UI Elements ----------")]
	    public Text headerText;
		public Button buttonYes;
		public Button buttonNo;
				
		//--------------------------------------------------------------------------------
		// OnChildEnable
		//--------------------------------------------------------------------------------
		public override void OnChildEnable() {
			
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
		public void Init() {
			
		}
		
		//--------------------------------------------------------------------------------
		// ClickYes
		//--------------------------------------------------------------------------------		
		public void ClickConfirmCode() {
		
			
		
		}
		
		//--------------------------------------------------------------------------------
		// ClickNo
		//--------------------------------------------------------------------------------		
		public void CallbackConfirmCode(string[] result) {
			
		}
		
		//--------------------------------------------------------------------------------	
	    
	}

}

// =======================================================================================