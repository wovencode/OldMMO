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
	// CAccount
	/// <summary>
	/// This class holds all account specific data and is used on the server-side as well
	/// as on the client-side. On the server, there is a dictionary that holds all accounts
	/// currently in the lobby. While on the client it is only used to hold the current
	/// account of the user.
	/// </summary>
	// ===================================================================================
	[Serializable]
	public partial class CAccount
	{
		
		public NetworkConnection networkConnection;
		
		public string 	sName;					// Account Name
		public string 	sPassword;				// Account Password
		public string 	sMail;					// Account eMail Address
		public string 	sDeviceId;				// Account Device ID
		public bool 	bBanned;				// Account banned?
		public bool 	bDeleted;				// Account (soft) deleted?
		public bool 	bConfirmed;				// Account confirmed?
		public bool		bDone;					// Security code entered & correct?
		public int 		nCode;					// Temporary security code
		public List<string> sActorPlayers;		// Players on that account (by name)
		
		protected int 	_nAction;				// Current account related action
		
		// -------------------------------------------------------------------------------
		// CAccount (Constructor)
		// -------------------------------------------------------------------------------
		public CAccount()
		{
			sName			= "";
			sPassword		= "";
			sMail			= "";
			sDeviceId		= "";
			bBanned			= false;
			bDeleted		= false;
			bConfirmed		= false;
			bDone			= false;
			nCode			= 0;
			sActorPlayers	= new List<string>();
			
			_nAction	= 0;
		}
		
		// -------------------------------------------------------------------------------
		// ValidatePassword
		// -------------------------------------------------------------------------------
		public bool ValidatePassword(string _sPassword)
		{
			return _sPassword == sPassword;
		}
		
		// -------------------------------------------------------------------------------
		// ValidateCode
		// -------------------------------------------------------------------------------
		public bool ValidateCode(int _nCode)
		{
			return _nCode == nCode;
		}
		
		// -------------------------------------------------------------------------------
		// ValidateAction
		// -------------------------------------------------------------------------------
		public bool ValidateAction(Constants.AccountActionType _Action)
		{
			return Action == _Action;
		}
		
		// -------------------------------------------------------------------------------
		// ValidateAll
		// -------------------------------------------------------------------------------
		public bool ValidateAll(Constants.AccountActionType Action, int _nCode)
		{
			return ValidateCode(_nCode) && ValidateAction(Action);
		}
		
		// -------------------------------------------------------------------------------
		// Action
		// -------------------------------------------------------------------------------
		public Constants.AccountActionType Action
		{
			get
			{
				return (Constants.AccountActionType)_nAction;
			}
			
			set
			{
				_nAction = (int)value;
			}
		}
		
		// -------------------------------------------------------------------------------
		// IsEmpty
		// -------------------------------------------------------------------------------
		public bool IsEmpty
		{
			get
			{
				return String.IsNullOrWhiteSpace(sName) && String.IsNullOrWhiteSpace(sPassword);
			}
		}
		
		// -------------------------------------------------------------------------------
		// IsDone
		// -------------------------------------------------------------------------------
		public bool IsDone
		{
			get
			{
				return bDone;
			}
		}
		
		// -------------------------------------------------------------------------------
		// IsValid
		// -------------------------------------------------------------------------------
		public bool IsValid
		{
			get
			{
				return !IsEmpty && !bBanned && !bDeleted;
			}
		}
		
		// -------------------------------------------------------------------------------
		// ConfirmCode
		// -------------------------------------------------------------------------------
		public void ConfirmCode()
		{
			
			if (Action == Constants.AccountActionType.ConfirmAccount)
			{
				bConfirmed = true;
			}
			else if (Action == Constants.AccountActionType.DeleteAccount)
			{
				bDeleted = true;
			}
			
			// -- confirm does not set done, all other actions do
			if (Action == Constants.AccountActionType.ConfirmAccount)
				ResetCode(false);
			else 
				ResetCode(true);
			
		}
		
		// -------------------------------------------------------------------------------
		// GenerateCode
		// -------------------------------------------------------------------------------
		public int GenerateCode(Constants.AccountActionType accountActionType)
		{
			ResetCode();
			nCode = UnityEngine.Random.Range(1000,9999);
			Action = accountActionType;
			return nCode;
		}
		
		// -------------------------------------------------------------------------------
		// ResetCode
		// -------------------------------------------------------------------------------
		public void ResetCode(bool _bDone = false)
		{
			nCode 		= 0;
			_nAction 	= 0;
			bDone		= _bDone;
		}
		
		// -------------------------------------------------------------------------------
		// Save
		// -------------------------------------------------------------------------------
		public BaseDataTable Save()
		{
			
			BaseDataTable data = new BaseDataTable();
			
			data.AddString(DatabaseManager.fieldName, sName);
			data.AddString(DatabaseManager.fieldPassword, sPassword);
			data.AddString(DatabaseManager.fieldEmail, sMail);
			data.AddInt(DatabaseManager.fieldAction, _nAction);
			data.AddString(DatabaseManager.fieldDeviceId, sDeviceId);
			data.AddBool(DatabaseManager.fieldBanned, bBanned);
			data.AddBool(DatabaseManager.fieldDeleted, bDeleted);
			data.AddBool(DatabaseManager.fieldConfirmed, bConfirmed);
			data.AddBool(DatabaseManager.fieldDone, bDone);
			data.AddInt(DatabaseManager.fieldCode, nCode);

			return data;
		
		}
		
		// -------------------------------------------------------------------------------
		// Load
		// -------------------------------------------------------------------------------
		public void Load(BaseDataTable data)
		{
			sName			= data.GetString(DatabaseManager.fieldName);
			sPassword		= data.GetString(DatabaseManager.fieldPassword);
			sMail			= data.GetString(DatabaseManager.fieldEmail);
			_nAction		= data.GetLongAsInt(DatabaseManager.fieldAction);
			sDeviceId		= data.GetString(DatabaseManager.fieldDeviceId);
			bBanned			= data.GetBoolFromInt(DatabaseManager.fieldBanned);
			bDeleted		= data.GetBoolFromInt(DatabaseManager.fieldDeleted);
			bConfirmed		= data.GetBoolFromInt(DatabaseManager.fieldConfirmed);
			bDone			= data.GetBoolFromInt(DatabaseManager.fieldDone);
			nCode			= data.GetLongAsInt(DatabaseManager.fieldCode);
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}