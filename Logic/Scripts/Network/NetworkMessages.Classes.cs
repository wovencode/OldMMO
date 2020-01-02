// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Collections.Generic;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// MsgReqCheckVersion
	// ===================================================================================
	public partial class MsgReqCheckVersion : BaseNetworkMessage
	{
		public static short nId = 1000;
		public string sDeviceId;
		public string sVersion;
	}
	
	// ===================================================================================
	// MsgAckCheckVersion
	// ===================================================================================
	public partial class MsgAckCheckVersion : BaseNetworkMessage
	{
		public static short nId = 1001;
		public bool bSuccess;
		public int nAccountsRemaining;
		public bool bConfirmAccountCreate;
		public bool bConfirmAccountDelete;
		public bool bConfirmAccountChangeName;
		public bool bConfirmAccountChangePassword;
		public bool bConfirmAccountChangeMail;
		public bool bConfirmAccountForgotPassword;
	}
	
	// ===================================================================================
	// MsgReqAccountRegister
	// ===================================================================================
	public partial class MsgReqAccountRegister : BaseNetworkMessage
	{
		public static short nId = 1002;
		public string sName;
		public string sPassword;
		public string sMail;
		public string sDeviceId;
	}
	
	// ===================================================================================
	// MsgAckAccountRegister
	// ===================================================================================
	public partial class MsgAckAccountRegister : BaseNetworkMessage
	{
		public static short nId = 1003;
		public int nResult;
	}
	
	// ===================================================================================
	// MsgReqAccountLogin
	// ===================================================================================
	public partial class MsgReqAccountLogin : BaseNetworkMessage
	{
		public static short nId = 1004;
		public string sName;
		public string sPassword;
		public string sDeviceId;
	}
	
	// ===================================================================================
	// MsgAckAccountLogin
	// ===================================================================================
	public partial class MsgAckAccountLogin : BaseNetworkMessage
	{
		public static short nId = 1005;
		public int nResult;
		public int nActorsRemaining;
		public string sName;
		public string sMail;
		public bool bBanned;
		public bool bDeleted;
		public bool bConfirmed;
	}
	
	// ===================================================================================
	// MsgReqAccountLogout
	// ===================================================================================
	public partial class MsgReqAccountLogout : BaseNetworkMessage
	{
		public static short nId = 1006;
		public string sName;
	}
	
	// ===================================================================================
	// MsgAckAccountLogout
	// ===================================================================================
	public partial class MsgAckAccountLogout : BaseNetworkMessage
	{
		public static short nId = 1007;
		public bool bSuccess;
	}	
	
	// ===================================================================================
	// MsgReqConfirmCode
	// ===================================================================================
	public partial class MsgReqCodeConfirm : BaseNetworkMessage
	{
		public static short nId = 1008;
		public string sName;
		public int nCode;
		public int nAction;
	}
	
	// ===================================================================================
	// MsgAckConfirmCode
	// ===================================================================================
	public partial class MsgAckCodeConfirm : BaseNetworkMessage
	{
		public static short nId = 1009;
		public bool bSuccess;
	}

	// ===================================================================================
	// MsgReqAccountResendConfirmation
	// ===================================================================================
	public partial class MsgReqAccountResendConfirmation : BaseNetworkMessage
	{
		public static short nId = 1010;
		public string sName;
	}
	
	// ===================================================================================
	// MsgAckAccountResendConfirmation
	// ===================================================================================
	public partial class MsgAckAccountResendConfirmation : BaseNetworkMessage
	{
		public static short nId = 1011;
		public bool bSuccess;
	}

	// ===================================================================================
	// MsgReqAccountChangePassword
	// ===================================================================================
	public partial class MsgReqAccountChangePassword : BaseNetworkMessage
	{
		public static short nId = 1012;
		public string sOldPassword;
		public string sNewPassword;
	}
	
	// ===================================================================================
	// MsgAckAccountChangePassword
	// ===================================================================================
	public partial class MsgAckAccountChangePassword : BaseNetworkMessage
	{
		public static short nId = 1013;
		public int nResult;
	}
	
	// ===================================================================================
	// MsgReqAccountChangeMail
	// ===================================================================================
	public partial class MsgReqAccountChangeMail : BaseNetworkMessage
	{
		public static short nId = 1014;
		public string sMail;
		public string sPassword;
	}
	
	// ===================================================================================
	// MsgAckAccountChangeMail
	// ===================================================================================
	public partial class MsgAckAccountChangeMail : BaseNetworkMessage
	{
		public static short nId = 1015;
		public int nResult;
	}
	
	// ===================================================================================
	// MsgReqAccountChangeName
	// ===================================================================================
	public partial class MsgReqAccountChangeName : BaseNetworkMessage
	{
		public static short nId = 1016;
		public string sName;
		public string sPassword;
	}
	
	// ===================================================================================
	// MsgAckAccountChangeName
	// ===================================================================================
	public partial class MsgAckAccountChangeName : BaseNetworkMessage
	{
		public static short nId = 1017;
		public int nResult;
	}
	
	// ===================================================================================
	// MsgReqAccountDelete
	// ===================================================================================
	public partial class MsgReqAccountDelete : BaseNetworkMessage
	{
		public static short nId = 1018;
		public string sPassword;
	}
	
	// ===================================================================================
	// MsgAckAccountDelete
	// ===================================================================================
	public partial class MsgAckAccountDelete : BaseNetworkMessage
	{
		public static short nId = 1019;
		public int nResult;
	}
	
	// ===================================================================================
	// MsgReqAccountForgotPassword
	// ===================================================================================
	public partial class MsgReqAccountForgotPassword : BaseNetworkMessage
	{
		public static short nId = 1020;
		public string sName;
	}
	
	// ===================================================================================
	// MsgAckAccountForgotPassword
	// ===================================================================================
	public partial class MsgAckAccountForgotPassword : BaseNetworkMessage
	{
		public static short nId = 1021;
		public int nResult;
	}
	
	// =======================================================================================
	// ACTOR PLAYER METHODS
	// =======================================================================================
	
	// =======================================================================================
	// MsgReqActorPlayerCreate
	// =======================================================================================
	public partial class MsgReqActorPlayerCreate : BaseNetworkMessage
	{
		public static short nId = 2000;
		public string sName;
		public string[] sAspects;
	}	
	
	// =======================================================================================
	// MsgAckActorPlayerCreate
	// =======================================================================================
	public partial class MsgAckActorPlayerCreate : BaseNetworkMessage
	{
		public static short nId = 2001;
		public bool bSuccess;
	}		
	
	// =======================================================================================
	// MsgReqActorPlayerDelete
	// =======================================================================================
	public partial class MsgReqActorPlayerDelete : BaseNetworkMessage
	{
		public static short nId = 2002;
		public string sName;
	}	
	
	// =======================================================================================
	// MsgAckActorPlayerDelete
	// =======================================================================================
	public partial class MsgAckActorPlayerDelete : BaseNetworkMessage
	{
		public static short nId = 2003;
		public bool bSuccess;
	}
	
	// =======================================================================================
	// MsgReqActorPlayerList
	// =======================================================================================
	public partial class MsgReqActorPlayerList : BaseNetworkMessage
	{
		public static short nId = 2004;
	}	
	
	// =======================================================================================
	// MsgAckActorPlayerList
	// =======================================================================================
	public partial class MsgAckActorPlayerList : BaseNetworkMessage
	{
		public static short nId = 2005;
		public bool bSuccess;
		public SActorPlayerPreview[] sActorPlayerPreviews;
	}
	
	// =======================================================================================
	// MsgReqActorPlayerLogin
	// =======================================================================================
	public partial class MsgReqActorPlayerLogin : BaseNetworkMessage
	{
		public static short nId = 2006;
		public string sName;
	}	
	
	// =======================================================================================
	// MsgAckActorPlayerLogin
	// =======================================================================================
	public partial class MsgAckActorPlayerLogin : BaseNetworkMessage
	{
		public static short nId = 2007;
		public bool bSuccess;
	}
	
}