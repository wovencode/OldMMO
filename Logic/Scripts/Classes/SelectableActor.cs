// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// SelectableActor
	// ===================================================================================
	public class SelectableActor : MonoBehaviour {
		
		protected string sName;
		protected OM_UI_PanelActorSelect parentPanel;

		//--------------------------------------------------------------------------------
		// Init
		//--------------------------------------------------------------------------------
		public void Init(OM_UI_PanelActorSelect _parentPanel, string _sName)
		{
			parentPanel = _parentPanel;
			sName = _sName;
		}
		
		//--------------------------------------------------------------------------------
		// OnMouseDown
		//--------------------------------------------------------------------------------
		public void OnMouseDown() {
			parentPanel.ClickSelectActor(sName);
		}
		
	 	//--------------------------------------------------------------------------------
		// Update
		//--------------------------------------------------------------------------------
		public void Update() {
			
		}

		//--------------------------------------------------------------------------------
	}

}

// =======================================================================================