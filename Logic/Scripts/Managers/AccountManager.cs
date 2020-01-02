// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// AccountManager
	/// <summary>
	/// This server-side class acts as intermediate between the NetworkManager and the
	/// server-sided DatabaseManager system. It handles all account related manipulation
	/// methods, the insertion of data into the Database as well as the return values
	/// from those methods.
	/// </summary>
	// ===================================================================================
	[DisallowMultipleComponent]
	public partial class AccountManager : BaseServerBehaviour
	{
		
		[Header("---------- [Optional] Account Settings ----------")]
		[Tooltip("[Optional] Limits the number of accounts that can be created on a single device")]
		[Range(1,99)]public int accountsPerDevice = 1;
		[Tooltip("[Optional] Limits the number of actors that can be created on a single account")]
		[Range(1,99)]public int actorsPerAccounts = 1;
		
		[Header("---------- [Optional] Confirmation Settings ----------")]
		[Tooltip("[Optional] New accounts must be confirmed, before logging in for the first time (forces eMail registration)")]
		public bool confirmAccountCreate;
		[Tooltip("[Optional] Accounts must be confirmed, when logging in from a different device (ignored without eMail)")]
		public bool confirmAccountLogin;
		[Tooltip("[Optional] Deleting account requires confirmation via eMail (disabled without eMail)")]
		public bool confirmAccountDelete;
		[Tooltip("[Optional] Changing account name requires confirmation via eMail (disabled without eMail)")]
		public bool confirmAccountChangeName;
		[Tooltip("[Optional] Changing account password requires confirmation via eMail (disabled without eMail)")]
		public bool confirmAccountChangePassword;
		[Tooltip("[Optional] Changing account eMail requires confirmation via eMail (disabled without eMail)")]
		public bool confirmAccountChangeMail;
		[Tooltip("[Optional] Requesting a temporary password requires confirmation via eMail (ignored without eMail)")]
		public bool confirmAccountForgotPassword;
		
		protected BaseNetworkManager 	networkManager;
		protected DatabaseManager 		databaseManager;
		protected MailManager 			mailManager;
		protected PasswordManager 		passwordManager;
		
		protected Dictionary<NetworkConnection, CAccount> dictLobby = new Dictionary<NetworkConnection, CAccount>();
		
		// -------------------------------------------------------------------------------
		// Awake
		// -------------------------------------------------------------------------------
		void Awake()
		{
#if OM_SERVER
			Startup();
#endif
		}
		
		// -------------------------------------------------------------------------------
		// OnDestroy
		// -------------------------------------------------------------------------------
		void OnDestroy()
		{
#if OM_SERVER
			Shutdown();
#endif
		}
		
		// -------------------------------------------------------------------------------
		// Startup
		// -------------------------------------------------------------------------------
		public void Startup()
		{
			AutoFindManagers();
		}
		
		// -------------------------------------------------------------------------------
		// Shutdown
		// -------------------------------------------------------------------------------
		public void Shutdown()
		{
			
		}
		
		// -------------------------------------------------------------------------------
		// AutoFindManagers
		// -------------------------------------------------------------------------------
       	protected void AutoFindManagers()
       	{
       		if (!networkManager) 	networkManager 		= FindObjectOfType<BaseNetworkManager>();
       		if (!databaseManager) 	databaseManager 	= FindObjectOfType<DatabaseManager>();
       		if (!mailManager) 		mailManager 		= FindObjectOfType<MailManager>();
       		if (!passwordManager) 	passwordManager 	= FindObjectOfType<PasswordManager>();
       	}
		
		// ===============================================================================
    	// GENERAL FUNCTIONS
    	// ===============================================================================
    	
    	// -------------------------------------------------------------------------------
		// AccountOnline
		// -------------------------------------------------------------------------------    	
    	public bool AccountOnline(NetworkConnection connection)
    	{
    		return dictLobby.ContainsKey(connection); //add check ingame here as well
    	}
    	
		// -------------------------------------------------------------------------------
		// ClientAccountsRemaining
		// -------------------------------------------------------------------------------		
		public int ClientAccountsRemaining(string sDeviceId)
		{
			return accountsPerDevice - databaseManager.GetAccountsOnDevice(sDeviceId);
		}
		
		// -------------------------------------------------------------------------------
		// AccountActorsRemaining
		// -------------------------------------------------------------------------------		
		public int AccountActorsRemaining(string sName)
		{
			return actorsPerAccounts - databaseManager.GetActorsOnAccount(sName);
		}
		
		// -------------------------------------------------------------------------------
		// AccountLoad
		// -------------------------------------------------------------------------------		
		public CAccount AccountLoad(string sName)
		{
			return databaseManager.AccountLoad(sName);
		}
		
		// ===============================================================================
    	// ACCOUNT RELATED
    	// ===============================================================================
			
		// -------------------------------------------------------------------------------
		// RequestSecurityCode
		// -------------------------------------------------------------------------------	
		public bool RequestSecurityCode(CAccount cAccount, Constants.AccountActionType accountActionType)
		{
			if (cAccount.IsEmpty || accountActionType == Constants.AccountActionType.None) return false;
			
			int tmpCode = cAccount.GenerateCode(accountActionType);
			mailManager.SendMailSecurityCode(cAccount.sMail, tmpCode);
			
			return databaseManager.AccountSave(cAccount);
		}		
		
		// -------------------------------------------------------------------------------
		// TryAccountCreate
		// Tries to create a new account (validating all required data etc.)
		// -------------------------------------------------------------------------------
		public CAccount TryAccountCreate(string _sName, string _sPassword, string _sMail, string _sDeviceId)
		{
			
			CAccount cAccount = null;
			
			// Validate Name and Password server side again
			// We only validate the eMail if its required to confirm the account, otherwise
			// the eMail is optional.
			if (_sName.validateName() &&
				!String.IsNullOrWhiteSpace(_sPassword) &&
				(!confirmAccountCreate || _sMail.validateEmail()) )
			{
				// Check if:
				// a. Account Name does not exist already
				// b. eMail is not in use by another account already
				if (!databaseManager.RowExists(DatabaseManager.tableAccounts, DatabaseManager.fieldName, _sName) && 
					!databaseManager.RowExists(DatabaseManager.tableAccounts, DatabaseManager.fieldEmail, _sMail) )
				{
					return AccountCreate(_sName, _sPassword, _sMail, _sDeviceId);
				}
			
			}
			
			return cAccount;
			
		}
		
		// -------------------------------------------------------------------------------
		// AccountCreate
		// Creates a new account with the given information
		// -------------------------------------------------------------------------------
		public CAccount AccountCreate(string _sName, string _sPassword, string _sMail, string _sDeviceId)
		{

			CAccount cAccount 	= new CAccount();
			cAccount.sName		= _sName;
			cAccount.sPassword	= passwordManager.CreateHash(_sPassword);
			cAccount.sMail		= _sMail;
			cAccount.sDeviceId	= _sDeviceId;
			
			// Now we:
			// a. Save Account + generate a security code and send via email (if required)
			// b. Or simply save the account
			if (confirmAccountCreate)
				RequestSecurityCode(cAccount, Constants.AccountActionType.ConfirmAccount);
			else
				databaseManager.AccountSave(cAccount);
			
			return cAccount;
			
		}
		
		// -------------------------------------------------------------------------------
		// AccountUpdate
		// -------------------------------------------------------------------------------
		public bool AccountUpdate(CAccount cAccount, NetworkConnection connection)
		{
		
			cAccount.ResetCode(false);
							
			if (dictLobby.ContainsKey(connection))
				dictLobby[connection] = cAccount;
			
			return databaseManager.AccountSave(cAccount);
									
		}
		
		// -------------------------------------------------------------------------------
		// AccountDelete
		// Soft or hard deletion of the stated account
		// -------------------------------------------------------------------------------
		public bool AccountDelete(CAccount cAccount, NetworkConnection connection, bool hardDelete = false)
		{
			
			if (cAccount == null) return false;
			
			if (hardDelete)
			{
				// Delete the account immediately
				return databaseManager.DeleteRows(DatabaseManager.tableAccounts, cAccount.sName);
			}
			else
			{
				// Update the account and set 'deleted' to true
				cAccount.bDeleted = true;
				return databaseManager.AccountSave(cAccount);
			}
			
		}
		
		// ===============================================================================
    	// ACTOR RELATED
    	// ===============================================================================
	
		// -------------------------------------------------------------------------------
		// TryActorPlayerCreate
		// -------------------------------------------------------------------------------
		public bool TryActorPlayerCreate(CAccount cAccount, string _sName, string[] fields)
		{
		
			// Validate Actor Name
			// Check Actors remaining on this account
			if (_sName.validateName() &&
				AccountActorsRemaining(cAccount.sName) > 0)
			{
				// Actor Name does not exist already
				if (!databaseManager.RowExists(DatabaseManager.tableActorPlayers, DatabaseManager.fieldName, _sName) )
				{
					return ActorPlayerCreate(cAccount, _sName, fields);
				}
			}
			
			return false;
			
		}
		
		// -------------------------------------------------------------------------------
		// ActorPlayerCreate
		// -------------------------------------------------------------------------------
		public bool ActorPlayerCreate(CAccount cAccount, string _sName, string[] fields)
		{
		
			bool bSuccess = false;
			List<TemplateAspect> actorAspects = new List<TemplateAspect>();
			GameObject actorObject = null;
			
			// -- determine the required prefab via the actors aspects
			for (int i = 0; i < fields.Length; ++i)
			{
				TemplateAspect tmpl;
				
				if (DataManager.dictAspect.TryGetValue(Int32.Parse(fields[i]), out tmpl))
				{
					actorAspects.Add(tmpl);
				}
				else
				{
					Debug.LogWarning("Skipped template '"+fields[i]+"' as it was not found in Library.");
				}
			
			}
			
			foreach (TemplateAspect aspect in actorAspects)
			{
				if (aspect.actorPrefab)
				{
				
					actorObject = Instantiate(aspect.actorPrefab);
					actorObject.GetComponent<MetaSubsystem>().Init(actorObject, _sName, cAccount.sName);
					actorObject.GetComponent<AspectSubsystem>().Init(actorObject, actorAspects.ToList());
										
					if (String.IsNullOrWhiteSpace(_sName))
					{
						actorObject.name = aspect.actorPrefab.name;
					}
					else
					{
						actorObject.name = _sName;
					}
				
					break;
				}
			}
			
			bSuccess = databaseManager.ActorPlayerSave(actorObject);
			GameObject.Destroy(actorObject);
			
			return bSuccess;
			
		}
		
		// -------------------------------------------------------------------------------
		// ActorPlayerDelete
		// Soft or hard deletion of the stated player
		// -------------------------------------------------------------------------------
		public bool ActorPlayerDelete(string sName, NetworkConnection connection, bool hardDelete = false)
		{
			if (!sName.validateName()) return false;
			return databaseManager.ActorPlayerDelete(sName, hardDelete);
		}
		
		// ===============================================================================
    	// NETWORK MESSAGE RELATED
    	// ===============================================================================
		
		// -------------------------------------------------------------------------------
		// ReqCheckVersion
		// Valide Client connect and version and return reply
		// -------------------------------------------------------------------------------	
		public MsgAckCheckVersion ReqCheckVersion(MsgReqCheckVersion message, NetworkConnection connection = null)
		{
			
			bool _bSuccess = (message.sVersion == Tools.GetVersion) ? true : false;
			int _nAccountsRemaining = ClientAccountsRemaining(message.sDeviceId);
			
			return new MsgAckCheckVersion {
											bSuccess 						= _bSuccess,
											nAccountsRemaining 				= _nAccountsRemaining,
											bConfirmAccountCreate			= confirmAccountCreate,
											bConfirmAccountDelete			= confirmAccountDelete,
											bConfirmAccountChangeName		= confirmAccountChangeName,
											bConfirmAccountChangePassword	= confirmAccountChangePassword,
											bConfirmAccountChangeMail		= confirmAccountChangeMail,
											bConfirmAccountForgotPassword	= confirmAccountForgotPassword
										};

		}		
		
		// -------------------------------------------------------------------------------
		// ReqAccountLogout
		// -------------------------------------------------------------------------------	
		public MsgAckAccountLogout ReqAccountLogout(MsgReqAccountLogout message, NetworkConnection connection = null)
		{
		
			bool _bSuccess 			= false;
			
			if (AccountOnline(connection))
			{
				dictLobby.Remove(connection);
				_bSuccess = true;
			}
			
			return new MsgAckAccountLogout { bSuccess = _bSuccess };

		}
		
		// -------------------------------------------------------------------------------
		// ReqAccountResendConfirmation
		// -------------------------------------------------------------------------------	
		public MsgAckAccountResendConfirmation ReqAccountResendConfirmation(MsgReqAccountResendConfirmation message, NetworkConnection connection = null)
		{
			
			bool _bSuccess = false;
			
			CAccount cAccount = AccountLoad(message.sName);
			
			if (cAccount != null && !cAccount.bConfirmed)
				_bSuccess = RequestSecurityCode(cAccount, Constants.AccountActionType.ConfirmAccount);
			
			return new MsgAckAccountResendConfirmation { bSuccess = _bSuccess };
		}
		
		// -------------------------------------------------------------------------------
		// ReqCodeConfirm
		// -------------------------------------------------------------------------------
		public MsgAckCodeConfirm ReqCodeConfirm(MsgReqCodeConfirm message, NetworkConnection connection = null)
		{
			
			bool _bSuccess 		= false;
			CAccount cAccount 	= AccountLoad(message.sName);

			if (cAccount != null &&
				!cAccount.IsEmpty &&
				cAccount.Action != Constants.AccountActionType.None)
			{
			
				if (cAccount.ValidateAll((Constants.AccountActionType)message.nAction, message.nCode)) {
					cAccount.ConfirmCode();
				
					if (dictLobby.ContainsKey(connection))
						dictLobby[connection] = cAccount;
				
					_bSuccess = databaseManager.AccountSave(cAccount);
				}
			
			}
			
			return new MsgAckCodeConfirm { bSuccess = _bSuccess };
			
		}
		
		// -------------------------------------------------------------------------------
		// ReqAccountRegister
		// -------------------------------------------------------------------------------	
		public MsgAckAccountRegister ReqAccountRegister(MsgReqAccountRegister message, NetworkConnection connection = null)
		{
		
			int _nResult			= confirmAccountCreate ? Constants.INT_CONFIRM : Constants.INT_SUCCESS;
			
			CAccount cAccount		= null;
			
			cAccount = TryAccountCreate(message.sName, message.sPassword, message.sMail, message.sDeviceId);
			
			_nResult				= (cAccount != null) ? _nResult : Constants.INT_FAILURE;
			
			return new MsgAckAccountRegister { nResult = _nResult };

		}
		
		// -------------------------------------------------------------------------------
		// ReqAccountLogin
		// -------------------------------------------------------------------------------	
		public MsgAckAccountLogin ReqAccountLogin(MsgReqAccountLogin message, NetworkConnection connection = null)
		{
			
			int _nResult			= confirmAccountCreate || confirmAccountLogin ? Constants.INT_CONFIRM : Constants.INT_FAILURE;
			int _nActorsRemaining 	= actorsPerAccounts;
			CAccount cAccount		= null;
			
			if (!AccountOnline(connection))
			{
			
				cAccount = databaseManager.AccountLoad(message.sName);

				if (cAccount != null && 
							cAccount.IsValid && 
							passwordManager.VerifyPassword(message.sPassword, cAccount.sPassword))
				{
				
					if (cAccount.bConfirmed || !confirmAccountCreate)
					{
						if (confirmAccountLogin && 
							!cAccount.IsDone && 
							!String.IsNullOrWhiteSpace(cAccount.sMail) &&
							message.sDeviceId != cAccount.sDeviceId)
						{
							RequestSecurityCode(cAccount, Constants.AccountActionType.LoginAccount);
							_nResult = Constants.INT_RECONFIRM;
						}
						else
						{
							dictLobby.Add(connection, cAccount);
							_nActorsRemaining = AccountActorsRemaining(message.sName);
							_nResult = Constants.INT_SUCCESS;
						}
					}
					
					return new MsgAckAccountLogin {
													nResult 			= _nResult,
													nActorsRemaining	= _nActorsRemaining,
													sName 				= cAccount.sName,
													sMail 				= cAccount.sMail,
													bBanned 			= cAccount.bBanned,
													bDeleted 			= cAccount.bDeleted,
													bConfirmed 			= cAccount.bConfirmed
												};
					
				}
				else
				{
					_nResult = Constants.INT_FAILURE;
				}
			}
			
			return new MsgAckAccountLogin { nResult = _nResult };
		
		}		
		
		// -------------------------------------------------------------------------------
		// ReqAccountDelete
		// -------------------------------------------------------------------------------	
		public MsgAckAccountDelete ReqAccountDelete(MsgReqAccountDelete message, NetworkConnection connection = null)
		{
		
			int _nResult			= confirmAccountDelete ? Constants.INT_CONFIRM : Constants.INT_FAILURE;
			CAccount cAccount 		= null;
			
			if (dictLobby.TryGetValue(connection, out cAccount))
			{
				if (passwordManager.VerifyPassword(message.sPassword, cAccount.sPassword))
				{
				
					if (confirmAccountDelete && !cAccount.IsDone)
						RequestSecurityCode(cAccount, Constants.AccountActionType.DeleteAccount);
						
					_nResult 		= AccountDelete(cAccount, connection, false) ? Constants.INT_SUCCESS : _nResult;
					
					if (dictLobby.ContainsKey(connection))
						dictLobby.Remove(connection);
					
				}
				else
				{
					_nResult = Constants.INT_FAILURE;
				}
			}
			
			return new MsgAckAccountDelete { nResult = _nResult };
			
		}
		
		// -------------------------------------------------------------------------------
		// ReqAccountChangeName
		// -------------------------------------------------------------------------------	
		public MsgAckAccountChangeName ReqAccountChangeName(MsgReqAccountChangeName message, NetworkConnection connection = null)
		{
		
			int _nResult			= confirmAccountChangeName ? Constants.INT_CONFIRM : Constants.INT_FAILURE;
			
			CAccount cAccount 		= null;
			
			if (dictLobby.TryGetValue(connection, out cAccount))
			{
				if (
					!databaseManager.RowExists(DatabaseManager.tableAccounts, DatabaseManager.fieldName, message.sName) &&
					passwordManager.VerifyPassword(message.sPassword, cAccount.sPassword))
				{
					if (confirmAccountChangeName && !cAccount.IsDone)
					{
						RequestSecurityCode(cAccount, Constants.AccountActionType.ChangeName);
					}
					else
					{
						if (AccountDelete(cAccount, connection, true))
						{
							cAccount.sName 	= message.sName;
							_nResult 		= AccountUpdate(cAccount, connection) ? Constants.INT_SUCCESS : _nResult;
						}
					}
				}
				else
				{
					_nResult = Constants.INT_FAILURE;
				}

			}
			
			return new MsgAckAccountChangeName { nResult = _nResult };
		}
		
		// -------------------------------------------------------------------------------
		// ReqAccountChangePassword
		// -------------------------------------------------------------------------------	
		public MsgAckAccountChangePassword ReqAccountChangePassword(MsgReqAccountChangePassword message, NetworkConnection connection = null)
		{
		
			int _nResult			= confirmAccountChangePassword ? Constants.INT_CONFIRM : Constants.INT_FAILURE;
			CAccount cAccount 		= null;
			
			if (dictLobby.TryGetValue(connection, out cAccount))
			{
				if (passwordManager.VerifyPassword(message.sOldPassword, cAccount.sPassword))
				{
					if (confirmAccountChangePassword && !cAccount.IsDone)
					{
						RequestSecurityCode(cAccount, Constants.AccountActionType.ChangePassword);
					}
					else
					{
						cAccount.sPassword = passwordManager.CreateHash(message.sNewPassword);
						_nResult 		= AccountUpdate(cAccount, connection) ? Constants.INT_SUCCESS : _nResult;
					}
				}
				else
				{
					_nResult = Constants.INT_FAILURE;
				}
			}
			
			return new MsgAckAccountChangePassword { nResult = _nResult };

		}		
		
		// -------------------------------------------------------------------------------
		// ReqAccountChangeMail
		// -------------------------------------------------------------------------------	
		public MsgAckAccountChangeMail ReqAccountChangeMail(MsgReqAccountChangeMail message, NetworkConnection connection = null)
		{
			
			int _nResult			= confirmAccountChangeMail ? Constants.INT_CONFIRM : Constants.INT_FAILURE;
			CAccount cAccount 		= null;
			
			if (dictLobby.TryGetValue(connection, out cAccount))
			{
				if (
					!databaseManager.RowExists(DatabaseManager.tableAccounts, DatabaseManager.fieldEmail, message.sMail) &&
					passwordManager.VerifyPassword(message.sPassword, cAccount.sPassword))
				{
					if (confirmAccountChangeMail && !cAccount.IsDone)
					{
						RequestSecurityCode(cAccount, Constants.AccountActionType.ChangeMail);
					}
					else
					{
						cAccount.sMail = message.sMail;
						_nResult 		= AccountUpdate(cAccount, connection) ? Constants.INT_SUCCESS : _nResult;
					}
				}
				else
				{
					_nResult = Constants.INT_FAILURE;
				}
			}
			
			return new MsgAckAccountChangeMail { nResult = _nResult };
		}			
		
		// -------------------------------------------------------------------------------
		// ReqAccountForgotPassword
		// -------------------------------------------------------------------------------	
		public MsgAckAccountForgotPassword ReqAccountForgotPassword(MsgReqAccountForgotPassword message, NetworkConnection connection = null)
		{
			
			int _nResult			= confirmAccountForgotPassword ? Constants.INT_CONFIRM : Constants.INT_FAILURE;
			CAccount cAccount 		= null;
			
			if (!dictLobby.TryGetValue(connection, out cAccount))
			{
				
				cAccount = databaseManager.AccountLoad(message.sName);

				if (cAccount != null && !cAccount.IsEmpty)
				{

					if (confirmAccountForgotPassword && !cAccount.IsDone)
					{
						RequestSecurityCode(cAccount, Constants.AccountActionType.ForgotPassword);
					}
					else
					{
						int tmpPassword = UnityEngine.Random.Range(1000,9999);
						cAccount.sPassword = passwordManager.CreateHash(Tools.HashPassword(tmpPassword.ToString()));
						mailManager.SendMailForgotPassword(cAccount.sMail, tmpPassword);
						_nResult 			= AccountUpdate(cAccount, connection) ? Constants.INT_SUCCESS : _nResult;
					}
				
				}
				else
				{
					_nResult = Constants.INT_FAILURE;
				}
				
			}
			else
			{
				_nResult = Constants.INT_FAILURE;
			}
			

			return new MsgAckAccountForgotPassword { nResult = _nResult };
		}
		
		// -------------------------------------------------------------------------------
		// ReqActorPlayerCreate
		// -------------------------------------------------------------------------------	
		public MsgAckActorPlayerCreate ReqActorPlayerCreate(MsgReqActorPlayerCreate message, NetworkConnection connection = null)
		{
		
			bool 		_bSuccess 	= false;
			CAccount 	cAccount 	= null;
			
			if (dictLobby.TryGetValue(connection, out cAccount))
				_bSuccess = TryActorPlayerCreate(cAccount, message.sName, message.sAspects);
			
			return new MsgAckActorPlayerCreate { bSuccess = _bSuccess };
		
		}
		
		// -------------------------------------------------------------------------------
		// ReqActorPlayerDelete
		// -------------------------------------------------------------------------------	
		public MsgAckActorPlayerDelete ReqActorPlayerDelete(MsgReqActorPlayerDelete message, NetworkConnection connection = null)
		{
		
			bool 		_bSuccess 	= false;
			CAccount 	cAccount 	= null;
			
			if (dictLobby.TryGetValue(connection, out cAccount))
				_bSuccess = ActorPlayerDelete(message.sName, connection);
			
			return new MsgAckActorPlayerDelete { bSuccess = _bSuccess };
		
		}

		// -------------------------------------------------------------------------------
		// ReqActorPlayerList
		// -------------------------------------------------------------------------------	
		public MsgAckActorPlayerList ReqActorPlayerList(MsgReqActorPlayerList message, NetworkConnection connection = null)
		{
		
			bool 					_bSuccess 	= false;
			SActorPlayerPreview[] 	_previews 	= null;
			CAccount 				cAccount 	= null;
			
			if (dictLobby.TryGetValue(connection, out cAccount))
			{
				_previews = databaseManager.GetActorPlayerPreviews(cAccount.sName);
				_bSuccess = (_previews.Length > 0) ? true : false;
			}
			
			return new MsgAckActorPlayerList { bSuccess = _bSuccess, sActorPlayerPreviews = _previews };
		
		}
		
		// -------------------------------------------------------------------------------
		// ReqActorPlayerLogin
		// -------------------------------------------------------------------------------	
		public MsgAckActorPlayerLogin ReqActorPlayerLogin(MsgReqActorPlayerLogin message, NetworkConnection connection = null)
		{
		
			bool 		_bSuccess 	= false;
			
			/*if (dictLobby.TryGetValue(connection, out cAccount))
				_bSuccess = ActorPlayerLogin(message.sName, connection);*/
			
			return new MsgAckActorPlayerLogin { bSuccess = _bSuccess };
		
		}
						
		// -------------------------------------------------------------------------------
		
	}
	
}