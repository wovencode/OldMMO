// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_SlotAspect
	// ===================================================================================
	public partial class OM_UI_SlotAspect : OM_UISlot, IPointerEnterHandler, IPointerExitHandler {

	    [Header("---------- [Required] UI Elements ----------")]	    
	    public Button button;
		
		protected OM_UI_PanelActorCreate parentPanel;
		protected TemplateAspect tmpl;

		//--------------------------------------------------------------------------------
		// Init
		//--------------------------------------------------------------------------------
		public void Init(TemplateAspect _tmpl) {
			
			if (_tmpl == null) return;
			
			if (!parentPanel) 		parentPanel 			= FindObjectOfType<OM_UI_PanelActorCreate>();
			
			tmpl = _tmpl;
			
			button.GetComponent<Image>().sprite = tmpl.icon;
			
			button.onClick.AddListener(() => {
                ClickButton();
            });
            
            GenerateTooltip(tmpl.GenerateToolTip);
            
		}
		
		//--------------------------------------------------------------------------------
		// ClickButton
		//--------------------------------------------------------------------------------
		public void ClickButton()
		{
			parentPanel.SetAspect(tmpl);
		}
		
		
		
		//--------------------------------------------------------------------------------
	}

}

// =======================================================================================