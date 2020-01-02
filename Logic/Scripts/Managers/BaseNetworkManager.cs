// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// BaseNetworkManager
	/// <summary>
	/// This manager class is present on both the client as well as the server and handles
	/// all network messages. On the server-side, it is linked to both the DatabaseManager
	/// as well as the accountManager. And on the client-side, this class is linked to the
	/// ClientManager. The actual execution of network messages is up to those managers.
	/// </summary>
	// ===================================================================================
	[DisallowMultipleComponent]
	public partial class BaseNetworkManager : NetworkManager
	{
		
		public enum NetStateType { None, Offline, Handshake, InLobby, InGame }
		
		[Header("---------- [Optional] Linked Managers ----------")]
		public DatabaseManager databaseManager;
		public AccountManager accountManager;
		public ClientManager clientManager;
		
		[HideInInspector] public NetStateType netState = NetStateType.None;
		
		protected const string sFolderEntities = "Entities";
		
		// -------------------------------------------------------------------------------
		// Start
		// -------------------------------------------------------------------------------
		void Start()
		{
			AutoFindManagers();
			AutoRegisterSpawnablePrefabs();
			AutoStart();
		}
		
		// -------------------------------------------------------------------------------
		// CheckNetState
		// -------------------------------------------------------------------------------
		public bool CheckNetState(NetStateType _netState) {
			return netState == _netState;
		}
			
		// -------------------------------------------------------------------------------
		// AutoRegisterSpawnablePrefabs
		// -------------------------------------------------------------------------------
		protected void AutoRegisterSpawnablePrefabs()
		{
       		spawnPrefabs.AddRange(Resources.LoadAll<GameObject>(sFolderEntities));
		}
       	
 		// -------------------------------------------------------------------------------
		// AutoFindManagers
		// -------------------------------------------------------------------------------
       	protected void AutoFindManagers()
       	{
       		if (!databaseManager) 	databaseManager = FindObjectOfType<DatabaseManager>();
       		if (!accountManager) 	accountManager 	= FindObjectOfType<AccountManager>();
       		if (!clientManager) 	clientManager 	= FindObjectOfType<ClientManager>();
       	}
       	
 		// -------------------------------------------------------------------------------
		// AutoRegisterServerHandlers
		// -------------------------------------------------------------------------------
       	protected void AutoRegisterServerHandlers()
       	{
       		NetworkServer.RegisterHandler(MsgReqCheckVersion.nId, 					OnReqCheckVersion);
       		NetworkServer.RegisterHandler(MsgReqAccountRegister.nId, 				OnReqAccountRegister);
       		NetworkServer.RegisterHandler(MsgReqAccountLogin.nId, 					OnReqAccountLogin);
       		NetworkServer.RegisterHandler(MsgReqAccountLogout.nId, 					OnReqAccountLogout);
       		NetworkServer.RegisterHandler(MsgReqCodeConfirm.nId, 					OnReqCodeConfirm);
       		NetworkServer.RegisterHandler(MsgReqAccountResendConfirmation.nId, 		OnReqAccountResendConfirmation);
       		NetworkServer.RegisterHandler(MsgReqAccountChangePassword.nId, 			OnReqAccountChangePassword);
       		NetworkServer.RegisterHandler(MsgReqAccountChangeMail.nId, 				OnReqAccountChangeMail);
       		NetworkServer.RegisterHandler(MsgReqAccountChangeName.nId, 				OnReqAccountChangeName);
       		NetworkServer.RegisterHandler(MsgReqAccountForgotPassword.nId, 			OnReqAccountForgotPassword);
       		NetworkServer.RegisterHandler(MsgReqAccountDelete.nId, 					OnReqAccountDelete);
       		NetworkServer.RegisterHandler(MsgReqActorPlayerCreate.nId, 				OnMsgReqActorPlayerCreate);
       		NetworkServer.RegisterHandler(MsgReqActorPlayerDelete.nId, 				OnMsgReqActorPlayerDelete);
       		NetworkServer.RegisterHandler(MsgReqActorPlayerList.nId, 				OnMsgReqActorPlayerList);
       		NetworkServer.RegisterHandler(MsgReqActorPlayerLogin.nId, 				OnMsgReqActorPlayerLogin);
       	}
       	
 		// -------------------------------------------------------------------------------
		// AutoRegisterClientHandlers
		// -------------------------------------------------------------------------------
       	protected void AutoRegisterClientHandlers()
       	{
       		client.RegisterHandler(MsgAckCheckVersion.nId, 							OnAckCheckVersion);
       		client.RegisterHandler(MsgAckAccountRegister.nId, 						OnAckAccountRegister);
       		client.RegisterHandler(MsgAckAccountLogin.nId, 							OnAckAccountLogin);
       		client.RegisterHandler(MsgAckAccountLogout.nId,							OnAckAccountLogout);
       		client.RegisterHandler(MsgAckCodeConfirm.nId, 							OnAckCodeConfirm);
       		client.RegisterHandler(MsgAckAccountResendConfirmation.nId, 			OnAckAccountResendConfirmation);
       		client.RegisterHandler(MsgAckAccountChangePassword.nId, 				OnAckAccountChangePassword);
       		client.RegisterHandler(MsgAckAccountChangeMail.nId, 					OnAckAccountChangeMail);
       		client.RegisterHandler(MsgAckAccountChangeName.nId, 					OnAckAccountChangeName);
       		client.RegisterHandler(MsgAckAccountForgotPassword.nId, 				OnAckAccountForgotPassword);
       		client.RegisterHandler(MsgAckAccountDelete.nId, 						OnAckAccountDelete);
       		client.RegisterHandler(MsgAckActorPlayerCreate.nId, 					OnMsgAckActorPlayerCreate);
       		client.RegisterHandler(MsgAckActorPlayerDelete.nId, 					OnMsgAckActorPlayerDelete);
       		client.RegisterHandler(MsgAckActorPlayerList.nId,	 					OnMsgAckActorPlayerList);
       		client.RegisterHandler(MsgAckActorPlayerLogin.nId,	 					OnMsgAckActorPlayerLogin);
       	}
       	
  		// -------------------------------------------------------------------------------
		// AutoStart
		// -------------------------------------------------------------------------------
       	protected void AutoStart()
       	{
#if OM_SERVER && !OM_CLIENT
			StartServer();
#elif !OM_SERVER && OM_CLIENT
			StartClient();
#elif OM_SERVER && OM_CLIENT
			StartHost();
#endif
       	}
       
   		// -------------------------------------------------------------------------------
		// OnClientConnect
		// -------------------------------------------------------------------------------
    	public override void OnClientConnect(NetworkConnection connection)
    	{
    		AutoRegisterClientHandlers();
    		ClientScene.Ready(connection);
    		netState = NetStateType.Offline;
    	}
    	
   		// -------------------------------------------------------------------------------
		// OnStartServer
		// -------------------------------------------------------------------------------
    	public override void OnStartServer()
    	{
    		AutoRegisterServerHandlers();
    		base.OnStartServer();
    	}

       	// -------------------------------------------------------------------------------
		// OnStopServer
		// -------------------------------------------------------------------------------
    	public override void OnStopServer()
    	{
    		base.OnStopServer();
    	}
    	
    	// ===============================================================================
    	// ACCOUNT RELATED MESSAGE HANDLERS
    	// ===============================================================================
    	
      	// -------------------------------------------------------------------------------
		// OnReqCheckVersion
		// -------------------------------------------------------------------------------
		public void OnReqCheckVersion(NetworkMessage networkMessage)
		{
			MsgReqCheckVersion message = networkMessage.ReadMessage<MsgReqCheckVersion>();
			networkMessage.conn.Send(MsgAckCheckVersion.nId, accountManager.ReqCheckVersion(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnAckCheckVersion
		// -------------------------------------------------------------------------------
		public void OnAckCheckVersion(NetworkMessage networkMessage)
		{
			MsgAckCheckVersion message = networkMessage.ReadMessage<MsgAckCheckVersion>();
			clientManager.AckCheckVersion(message, networkMessage.conn);
		}
		
		// -------------------------------------------------------------------------------
		// OnReqAccountRegister
		// -------------------------------------------------------------------------------
		public void OnReqAccountRegister(NetworkMessage networkMessage)
		{
			MsgReqAccountRegister message = networkMessage.ReadMessage<MsgReqAccountRegister>();
			networkMessage.conn.Send(MsgAckAccountRegister.nId, accountManager.ReqAccountRegister(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnAckAccountRegister
		// -------------------------------------------------------------------------------
		public void OnAckAccountRegister(NetworkMessage networkMessage)
		{
			MsgAckAccountRegister message = networkMessage.ReadMessage<MsgAckAccountRegister>();
			clientManager.AckRegisterAccount(message, networkMessage.conn);
		}
		
		// -------------------------------------------------------------------------------
		// OnReqAccountLogin
		// -------------------------------------------------------------------------------
		public void OnReqAccountLogin(NetworkMessage networkMessage)
		{
			MsgReqAccountLogin message = networkMessage.ReadMessage<MsgReqAccountLogin>();
			networkMessage.conn.Send(MsgAckAccountLogin.nId, accountManager.ReqAccountLogin(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnAckAccountLogin
		// -------------------------------------------------------------------------------
		public void OnAckAccountLogin(NetworkMessage networkMessage)
		{
			MsgAckAccountLogin message = networkMessage.ReadMessage<MsgAckAccountLogin>();
			clientManager.AckAccountLogin(message, networkMessage.conn);
		}	
		
		// -------------------------------------------------------------------------------
		// OnReqAccountLogout
		// -------------------------------------------------------------------------------
		public void OnReqAccountLogout(NetworkMessage networkMessage)
		{
			MsgReqAccountLogout message = networkMessage.ReadMessage<MsgReqAccountLogout>();
			networkMessage.conn.Send(MsgAckAccountLogout.nId, accountManager.ReqAccountLogout(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnAckAccountLogout
		// -------------------------------------------------------------------------------
		public void OnAckAccountLogout(NetworkMessage networkMessage)
		{
			MsgAckAccountLogout message = networkMessage.ReadMessage<MsgAckAccountLogout>();
			clientManager.AckAccountLogout(message, networkMessage.conn);
		}
		
		// -------------------------------------------------------------------------------
		// OnReqCodeConfirm
		// -------------------------------------------------------------------------------
		public void OnReqCodeConfirm(NetworkMessage networkMessage)
		{
			MsgReqCodeConfirm message = networkMessage.ReadMessage<MsgReqCodeConfirm>();
			networkMessage.conn.Send(MsgAckCodeConfirm.nId, accountManager.ReqCodeConfirm(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnAckCodeConfirm
		// -------------------------------------------------------------------------------
		public void OnAckCodeConfirm(NetworkMessage networkMessage)
		{
			MsgAckCodeConfirm message = networkMessage.ReadMessage<MsgAckCodeConfirm>();
			clientManager.AckCodeConfirm(message, networkMessage.conn);
		}
			
		// -------------------------------------------------------------------------------
		// OnReqAccountResendConfirmation
		// -------------------------------------------------------------------------------
		public void OnReqAccountResendConfirmation(NetworkMessage networkMessage)
		{
			MsgReqAccountResendConfirmation message = networkMessage.ReadMessage<MsgReqAccountResendConfirmation>();
			networkMessage.conn.Send(MsgAckAccountResendConfirmation.nId, accountManager.ReqAccountResendConfirmation(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnAckAccountResendConfirmation
		// -------------------------------------------------------------------------------
		public void OnAckAccountResendConfirmation(NetworkMessage networkMessage)
		{
			MsgAckAccountResendConfirmation message = networkMessage.ReadMessage<MsgAckAccountResendConfirmation>();
			clientManager.AckAccountResendConfirmation(message, networkMessage.conn);
		}
		
		// -------------------------------------------------------------------------------
		// OnReqAccountChangePassword
		// -------------------------------------------------------------------------------
		public void OnReqAccountChangePassword(NetworkMessage networkMessage)
		{
			MsgReqAccountChangePassword message = networkMessage.ReadMessage<MsgReqAccountChangePassword>();
			networkMessage.conn.Send(MsgAckAccountChangePassword.nId, accountManager.ReqAccountChangePassword(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnAckAccountChangePassword
		// -------------------------------------------------------------------------------
		public void OnAckAccountChangePassword(NetworkMessage networkMessage)
		{
			MsgAckAccountChangePassword message = networkMessage.ReadMessage<MsgAckAccountChangePassword>();
			clientManager.AckAccountChangePassword(message, networkMessage.conn);
		}
		
		// -------------------------------------------------------------------------------
		// OnReqAccountChangeMail
		// -------------------------------------------------------------------------------
		public void OnReqAccountChangeMail(NetworkMessage networkMessage)
		{
			MsgReqAccountChangeMail message = networkMessage.ReadMessage<MsgReqAccountChangeMail>();
			networkMessage.conn.Send(MsgAckAccountChangeMail.nId, accountManager.ReqAccountChangeMail(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnAckAccountChangeMail
		// -------------------------------------------------------------------------------
		public void OnAckAccountChangeMail(NetworkMessage networkMessage)
		{
			MsgAckAccountChangeMail message = networkMessage.ReadMessage<MsgAckAccountChangeMail>();
			clientManager.AckAccountChangeMail(message, networkMessage.conn);
		}
		
		// -------------------------------------------------------------------------------
		// OnReqAccountChangeName
		// -------------------------------------------------------------------------------
		public void OnReqAccountChangeName(NetworkMessage networkMessage)
		{
			MsgReqAccountChangeName message = networkMessage.ReadMessage<MsgReqAccountChangeName>();
			networkMessage.conn.Send(MsgAckAccountChangeName.nId, accountManager.ReqAccountChangeName(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnAckAccountChangeName
		// -------------------------------------------------------------------------------
		public void OnAckAccountChangeName(NetworkMessage networkMessage)
		{
			MsgAckAccountChangeName message = networkMessage.ReadMessage<MsgAckAccountChangeName>();
			clientManager.AckAccountChangeName(message, networkMessage.conn);
		}
		
		// -------------------------------------------------------------------------------
		// OnReqAccountForgotPassword
		// -------------------------------------------------------------------------------
		public void OnReqAccountForgotPassword(NetworkMessage networkMessage)
		{
			MsgReqAccountForgotPassword message = networkMessage.ReadMessage<MsgReqAccountForgotPassword>();
			networkMessage.conn.Send(MsgAckAccountForgotPassword.nId, accountManager.ReqAccountForgotPassword(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnAckAccountForgotPassword
		// -------------------------------------------------------------------------------
		public void OnAckAccountForgotPassword(NetworkMessage networkMessage)
		{
			MsgAckAccountForgotPassword message = networkMessage.ReadMessage<MsgAckAccountForgotPassword>();
			clientManager.AckAccountForgotPassword(message, networkMessage.conn);
		}
		
		// -------------------------------------------------------------------------------
		// OnReqAccountDelete
		// -------------------------------------------------------------------------------
		public void OnReqAccountDelete(NetworkMessage networkMessage)
		{
			MsgReqAccountDelete message = networkMessage.ReadMessage<MsgReqAccountDelete>();
			networkMessage.conn.Send(MsgReqAccountDelete.nId, accountManager.ReqAccountDelete(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnAckAccountDelete
		// -------------------------------------------------------------------------------
		public void OnAckAccountDelete(NetworkMessage networkMessage)
		{
			MsgAckAccountDelete message = networkMessage.ReadMessage<MsgAckAccountDelete>();
			clientManager.AckAccountDelete(message, networkMessage.conn);
		}
		
    	// ===============================================================================
    	// ACTOR RELATED
    	// ===============================================================================
		
		// -------------------------------------------------------------------------------
		// OnMsgReqActorPlayerCreate
		// -------------------------------------------------------------------------------
		public void OnMsgReqActorPlayerCreate(NetworkMessage networkMessage)
		{
			MsgReqActorPlayerCreate message = networkMessage.ReadMessage<MsgReqActorPlayerCreate>();
			networkMessage.conn.Send(MsgAckActorPlayerCreate.nId, accountManager.ReqActorPlayerCreate(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnMsgAckActorPlayerCreate
		// -------------------------------------------------------------------------------
		public void OnMsgAckActorPlayerCreate(NetworkMessage networkMessage)
		{
			MsgAckActorPlayerCreate message = networkMessage.ReadMessage<MsgAckActorPlayerCreate>();
			clientManager.AckActorPlayerCreate(message, networkMessage.conn);
		}
		
		// -------------------------------------------------------------------------------
		// OnMsgReqActorPlayerDelete
		// -------------------------------------------------------------------------------
		public void OnMsgReqActorPlayerDelete(NetworkMessage networkMessage)
		{
			MsgReqActorPlayerDelete message = networkMessage.ReadMessage<MsgReqActorPlayerDelete>();
			networkMessage.conn.Send(MsgAckActorPlayerDelete.nId, accountManager.ReqActorPlayerDelete(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnMsgAckActorPlayerDelete
		// -------------------------------------------------------------------------------
		public void OnMsgAckActorPlayerDelete(NetworkMessage networkMessage)
		{
			MsgAckActorPlayerDelete message = networkMessage.ReadMessage<MsgAckActorPlayerDelete>();
			clientManager.AckActorPlayerDelete(message, networkMessage.conn);
		}
		
		// -------------------------------------------------------------------------------
		// OnMsgReqActorPlayerList
		// -------------------------------------------------------------------------------
		public void OnMsgReqActorPlayerList(NetworkMessage networkMessage)
		{
			MsgReqActorPlayerList message = networkMessage.ReadMessage<MsgReqActorPlayerList>();
			networkMessage.conn.Send(MsgAckActorPlayerList.nId, accountManager.ReqActorPlayerList(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnMsgAckActorPlayerList
		// -------------------------------------------------------------------------------
		public void OnMsgAckActorPlayerList(NetworkMessage networkMessage)
		{
			MsgAckActorPlayerList message = networkMessage.ReadMessage<MsgAckActorPlayerList>();
			clientManager.AckActorPlayerList(message, networkMessage.conn);
		}
		
		// -------------------------------------------------------------------------------
		// OnMsgReqActorPlayerLogin
		// -------------------------------------------------------------------------------
		public void OnMsgReqActorPlayerLogin(NetworkMessage networkMessage)
		{
			MsgReqActorPlayerLogin message = networkMessage.ReadMessage<MsgReqActorPlayerLogin>();
			networkMessage.conn.Send(MsgAckActorPlayerLogin.nId, accountManager.ReqActorPlayerLogin(message, networkMessage.conn));
		}
		
		// -------------------------------------------------------------------------------
		// OnMsgAckActorPlayerLogin
		// -------------------------------------------------------------------------------
		public void OnMsgAckActorPlayerLogin(NetworkMessage networkMessage)
		{
			MsgAckActorPlayerLogin message = networkMessage.ReadMessage<MsgAckActorPlayerLogin>();
			clientManager.AckActorPlayerLogin(message, networkMessage.conn);
		}
		
		
		// -------------------------------------------------------------------------------
		
	}
	
}