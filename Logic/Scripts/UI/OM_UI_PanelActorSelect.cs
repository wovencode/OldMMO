// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelActorSelect
	// ===================================================================================
	public class OM_UI_PanelActorSelect : OM_UI_Panel {
		
		[Header("---------- [Required] Camera & Positions ----------")]
		public string 	cameraPositionObject;
		public string 	cameraTargetObject;
		public string[]	actorPositionObject;
		
		[Header("---------- [Required] Labels & Messages ----------")]
		public string labelButtonCreate 				= "Create Character ({0} left)";
		public string msgDeleteFail						= "Failed to delete character!";
		
		[Header("---------- [Required] Settings ----------")]
		public int actorAspects = 3;
		
		[Header("---------- [Required] UI Elements ----------")]
		public Button buttonActorCreate;
		public Button buttonActorDelete;
		public Button buttonActorLogin;
		public Button ButtonCancel;
		public Button buttonOptions;
		
		public OM_UI_PanelActorCreate 					panelActorCreate;
		public OM_UI_PanelAccountOptions 				panelAccountOptions;
		public OM_UI_PanelMain 							panelMain;
		public OM_UI_PanelOverlay						panelOverlay;
		public OM_UI_PanelMessage 						panelMessage;
		
		protected string								_selectedActorName;
		protected GameObject[]							_actorPositionObject;
		protected GameObject[]							_actorObject;
		
		//--------------------------------------------------------------------------------
		// OnChildEnable
		//--------------------------------------------------------------------------------
		public override void OnChildEnable()
		{
		
			if (!panelActorCreate) 		panelActorCreate 		= FindObjectOfType<OM_UI_PanelActorCreate>();
			if (!panelAccountOptions) 	panelAccountOptions 	= FindObjectOfType<OM_UI_PanelAccountOptions>();
			if (!panelMain) 			panelMain 				= FindObjectOfType<OM_UI_PanelMain>();
			if (!panelOverlay)			panelOverlay 			= FindObjectOfType<OM_UI_PanelOverlay>();
			if (!panelMessage) 			panelMessage 			= FindObjectOfType<OM_UI_PanelMessage>();
			
			buttonActorDelete.interactable = false;
			buttonActorLogin.interactable = false;
			
			if (buttonActorCreate != null) {
				buttonActorCreate.GetComponentInChildren<Text>().text = String.Format(labelButtonCreate, clientManager.actorsRemaining);
				if (clientManager.actorsRemaining < 1)
					buttonActorCreate.interactable = false;
			} else {
				Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
			}
			
			if (!_cameraPositionObject)		_cameraPositionObject	= GameObject.Find(cameraPositionObject);
			if (!_cameraTargetObject)		_cameraTargetObject		= GameObject.Find(cameraTargetObject);
			
			_actorPositionObject = new GameObject[actorPositionObject.Length];
			
			for (int i = 0; i < actorPositionObject.Length; ++i)
				_actorPositionObject[i] = GameObject.Find(actorPositionObject[i]);
			
			_actorObject = new GameObject[actorPositionObject.Length];
			
        	Camera.main.transform.position = _cameraPositionObject.transform.position;
        	
        	iTween.LookTo(Camera.main.gameObject, _cameraTargetObject.transform.position, 0f);
        	
		}
		
		//--------------------------------------------------------------------------------
		// Show
		//--------------------------------------------------------------------------------		
		public override void Show()
		{
			clientManager.ReqActorPlayerList(new string[] {}, CallbackActorPlayerList);
			base.Show();
		}
		
		//--------------------------------------------------------------------------------
		// CallbackActorPlayerList
		//--------------------------------------------------------------------------------		
		private void CallbackActorPlayerList(string[] result) {
						
			for (int i = 0; i < result.Length; ++i)
			{

				SActorPlayerPreview preview = JsonUtility.FromJson<SActorPlayerPreview>(result[i]);
				
				foreach (int aspectId in preview.sAspects)
				{
				
					TemplateAspect tmpl;
				
					if (DataManager.dictAspect.TryGetValue(aspectId, out tmpl))
					{
					
						if (_actorObject.Length > i && tmpl.actorPrefab)
						{
				
							_actorObject[i]						= Instantiate(tmpl.actorPrefab);
							_actorObject[i].name 				= preview.sName;
							_actorObject[i].transform.position 	= _actorPositionObject[i].transform.position;
							_actorObject[i].transform.parent 	= _actorPositionObject[i].transform;
					 		_actorObject[i].AddComponent<SelectableActor>();
					 		_actorObject[i].GetComponent<SelectableActor>().Init(this, preview.sName);
					 		
							break;
						}
					
					}
				}
				
			}
			
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
		// ClickSelectActor
		//--------------------------------------------------------------------------------
		public void ClickSelectActor(string sName)
		{	
			if (_selectedActorName != sName)
			{
				_selectedActorName = sName;
				buttonActorDelete.interactable = true;
				buttonActorLogin.interactable = true;
				
			}
			else
			{
				_selectedActorName = "";
				buttonActorDelete.interactable = false;
				buttonActorLogin.interactable = false;
			}
		}
		
		//--------------------------------------------------------------------------------
		// ClickCreateActor
		//--------------------------------------------------------------------------------		
		public void ClickCreateActor() {
			panelOverlay.Show();
			panelActorCreate.Show();
			Hide();
		}
		
		//--------------------------------------------------------------------------------
		// ClickDeleteActor
		//--------------------------------------------------------------------------------		
		public void ClickDeleteActor() {
			clientManager.ReqActorPlayerDelete(_selectedActorName, CallbackDeleteActor);
			TemporaryDisable(buttonActorDelete);
		}
		
		//--------------------------------------------------------------------------------
		// CallbackDeleteActor
		//--------------------------------------------------------------------------------		
		public void CallbackDeleteActor(string[] result)
		{
			if (result[0] == Constants.INT_FAILURE.ToString())
			{
				panelMessage.Show(msgDeleteFail);
			} 
			else 
			{
				foreach (GameObject go in _actorObject)
					if (go && go.name == _selectedActorName) Destroy(go);
				
				_selectedActorName = "";
			}
		}
		
		//--------------------------------------------------------------------------------
		// ClickLoginActor
		//--------------------------------------------------------------------------------		
		public void ClickLoginActor()
		{
			TemporaryDisable(buttonActorLogin);
			// -->
		}
		
		//--------------------------------------------------------------------------------
		// ClickOptions
		//--------------------------------------------------------------------------------		
		public void ClickOptions() {
			TemporaryDisable(buttonOptions);
			panelAccountOptions.Show();	
		}
		
		//--------------------------------------------------------------------------------
		// ClickCancel
		//--------------------------------------------------------------------------------		
		public void ClickCancel() {
			TemporaryDisable(ButtonCancel);
			clientManager.ReqAccountLogout(CallbackAccountLogout);
		}
		
		//--------------------------------------------------------------------------------
		// CallbackAccountLogout
		//--------------------------------------------------------------------------------		
		public void CallbackAccountLogout(string[] result) 
		{
			if (result[0] == Constants.INT_SUCCESS.ToString())
			{
				Hide();
				panelMain.Show();
			}
		}

		// -------------------------------------------------------------------------------
		// Hide
		// -------------------------------------------------------------------------------				
		public override void Hide()
		{
			foreach (GameObject go in _actorObject)
				if (go) Destroy(go);
				
			base.Hide();
			
		}
		
		//--------------------------------------------------------------------------------	
	    
	}

}

// =======================================================================================