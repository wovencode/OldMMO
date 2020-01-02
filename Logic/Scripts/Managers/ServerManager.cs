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
	// ServerManager
	/// <summary>
	/// This server-side class acts as intermediate between the NetworkManager and all
	/// other server-sided managers (most notably the AccountManager). 
	/// </summary>
	// ===================================================================================
	[DisallowMultipleComponent]
	public partial class ServerManager : BaseServerBehaviour
	{
		
		[Header("---------- [Optional] Linked Managers ----------")]
		public BaseNetworkManager networkManager;
		public AccountManager accountManager;
		
		// -------------------------------------------------------------------------------
		// Start
		// -------------------------------------------------------------------------------
		void Start()
		{
			AutoFindManagers();
		}
		
		// -------------------------------------------------------------------------------
		// AutoFindManagers
		// -------------------------------------------------------------------------------
       	protected void AutoFindManagers()
       	{
       		if (!networkManager) networkManager 	= FindObjectOfType<BaseNetworkManager>();
       		if (!accountManager) accountManager 	= FindObjectOfType<AccountManager>();
       	}
		
		// ===============================================================================
		// 
		// ===============================================================================
		
		
		
		
		
		
		// -------------------------------------------------------------------------------	
		
	}
	
}