// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UISlot
	// ===================================================================================
	public partial class OM_UISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

		protected string _tooltip;
	   	protected OM_UI_PanelTooltip panelTooltip;
	   	
		// -------------------------------------------------------------------------------
		// Start
		// -------------------------------------------------------------------------------
		public void Start()
		{
			if (!panelTooltip) 		panelTooltip 	= FindObjectOfType<OM_UI_PanelTooltip>();
		}
		
		// -------------------------------------------------------------------------------
		// GenerateTooltip
		// -------------------------------------------------------------------------------
		public void GenerateTooltip(string text)
		{
			_tooltip = text;
		}
		
		//--------------------------------------------------------------------------------
		// 
		//--------------------------------------------------------------------------------
		public void OnPointerEnter(PointerEventData eventData)
		{
			panelTooltip.Show(_tooltip);
		}
		
		//--------------------------------------------------------------------------------
		// 
		//--------------------------------------------------------------------------------
		public void OnPointerExit(PointerEventData eventData)
		{
			panelTooltip.Hide();
		}
		
		//--------------------------------------------------------------------------------
	}

}

// =======================================================================================