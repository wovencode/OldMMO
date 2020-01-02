// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Linq;
using System.Collections.Generic;
using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelActorCreate
	// ===================================================================================
	public partial class OM_UI_PanelActorCreate : OM_UI_Panel {
		
		[Header("---------- [Required] Camera & Positions ----------")]
		public string cameraPositionObject;
		public string cameraTargetObject;
		public string actorPositionObject;
		
		[Header("---------- [Required] Feedback Messages ----------")]
		public string msgCreateSuccess = "Character created!";
		public string msgCreateFail = "Failed to create character!";
		
		[Header("---------- [Required] UI Elements ----------")]
	    public InputField inputActorname;
		public Button buttonCreate;
		public Button buttonCancel;
		
		public OM_UI_PanelActorSelect 	panelActorList;
		public OM_UI_PanelMessage 		panelMessage;
		
		[Header("---------- [Required] Actor Aspects ----------")]
		public ActorAspects[] actorAspects;
		
		protected Dictionary<string, TemplateAspect> dictAspects = new Dictionary<string, TemplateAspect>();
		protected GameObject 	_actorPositionObject;
		protected string 		actorName;
		
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		// ActorAspects
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		[System.Serializable]
		public class ActorAspects
		{
			public string aspectFamily;
			public OM_UI_PanelActorAspect aspectPanel;
			public OM_UI_SlotAspect aspectSlot;
		}
		
		// -------------------------------------------------------------------------------
		// OnChildEnable
		// -------------------------------------------------------------------------------
		public override void OnChildEnable() {
			
			if (!panelActorList) 		panelActorList 			= FindObjectOfType<OM_UI_PanelActorSelect>();
			if (!panelMessage) 			panelMessage 			= FindObjectOfType<OM_UI_PanelMessage>();
			
			buttonCreate.interactable = false;
			inputActorname.text = "";
			
			dictAspects.Clear();
			
			foreach (ActorAspects actorAspect in actorAspects)
			{
				if (!String.IsNullOrWhiteSpace(actorAspect.aspectFamily) &&
					actorAspect.aspectPanel != null)
				{
					actorAspect.aspectPanel.Init(actorAspect.aspectFamily);
				}
				else
				{
					actorAspect.aspectPanel.Hide();
				}
			}
			
			if (!_cameraPositionObject)		_cameraPositionObject		= GameObject.Find(cameraPositionObject);
			if (!_cameraTargetObject)		_cameraTargetObject			= GameObject.Find(cameraTargetObject);
			if (!_actorPositionObject)		_actorPositionObject		= GameObject.Find(actorPositionObject);
			
        	Camera.main.transform.position = _cameraPositionObject.transform.position;
        	
        	iTween.LookTo(Camera.main.gameObject, _cameraTargetObject.transform.position, 0f);
			
		}
		
		// -------------------------------------------------------------------------------
		// OnInvokeRepeating
		// -------------------------------------------------------------------------------
		public override void OnInvokeRepeating() {}
		
		// -------------------------------------------------------------------------------
		// SlowUpdate
		// -------------------------------------------------------------------------------
		public override void SlowUpdate() {}
		
		// -------------------------------------------------------------------------------
		// SetAspect
		// -------------------------------------------------------------------------------
		public void SetAspect(TemplateAspect tmpl)
		{
			
			if (dictAspects.ContainsKey(tmpl.family))
				dictAspects.Remove(tmpl.family);
			
			dictAspects.Add(tmpl.family, tmpl);
			
			foreach (ActorAspects actorAspect in actorAspects)
			{
				if (actorAspect.aspectFamily == tmpl.family)
				{
					actorAspect.aspectSlot.Init(tmpl);
				}
			}
			
			UpdateActorObject();
			
		}
		
		// -------------------------------------------------------------------------------
		// UpdateActorObject
		// -------------------------------------------------------------------------------
		protected void UpdateActorObject()
		{
			
			if (!_actorPositionObject)		_actorPositionObject		= GameObject.Find(actorPositionObject);
			
			foreach (TemplateAspect aspect in dictAspects.Values)
			{
				if (aspect.actorPrefab)
				{
				
					for (int i = 0; i < _actorPositionObject.transform.childCount; i++)
						Destroy(_actorPositionObject.transform.GetChild(i).gameObject);
						
					GameObject actorObject = Instantiate(aspect.actorPrefab);
					actorObject.GetComponent<AspectSubsystem>().Init(actorObject, dictAspects.Values.ToList());
					actorObject.transform.position 	= _actorPositionObject.transform.position;
					actorObject.transform.parent 	= _actorPositionObject.transform;
					
					if (String.IsNullOrWhiteSpace(actorName))
					{
						actorObject.name = aspect.actorPrefab.name;
					}
					else
					{
						actorObject.name = actorName;
					}
				
					break;
				}
			}
		
		}
		
		// -------------------------------------------------------------------------------
		// InputActorNameChanged
		// -------------------------------------------------------------------------------
		public void InputActorNameChanged()
		{
			if (!String.IsNullOrWhiteSpace(inputActorname.text))
			{
				actorName = inputActorname.text;
				buttonCreate.interactable = true;
			}
			else
			{
				buttonCreate.interactable = false;
			}
		}
		
		// -------------------------------------------------------------------------------
		// ClickActorCreate
		// -------------------------------------------------------------------------------
		public void ClickActorCreate() {
		
			if (actorName.validateName() &&
				dictAspects.Count > 0)
			{
				string[] fields = dictAspects.Select(x => x.Value.GetId.ToString()).ToArray();
				clientManager.ReqActorPlayerCreate(actorName, fields, CallbackActorCreate);
			}
			else
			{
				panelMessage.Show(Constants.STR_ERROR);
			}

		}
		
		// -------------------------------------------------------------------------------
		// CallbackActorCreate
		// -------------------------------------------------------------------------------
		public void CallbackActorCreate(string[] result) {
			if (result[0] == Constants.INT_FAILURE.ToString()) {
				panelMessage.Show(msgCreateFail);
			} else if (result[0] == Constants.INT_SUCCESS.ToString()) {
				panelMessage.Show(msgCreateSuccess);
				ClickCancel();
			}
		}
		
		// -------------------------------------------------------------------------------
		// ClickCancel
		// -------------------------------------------------------------------------------	
		public void ClickCancel() {
			
			for (int i = 0; i < _actorPositionObject.transform.childCount; i++)
				Destroy(_actorPositionObject.transform.GetChild(i).gameObject);
			
			foreach (ActorAspects aspects in actorAspects)
				aspects.aspectPanel.Hide();
			
			Hide();
			panelActorList.Show();
		}
		
		// -------------------------------------------------------------------------------
	    
	}

}

// =======================================================================================