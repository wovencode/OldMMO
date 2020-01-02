// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// ClientManager
	/// <summary>
	/// This client-side class acts as intermediate between the NetworkManager and the
	/// client-sided UI system. It also handles callbacks to the UI system.
	///
	/// In order to exchange messages with the UI and other parts of the client, both
	/// actions and a array of strings are used. It should be noted that all activities are
	/// just client side, not networked and not time critical and therefore should be OK.
	/// 
	/// This approach also allows us to pass any kind of struct or class back and forth
	/// between the UI and the client manager via Json serialization from/to string.
	///
	/// The reason for this aproach is the action based handling of data, that required
	/// a unified aproach of how to transport parameters. Therefore a string array was
	/// chosen.
	///
	/// Another interesting point is that we can serialize/unserialize complete structs
	/// and classes using Json Utility, in combination with partial classes it allows us
	/// to expand the data payload without having to adjust the system every time.
	/// </summary>
	// ===================================================================================
	[DisallowMultipleComponent]
	public partial class ClientManager : BaseClientBehaviour
	{
	
		[Header("---------- [Required] Linked Scenes ----------")]
		public UnityScene accountGUIScene;
		public UnityScene gameGUIScene;
		
		[Header("---------- [Optional] Linked Managers ----------")]
		public BaseNetworkManager networkManager;
		
		[HideInInspector]public int actorsRemaining;
		[HideInInspector]public int accountsRemaining;
		[HideInInspector]public CAccount clientAccount;
		[HideInInspector]public bool bConfirmAccountCreate;
		[HideInInspector]public bool bConfirmAccountDelete;
		[HideInInspector]public bool bConfirmAccountChangeName;
		[HideInInspector]public bool bConfirmAccountChangePassword;
		[HideInInspector]public bool bConfirmAccountChangeMail;
		[HideInInspector]public bool bConfirmAccountForgotPassword;
		
		protected Dictionary<string,Action<string[]>> dictCallbacks;
		
		// -------------------------------------------------------------------------------
		// Start
		// -------------------------------------------------------------------------------
		void Start()
		{
			dictCallbacks = new Dictionary<string,Action<string[]>>();
			AutoFindManagers();
			
			SceneManager.LoadScene(accountGUIScene, LoadSceneMode.Additive);
		}
		
		// -------------------------------------------------------------------------------
		// AutoFindManagers
		// -------------------------------------------------------------------------------
       	protected void AutoFindManagers()
       	{
       		if (!networkManager) networkManager 	= FindObjectOfType<BaseNetworkManager>();
       	}
       	
       	// -------------------------------------------------------------------------------
		// CheckNetState
		// -------------------------------------------------------------------------------
		public bool CheckNetState(BaseNetworkManager.NetStateType _netState)
		{
			
			if (networkManager)
				return networkManager.CheckNetState(_netState);
				
			return false;
		}
		
		// -------------------------------------------------------------------------------
		// DictionaryAddAction
		// -------------------------------------------------------------------------------
		public void DictionaryAddAction(Action<string[]> callbackFunction)
		{
			if (!dictCallbacks.ContainsKey(callbackFunction.Method.Name))
				dictCallbacks.Add(callbackFunction.Method.Name, callbackFunction);
		}
		
		// -------------------------------------------------------------------------------
		// DictionaryDoAction
		// -------------------------------------------------------------------------------
		public void DictionaryDoAction(string sName, string[] parameters)
		{
		
			Action<string[]> callbackFunction;
			
			if (dictCallbacks.TryGetValue(sName, out callbackFunction))
				callbackFunction(parameters);
			else
				Debug.LogWarning("DictionaryDoAction: Method '" + sName + "' does not exist!");
				
		}
       	
		// ===============================================================================
		// ACCOUNT METHODS
		// ===============================================================================
				
		// -------------------------------------------------------------------------------
		// ReqCheckVersion
		// -------------------------------------------------------------------------------	
		public void ReqCheckVersion(string[] fields, Action<string[]> callbackFunction)
		{
			
			DictionaryAddAction(callbackFunction);
						
			MsgReqCheckVersion message = new MsgReqCheckVersion {
												sDeviceId 	= fields[0],
                        						sVersion 	= fields[1]
                    							};

			networkManager.client.Send(MsgReqCheckVersion.nId, message);
			
		}
		
		// -------------------------------------------------------------------------------
		// AckCheckVersion
		// -------------------------------------------------------------------------------	
		public void AckCheckVersion(MsgAckCheckVersion message, NetworkConnection connection = null)
		{
			
			string _bSuccess 		= (message.bSuccess) ? Constants.INT_SUCCESS.ToString() : Constants.INT_FAILURE.ToString();
			accountsRemaining 		= message.nAccountsRemaining;
			
			bConfirmAccountCreate			= message.bConfirmAccountCreate;
			bConfirmAccountDelete			= message.bConfirmAccountDelete;
			bConfirmAccountChangeName		= message.bConfirmAccountChangeName;
			bConfirmAccountChangePassword	= message.bConfirmAccountChangePassword;
			bConfirmAccountChangeMail		= message.bConfirmAccountChangeMail;
			bConfirmAccountForgotPassword	= message.bConfirmAccountForgotPassword;
			
			if (message.bSuccess)
				networkManager.netState = BaseNetworkManager.NetStateType.Handshake;
			
			DictionaryDoAction("CallbackCheckVersion", new string[] { _bSuccess, accountsRemaining.ToString() });
						
		}
		
		// -------------------------------------------------------------------------------
		// ReqRegisterAccount
		// -------------------------------------------------------------------------------	
		public void ReqRegisterAccount(string[] fields, Action<string[]> callbackFunction)
		{
			
			DictionaryAddAction(callbackFunction);
			
			MsgReqAccountRegister message = new MsgReqAccountRegister {
												sName 		= fields[0],
                        						sPassword 	= Tools.HashPassword(fields[1]),
                        						sMail 		= fields[2],
                        						sDeviceId	= fields[3]
                    							};

			networkManager.client.Send(MsgReqAccountRegister.nId, message);
			
		}
		
		// -------------------------------------------------------------------------------
		// AckRegisterAccount
		// -------------------------------------------------------------------------------
		public void AckRegisterAccount(MsgAckAccountRegister message, NetworkConnection connection = null)
		{
		
			string sResult = message.nResult.ToString();
			
			if (message.nResult == Constants.INT_SUCCESS || message.nResult == Constants.INT_CONFIRM)
			{
				accountsRemaining--;
			}
			
			DictionaryDoAction("CallbackRegisterAccount", new string[] { sResult });
			
		}

		// -------------------------------------------------------------------------------
		// ReqAccountLogin
		// -------------------------------------------------------------------------------	
		public void ReqAccountLogin(string[] fields, Action<string[]> callbackFunction)
		{
			
			DictionaryAddAction(callbackFunction);
			
			clientAccount = new CAccount();
				
			clientAccount.sName			= fields[0];
			
			MsgReqAccountLogin message = new MsgReqAccountLogin {
												sName 		= fields[0],
                        						sPassword 	= Tools.HashPassword(fields[1]),
                        						sDeviceId	= fields[2]
                    							};

			networkManager.client.Send(MsgReqAccountLogin.nId, message);
			
		}
		
		// -------------------------------------------------------------------------------
		// AckAccountLogin
		// -------------------------------------------------------------------------------
		public void AckAccountLogin(MsgAckAccountLogin message, NetworkConnection connection = null)
		{
			
			string sResult = message.nResult.ToString();
			
			if (message.nResult == Constants.INT_SUCCESS)
			{
				sResult = Constants.INT_SUCCESS.ToString();
				
				clientAccount = new CAccount();
				
				clientAccount.sName			= message.sName;
				clientAccount.sMail			= message.sMail;
				clientAccount.bBanned		= message.bBanned;
				clientAccount.bDeleted		= message.bDeleted;
				clientAccount.bConfirmed	= message.bConfirmed;
				
				actorsRemaining				= message.nActorsRemaining;
				
				networkManager.netState = BaseNetworkManager.NetStateType.InLobby;
				
			}
			
			DictionaryDoAction("CallbackLogin", new string[] { sResult });
			
		}

		// -------------------------------------------------------------------------------
		// ReqAccountLogout
		// -------------------------------------------------------------------------------	
		public void ReqAccountLogout(Action<string[]> callbackFunction)
		{
		
			DictionaryAddAction(callbackFunction);
			
			MsgReqAccountLogout message = new MsgReqAccountLogout { sName 		= clientAccount.sName };
			
			networkManager.client.Send(MsgReqAccountLogout.nId, message);
		}
		
		// -------------------------------------------------------------------------------
		// AckAccountLogout
		// -------------------------------------------------------------------------------
		public void AckAccountLogout(MsgAckAccountLogout message, NetworkConnection connection = null)
		{
			
			string sResult = (message.bSuccess) ? Constants.INT_SUCCESS.ToString() : Constants.INT_FAILURE.ToString();
			
			if (message.bSuccess)
			{
				clientAccount = null;
				networkManager.netState = BaseNetworkManager.NetStateType.Handshake;
			}
			
			DictionaryDoAction("CallbackAccountLogout", new string[] { sResult });
		}

		// -------------------------------------------------------------------------------
		// ReqCodeConfirm
		// -------------------------------------------------------------------------------
		public void ReqCodeConfirm(string[] fields, Action<string[]> callbackFunction)
		{
			
			DictionaryAddAction(callbackFunction);
			
			MsgReqCodeConfirm message = new MsgReqCodeConfirm {
												sName		= fields[0],
												nCode 		= Int32.Parse(fields[1]),
												nAction		= Int32.Parse(fields[2])
                    							};

			networkManager.client.Send(MsgReqCodeConfirm.nId, message);

		}

		// -------------------------------------------------------------------------------
		// AckCodeConfirm
		// -------------------------------------------------------------------------------
		public void AckCodeConfirm(MsgAckCodeConfirm message, NetworkConnection connection = null)
		{

			string sResult = (message.bSuccess) ? Constants.INT_SUCCESS.ToString() : Constants.INT_FAILURE.ToString();

			DictionaryDoAction("CallbackConfirmCode", new string[] { sResult });
			
		}
		
		// -------------------------------------------------------------------------------
		// ReqAccountResendConfirmation
		// -------------------------------------------------------------------------------
		public void ReqAccountResendConfirmation(string[] fields, Action<string[]> callbackFunction)
		{
		
			DictionaryAddAction(callbackFunction);
			
			MsgReqAccountResendConfirmation message = new MsgReqAccountResendConfirmation {
												sName 		= fields[0]
                    							};

			networkManager.client.Send(MsgReqAccountResendConfirmation.nId, message);

		}		

		// -------------------------------------------------------------------------------
		// AckAccountResendConfirmation
		// -------------------------------------------------------------------------------
		public void AckAccountResendConfirmation(MsgAckAccountResendConfirmation message, NetworkConnection connection = null)
		{
			string sResult = (message.bSuccess) ? Constants.INT_SUCCESS.ToString() : Constants.INT_FAILURE.ToString();
			DictionaryDoAction("CallbackResendConfirmation", new string[] { sResult });
		}
		
		// -------------------------------------------------------------------------------
		// ReqAccountChangePassword
		// -------------------------------------------------------------------------------
		public void ReqAccountChangePassword(string[] fields, Action<string[]> callbackFunction)
		{
		
			DictionaryAddAction(callbackFunction);
			
			MsgReqAccountChangePassword message = new MsgReqAccountChangePassword {
												sOldPassword 		= Tools.HashPassword(fields[0]),
                        						sNewPassword 		= Tools.HashPassword(fields[1])
                    							};

			networkManager.client.Send(MsgReqAccountChangePassword.nId, message);
			
		}		
		
		// -------------------------------------------------------------------------------
		// AckAccountChangePassword
		// -------------------------------------------------------------------------------
		public void AckAccountChangePassword(MsgAckAccountChangePassword message, NetworkConnection connection = null)
		{
			string sResult = message.nResult.ToString();
			DictionaryDoAction("CallbackAccountChangePassword", new string[] { sResult });
			
		}
		
		// -------------------------------------------------------------------------------
		// ReqAccountChangeMail
		// -------------------------------------------------------------------------------
		public void ReqAccountChangeMail(string[] fields, Action<string[]> callbackFunction)
		{
		
			DictionaryAddAction(callbackFunction);
			
			MsgReqAccountChangeMail message = new MsgReqAccountChangeMail {
                        						sMail 		= fields[0],
                        						sPassword	= Tools.HashPassword(fields[1])
                    							};

			networkManager.client.Send(MsgReqAccountChangeMail.nId, message);
			
		}	
		
		// -------------------------------------------------------------------------------
		// AckAccountChangeMail
		// -------------------------------------------------------------------------------
		public void AckAccountChangeMail(MsgAckAccountChangeMail message, NetworkConnection connection = null)
		{
			string sResult = message.nResult.ToString();
			DictionaryDoAction("CallbackAccountChangeMail", new string[] { sResult });
		}
		
		// -------------------------------------------------------------------------------
		// ReqAccountChangeName
		// -------------------------------------------------------------------------------
		public void ReqAccountChangeName(string[] fields, Action<string[]> callbackFunction)
		{
		
			DictionaryAddAction(callbackFunction);
			
			MsgReqAccountChangeName message = new MsgReqAccountChangeName {
												sName 		= fields[0],
												sPassword	= Tools.HashPassword(fields[1])
                        						};

			networkManager.client.Send(MsgReqAccountChangeName.nId, message);
			
		}	
		
		// -------------------------------------------------------------------------------
		// AckAccountChangeName
		// -------------------------------------------------------------------------------
		public void AckAccountChangeName(MsgAckAccountChangeName message, NetworkConnection connection = null)
		{
			string sResult = message.nResult.ToString();
			DictionaryDoAction("CallbackAccountChangeName", new string[] { sResult });
		}
		
		// -------------------------------------------------------------------------------
		// ReqAccountDelete
		// -------------------------------------------------------------------------------
		public void ReqAccountDelete(string[] fields, Action<string[]> callbackFunction)
		{
		
			DictionaryAddAction(callbackFunction);
			
			MsgReqAccountDelete message = new MsgReqAccountDelete {
											sPassword	= Tools.HashPassword(fields[0])
											};

			networkManager.client.Send(MsgReqAccountDelete.nId, message);
			
		}	
		
		// -------------------------------------------------------------------------------
		// AckAccountDelete
		// -------------------------------------------------------------------------------
		public void AckAccountDelete(MsgAckAccountDelete message, NetworkConnection connection = null)
		{
			string sResult = message.nResult.ToString();
			
			if (message.nResult == Constants.INT_SUCCESS || message.nResult == Constants.INT_CONFIRM)
			{
				accountsRemaining--;
			}
			
			DictionaryDoAction("CallbackConfirmAccountDelete", new string[] { sResult });
		}
		
		// -------------------------------------------------------------------------------
		// ReqAccountForgotPassword
		// -------------------------------------------------------------------------------
		public void ReqAccountForgotPassword(string[] fields, Action<string[]> callbackFunction)
		{
		
			DictionaryAddAction(callbackFunction);
			
			clientAccount = new CAccount();
				
			clientAccount.sName			= fields[0];
			
			MsgReqAccountForgotPassword message = new MsgReqAccountForgotPassword {
													sName	= fields[0]
													};

			networkManager.client.Send(MsgReqAccountForgotPassword.nId, message);
			
		}	
		
		// -------------------------------------------------------------------------------
		// AckAccountForgotPassword
		// -------------------------------------------------------------------------------
		public void AckAccountForgotPassword(MsgAckAccountForgotPassword message, NetworkConnection connection = null)
		{
			string sResult = message.nResult.ToString();
			DictionaryDoAction("CallbackAccountForgotPassword", new string[] { sResult });
		}
		
		// ===============================================================================
		// ACTOR METHODS
		// ===============================================================================
		
		// -------------------------------------------------------------------------------
		// ReqActorPlayerCreate
		// -------------------------------------------------------------------------------
		public void ReqActorPlayerCreate(string sName, string[] fields, Action<string[]> callbackFunction)
		{
			
			DictionaryAddAction(callbackFunction);
		
			MsgReqActorPlayerCreate message = new MsgReqActorPlayerCreate {
													sName	= sName,
													sAspects = fields
													};

			networkManager.client.Send(MsgReqActorPlayerCreate.nId, message);
			
		}
		
		// -------------------------------------------------------------------------------
		// AckActorPlayerCreate
		// -------------------------------------------------------------------------------
		public void AckActorPlayerCreate(MsgAckActorPlayerCreate message, NetworkConnection connection = null)
		{
			string sResult = (message.bSuccess) ? Constants.INT_SUCCESS.ToString() : Constants.INT_FAILURE.ToString();
			
			if (message.bSuccess)
			{
				actorsRemaining--;
			}
			
			DictionaryDoAction("CallbackActorCreate", new string[] { sResult });
		}
		
		// -------------------------------------------------------------------------------
		// ReqActorPlayerDelete
		// -------------------------------------------------------------------------------
		public void ReqActorPlayerDelete(string _sName, Action<string[]> callbackFunction)
		{
		
			DictionaryAddAction(callbackFunction);
		
			MsgReqActorPlayerDelete message = new MsgReqActorPlayerDelete { sName = _sName};

			networkManager.client.Send(MsgReqActorPlayerDelete.nId, message);
			
		}
		
		// -------------------------------------------------------------------------------
		// AckActorPlayerDelete
		// -------------------------------------------------------------------------------
		public void AckActorPlayerDelete(MsgAckActorPlayerDelete message, NetworkConnection connection = null)
		{
			string sResult = (message.bSuccess) ? Constants.INT_SUCCESS.ToString() : Constants.INT_FAILURE.ToString();
			
			if (message.bSuccess)
				actorsRemaining--;
			
			DictionaryDoAction("CallbackDeleteActor", new string[] { sResult });
		}
		
		// -------------------------------------------------------------------------------
		// ReqActorPlayerList
		// -------------------------------------------------------------------------------
		public void ReqActorPlayerList(string[] fields, Action<string[]> callbackFunction)
		{
		
			DictionaryAddAction(callbackFunction);
		
			MsgReqActorPlayerList message = new MsgReqActorPlayerList { };

			networkManager.client.Send(MsgReqActorPlayerList.nId, message);
			
		}
		
		// -------------------------------------------------------------------------------
		// AckActorPlayerList
		// -------------------------------------------------------------------------------
		public void AckActorPlayerList(MsgAckActorPlayerList message, NetworkConnection connection = null)
		{
			
			List<string> previews = new List<string>();

			foreach (SActorPlayerPreview preview in message.sActorPlayerPreviews)
			{
				
				previews.Add(JsonUtility.ToJson(preview));
				
			}
			
			
			DictionaryDoAction("CallbackActorPlayerList", previews.ToArray() );
		}
		
		
		// -------------------------------------------------------------------------------
		// ReqActorPlayerLogin
		// -------------------------------------------------------------------------------
		public void ReqActorPlayerLogin(string[] fields, Action<string[]> callbackFunction)
		{
		
			DictionaryAddAction(callbackFunction);
		
			MsgReqActorPlayerLogin message = new MsgReqActorPlayerLogin { };

			networkManager.client.Send(MsgReqActorPlayerLogin.nId, message);
			
		}
		
		// -------------------------------------------------------------------------------
		// AckActorPlayerLogin
		// -------------------------------------------------------------------------------
		public void AckActorPlayerLogin(MsgAckActorPlayerLogin message, NetworkConnection connection = null)
		{
			
			// -->
			
			//DictionaryDoAction("CallbackActorPlayerLogin", previews.ToArray() );
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}