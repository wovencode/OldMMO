// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UISlotGrid
	// ===================================================================================
	public partial class OM_UISlotGrid : OM_UISlot {

	    [Header("---------- Required UI Elements ----------")]	    
	    public Image image;
	    public Text textValue;
		
		//[HideInInspector] public LoomClient.LOOM_ENUM_VIRTUAL_OBJ_TYPE objectType;
		[HideInInspector] public int objectId;
		
		protected GameObject panel;
				
		//--------------------------------------------------------------------------------
		// ClickShowObjectInfo
		//--------------------------------------------------------------------------------		
		public void ClickShowObjectInfo() {
			//FindObjectOfType<OM_UI_PanelVirtualPossessionInfo>().Show(objectType, objectId);
		}

		//--------------------------------------------------------------------------------
	}

}

// =======================================================================================