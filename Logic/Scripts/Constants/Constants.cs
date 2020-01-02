// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using UnityEngine;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// Constants
	// ===================================================================================
	public static partial class Constants
	{
		
		public enum AccountActionType { None, ConfirmAccount, ChangeMail, ForgotPassword, ChangePassword, ChangeName, DeleteAccount, LoginAccount }
		
		public const string STR_ERROR				= "An Error occured!";
		public const string STR_ERROR_MISSING_UI	= "Missing UI element: ";
		
		public const int INT_FAILURE 	= 0;
		public const int INT_CONFIRM 	= 1;
		public const int INT_SUCCESS 	= 2;
		public const int INT_RECONFIRM 	= 3;
		
	}
	
}