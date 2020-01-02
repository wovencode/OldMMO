// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelActorAspect
	// ===================================================================================
	public partial class OM_UI_PanelActorAspect : OM_UI_Panel {
		
		[Header("---------- [Required] UI Elements ----------")]
		public Text headerText;
	    public GameObject slotPrefab;
		public Transform content;
		
		protected OM_UI_PanelActorCreate parentPanel;
		protected int nSelected;
		protected string sFamily;
		protected List<TemplateAspect> tmpls = new List<TemplateAspect>();
		
		// -------------------------------------------------------------------------------
		// OnChildEnable
		// -------------------------------------------------------------------------------
		public override void OnChildEnable() {
			if (!parentPanel) 		parentPanel 			= FindObjectOfType<OM_UI_PanelActorCreate>();
		}
		
		// -------------------------------------------------------------------------------
		// OnInvokeRepeating
		// -------------------------------------------------------------------------------
		public override void OnInvokeRepeating() {}
		
		// -------------------------------------------------------------------------------
		// SlowUpdate
		// -------------------------------------------------------------------------------
		public override void SlowUpdate() {}
		
		//--------------------------------------------------------------------------------
		// Init
		//--------------------------------------------------------------------------------
		public void Init(string _sFamily) {
			
			if (String.IsNullOrWhiteSpace(_sFamily)) return;
			
			for (int i = 0; i < content.childCount; ++i)
				Destroy(content.GetChild(i).gameObject);
		
			tmpls.Clear();
			sFamily 		= _sFamily;
			headerText.text = sFamily;
			
			foreach (TemplateAspect tmpl in DataManager.dictAspect.Values)
			{
				if (tmpl.family == sFamily)
					tmpls.Add(tmpl);
			}
			
			if (tmpls.Count > 0)
			{
			
				TemplateAspect defaultTemplate = null;
				
				foreach (TemplateAspect tmpl in tmpls)
				{
					
					if (!defaultTemplate)
						defaultTemplate = tmpl;
				
					GameObject uiObject = Instantiate(slotPrefab);
					uiObject.GetComponent<OM_UI_SlotAspect>().Init(tmpl);
					uiObject.SetActive(true);
					uiObject.transform.SetParent(content.transform, false);
					
				}
				
				if (!parentPanel) parentPanel = FindObjectOfType<OM_UI_PanelActorCreate>();	
				parentPanel.SetAspect(defaultTemplate);
			}
			
			Show();
		}
		
		// -------------------------------------------------------------------------------
	    
	}

}

// =======================================================================================