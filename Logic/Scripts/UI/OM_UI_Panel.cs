// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Collections.Generic;
using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_Panel
	// ===================================================================================
	public abstract partial class OM_UI_Panel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	
		public enum panType		{ alwaysEnabled, onlyOffline, onlyHandshake, onlyInLobby, onlyInGame };
		public enum panEvents	{ none, autoShow, autoHide, autoBoth };
		public enum panInit		{ always, once, never };
		
		[HideInInspector] public ClientManager clientManager;
		
		[Header("---------- [Optional] Panel Settings ----------")]
	    public GameObject panelRoot;
	    public panType panelType;
	    public panEvents panelEvents;
	    public panInit panelInit;
	    public bool panelDragable;
	    [Range(0,99)]public float updateInterval = 1f;
	    [Range(0,99)]public float invokeRepeating;
	    [Range(1,10)]public float clickDelayTime = 1f;
	    
	    protected Button 	tmpDisabledButton;
	    protected bool 		_initialized;
	    protected bool 		_visible;
	    protected float 	_cacheTimer;
	    
	    protected OM_UI_PanelTooltip panelTooltip;
	    
	    protected GameObject _cameraPositionObject;
		protected GameObject _cameraTargetObject;
	    
	    // -------------------------------------------------------------------------------
		// Start
		// -------------------------------------------------------------------------------
		void Start()
		{
			if (!clientManager) 	clientManager 	= FindObjectOfType<ClientManager>();
			if (!panelTooltip) 		panelTooltip 	= FindObjectOfType<OM_UI_PanelTooltip>();
		}
	    
	    //--------------------------------------------------------------------------------
		// initialized
		//--------------------------------------------------------------------------------
		public bool initialized {
			get { return _initialized; }
			set { _initialized = value; }
		}
		
	    //--------------------------------------------------------------------------------
		// OnChildEnable
		//--------------------------------------------------------------------------------
		public abstract void OnChildEnable();

	    //--------------------------------------------------------------------------------
		// OnInvokeRepeating
		//--------------------------------------------------------------------------------	    
	    public abstract void OnInvokeRepeating();
	    
	    //--------------------------------------------------------------------------------
		// SlowUpdate
		//--------------------------------------------------------------------------------
	    public abstract void SlowUpdate();
	    
		//--------------------------------------------------------------------------------
		// Update
		//--------------------------------------------------------------------------------		
		public virtual void Update() {
			
			// Automatically hide this panel, if certain criteria are met
			if ((panelEvents == panEvents.autoHide || panelEvents == panEvents.autoBoth) &&
				panelRoot.activeInHierarchy &&
				(
				panelType == panType.onlyInGame && !clientManager.CheckNetState(BaseNetworkManager.NetStateType.InGame)  ||
				panelType == panType.onlyOffline && !clientManager.CheckNetState(BaseNetworkManager.NetStateType.Offline) ||
				panelType == panType.onlyInLobby && !clientManager.CheckNetState(BaseNetworkManager.NetStateType.InLobby) ||
				panelType == panType.onlyHandshake && !clientManager.CheckNetState(BaseNetworkManager.NetStateType.Handshake) ||
				panelType == panType.alwaysEnabled
				)
				
				) {
				this.Hide();
			}
			
			// Automatically show this panel, if certain criteria are met
			if ((panelEvents == panEvents.autoShow || panelEvents == panEvents.autoBoth) &&
				!panelRoot.activeInHierarchy &&
				!_initialized &&
				(
				panelType == panType.onlyInGame && clientManager.CheckNetState(BaseNetworkManager.NetStateType.InGame) ||
				panelType == panType.onlyOffline && clientManager.CheckNetState(BaseNetworkManager.NetStateType.Offline) ||
				panelType == panType.onlyInLobby && clientManager.CheckNetState(BaseNetworkManager.NetStateType.InLobby)  ||
				panelType == panType.onlyHandshake && clientManager.CheckNetState(BaseNetworkManager.NetStateType.Handshake)  ||
				panelType == panType.alwaysEnabled
				)
				
				) {
				this.Show();
			}
			
			// Only if the associated panel is active and the cache timer is finished,
			// call the custom method "SlowUpdate" once. This limits how often UI elements
			// get updated in general.
			if (panelRoot.activeSelf && Time.time > _cacheTimer)
			{
				_cacheTimer = Time.time + updateInterval;
				SlowUpdate();
			}
			
		}		
		
		//--------------------------------------------------------------------------------
		// Show
		//--------------------------------------------------------------------------------
		public virtual void Show() {
			if (panelRoot != null) {
				if (!panelRoot.activeInHierarchy &&
					(panelInit == panInit.always || panelInit == panInit.once && _initialized == false))
					_initialized = true;
					_visible = true;
					OnChildEnable();
					
					if (invokeRepeating > 0)
						InvokeRepeating("OnInvokeRepeating", invokeRepeating, invokeRepeating);
					
					transform.SetAsLastSibling();
					
					panelRoot.SetActive(true);
					
			} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}
		}
	
		//--------------------------------------------------------------------------------
		// Hide
		//--------------------------------------------------------------------------------
		public virtual void Hide() {
			if (panelRoot != null) {
			
				if (panelInit == panInit.always)
					_initialized = false;
				
				if (invokeRepeating > 0)
					CancelInvoke("OnInvokeRepeating");
					
				_visible = false;
				panelRoot.SetActive(false);
			} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}
		}
		
		//--------------------------------------------------------------------------------
		// Toggle
		//--------------------------------------------------------------------------------
		public virtual void Toggle() {
			if (panelRoot != null) {
				if (_visible) {
					Hide();
				} else {
					Show();
				}
			} else {
    			Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    		}
		}
		
		//--------------------------------------------------------------------------------
		// TemporaryDisable
		//--------------------------------------------------------------------------------
		protected void TemporaryDisable(Button obj) {
			if (obj != null) {
				tmpDisabledButton = obj;
				tmpDisabledButton.interactable = false;
				Invoke("Enable", clickDelayTime);
			}
		}
	
		//--------------------------------------------------------------------------------
		// Enable
		//--------------------------------------------------------------------------------
		protected void Enable() {
			if (tmpDisabledButton != null) {
				tmpDisabledButton.interactable = true;
			}	
		}
		
		//--------------------------------------------------------------------------------
		// onDrag
		//--------------------------------------------------------------------------------
		protected void onDrag(PointerEventData data) {
			if (panelDragable)
       			this.transform.Translate(data.delta);
    	}

    	public void OnBeginDrag(PointerEventData data) {
        	onDrag(data);
    	}

    	public void OnDrag(PointerEventData data) {
        	onDrag(data);
    	}

    	public void OnEndDrag(PointerEventData data) {
    	    onDrag(data);
    	}
    	
    	// -------------------------------------------------------------------------------
		// DestroyChildObjects
		// -------------------------------------------------------------------------------	
		protected void DestroyChildObjects(List<GameObject> parentObjects)
		{
			foreach (GameObject child in parentObjects)
			{
            	if (child.transform.childCount > 0)
                	Destroy(child.transform.GetChild(0).gameObject);
			}
		}

		//--------------------------------------------------------------------------------
	}

}

// =======================================================================================