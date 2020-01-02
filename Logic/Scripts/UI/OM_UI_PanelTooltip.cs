// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelTooltip
	// ===================================================================================
	public partial class OM_UI_PanelTooltip : MonoBehaviour {
		
		[Header("---------- Required UI Elements ----------")]
	    public GameObject panelRoot;
	    public Text text;
	    
		//--------------------------------------------------------------------------------
		// Show
		//--------------------------------------------------------------------------------
	    public void Show(string tooltip="") {
	    	if (tooltip != "" &&
	    		text != null &&
	    		panelRoot != null
	    	) {
	    		CancelInvoke("DelayedHide");
	    		text.text = tooltip;
				transform.SetAsLastSibling();
	        	panelRoot.SetActive(true);
	        }
	        else
	        {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}
	    }
	    
	    //--------------------------------------------------------------------------------
		// Hide
		//--------------------------------------------------------------------------------
	    public void Hide() {
	    	if (text != null &&
	    		panelRoot != null)
	    	{
	    		Invoke("DelayedHide", 0.1f);
	    	}
	    	else
	    	{
	    		Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}
	    }
	    
	    //--------------------------------------------------------------------------------
		// DelayedHide
		//--------------------------------------------------------------------------------
	    protected void DelayedHide()
	    {
	    	panelRoot.SetActive(false);
	    	text.text = "";
	    }

	 	//--------------------------------------------------------------------------------
		// OnChildDisable
		//--------------------------------------------------------------------------------
		public void OnChildDisable() {
			Hide();
		}

		//--------------------------------------------------------------------------------
	}

}

// =======================================================================================