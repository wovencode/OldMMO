// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using Mono.Data.Sqlite; 				// copied from Unity/Mono/lib/mono/2.0 to Plugins
using System.Data.SqlClient;
using UnityEngine;
using System.Threading.Tasks;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// DatabaseManager
	/// <summary>
	/// This server-side class acts as intermediate between the actual database and the
	/// rest of the server managers. This class exists on the server only depending on
	/// the NetworkManager and not linked to any other Manager.
	/// </summary>
	// ===================================================================================
	[DisallowMultipleComponent]
	public partial class DatabaseManager : BaseServerBehaviour
	{
		
		public enum DatabaseType { SQLite, mySQL }
		
		public static string tablePrefix 		= "om_";
		
		public static string tableAccounts		= "accounts";
		public static string tableActorPlayers	= "actorPlayers";
		/*
			pets
			mounts
			etc.
		*/
		public static string tableAspects		= "aspects";
		public static string tableAttributes 	= "attributes";
		public static string tableAlignments	= "alignments";
		public static string tableCurrencies	= "currencies";
		public static string tableEnergies		= "energies";
		public static string tableEquipment		= "equipment";
		public static string tableInventory		= "inventory";
		public static string tableSkills		= "skills";
		public static string tableStatus		= "status";
		public static string tableProfessions	= "professions";
		
		public static string fieldName			= "sName";
		public static string fieldPassword		= "sPassword";
		public static string fieldEmail			= "sMail";
		public static string fieldDeviceId		= "sDeviceId";
		public static string fieldConfirmed		= "nConfirmed";
		public static string fieldCode			= "nCode";
		public static string fieldBanned		= "nBanned";
		public static string fieldDeleted		= "nDeleted";
		public static string fieldDone			= "nDone";
		public static string fieldAction		= "nAction";
		
		public static string fieldId			= "sId";
		public static string fieldValue			= "nValue";
		public static string fieldSlot			= "nSlot";
		public static string fieldAmount		= "nAmount";
		public static string fieldAmmo			= "nAmmo";
		public static string fieldCharges		= "nCharges";
		public static string fieldLevel			= "nLevel";
		public static string fieldDuration		= "fDuration";
		
		public DatabaseType databaseType;
		public float dbSaveInterval 			= 60f;
		public static int maxLengthName			= 16;
		public static int maxLengthId			= 32;
		public static int maxLengthPassword		= 255;
		public static int maxLengthEmail		= 32;
		public static int maxLengthDeviceId		= 32;
		
		[StringShowConditional(conditionFieldName: "databaseType", conditionValue: "SQLite")]
		public string dbFileName = "database";
		
		[StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
		public string 	dbHost = "localhost";
		[StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
		public string 	dbName = "OpenMMO";
		[StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
		public string 	dbUser = "OpenMMO";
		[StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
		public string 	dbPassword = "OpenMMO";
		[StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
		public uint 	dbPort = 3306;
		[StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
		public string 	dbCharacterSet = "utf8";
		
		protected IDatabaseProvider databaseProvider;
		protected BaseNetworkManager networkManager;
		
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
			
			if (databaseType == DatabaseType.SQLite) {
				databaseProvider = new SQLiteWrapper(this);
			}
			else
			{
				databaseProvider = new MySQLWrapper(this);
			}
			
			AutoCreateTables();
			
			InvokeRepeating("SaveActorPlayers", dbSaveInterval, dbSaveInterval);
		}
		
		// -------------------------------------------------------------------------------
		// Shutdown
		// -------------------------------------------------------------------------------
		public void Shutdown()
		{
			CancelInvoke("SaveActorPlayers");
			databaseProvider.Shutdown();
		}
		
		// -------------------------------------------------------------------------------
		// SaveActorPlayers
		// -------------------------------------------------------------------------------
		protected void SaveActorPlayers()
		{
			//databaseProvider.SaveActorPlayers();
		}
		
		// -------------------------------------------------------------------------------
		// AutoFindManagers
		// -------------------------------------------------------------------------------
       	protected void AutoFindManagers()
       	{
       		if (!networkManager) networkManager 	= FindObjectOfType<BaseNetworkManager>();
       		
       	}
		
		// ===============================================================================
    	// BASIC DATABASE ACCESS METHODS
    	// ===============================================================================
    	
		// -------------------------------------------------------------------------------
		// AutoCreateTables
		// -------------------------------------------------------------------------------
		public void AutoCreateTables()
		{
			databaseProvider.CreateTableAccounts();
			databaseProvider.CreateTableMeta();
			databaseProvider.CreateTableAspects();
			databaseProvider.CreateTableProfessions();
			databaseProvider.CreateTableAttributes();
			databaseProvider.CreateTableCurrencies();
			databaseProvider.CreateTableEnergies();
			databaseProvider.CreateTableEquipment();
			databaseProvider.CreateTableInventory();
			databaseProvider.CreateTableSkills();
			databaseProvider.CreateTableStatus();
			databaseProvider.CreateTableAlignments();
		}
		
		
		// ===============================================================================
    	// 
    	// ===============================================================================
		
		// -------------------------------------------------------------------------------
		// GetAccountsOnDevice
		// -------------------------------------------------------------------------------
		public int GetAccountsOnDevice(string sDeviceId)
		{
			return databaseProvider.RowCount(tableAccounts, fieldDeviceId, sDeviceId).Result;
		}
		
		// -------------------------------------------------------------------------------
		// GetActorsOnAccount
		// -------------------------------------------------------------------------------
		public int GetActorsOnAccount(string sName)
		{
			return databaseProvider.ActorCount(sName).Result;
		}
		
		// -------------------------------------------------------------------------------
		// GetActorNamesOnAccount
		// -------------------------------------------------------------------------------
		public List<string> GetActorNamesOnAccount(string sName)
		{

			BaseDataTable data;
			List<string> names = new List<string>();
			
			data = databaseProvider.SelectActors(sName).Result;
			
			for (int i = 0; i < data.Rows.Count; ++i)
				names.Add(data.GetString(fieldName, i));
			
			data.Cleanup();
			
			return names;
		}
		
		// -------------------------------------------------------------------------------
		// RowExists
		// -------------------------------------------------------------------------------
		public bool RowExists(string sTable, string sFieldName, string sFieldValue)
		{
			if ( String.IsNullOrWhiteSpace(sTable) || String.IsNullOrWhiteSpace(sFieldName) || String.IsNullOrWhiteSpace(sFieldValue) ) return false;
			return databaseProvider.RowExists(sTable, sFieldName, sFieldValue).Result;
		}

		// -------------------------------------------------------------------------------
		// DeleteRows
		// -------------------------------------------------------------------------------				
		public bool DeleteRows(string sTable, string sName)
		{
			if ( String.IsNullOrWhiteSpace(sTable) || String.IsNullOrWhiteSpace(sName) ) return false;
			return databaseProvider.DeleteRows(sTable, sName).Result;
		}
		
		// -------------------------------------------------------------------------------
		// UpdateRow
		// -------------------------------------------------------------------------------				
		public bool UpdateRow(string sTable, string sField, string sValue, string sTargetField, string sTargetValue)
		{
			return databaseProvider.UpdateRow(sTable, sField, sValue, sTargetField, sTargetValue).Result;
		}

		// ===============================================================================
    	// ACCOUNT RELATED
    	// ===============================================================================
	
		// -------------------------------------------------------------------------------
		// AccountLoad
		// -------------------------------------------------------------------------------
		public CAccount AccountLoad(string sName)
		{
			
			CAccount cAccount = null;
			
			if ( String.IsNullOrWhiteSpace(sName) ) return cAccount;
			
			if ( !String.IsNullOrWhiteSpace(sName) && 
				 databaseProvider.RowExists(tableAccounts, fieldName, sName).Result )
			{
				BaseDataTable data = databaseProvider.LoadAccount(sName).Result;
				
				if (data != null) {
					cAccount = new CAccount();
					cAccount.Load(data);
				}
			}
			
			return cAccount;
					
		}
		
		// -------------------------------------------------------------------------------
		// AccountCreate
		// -------------------------------------------------------------------------------
		public bool AccountCreate(CAccount cAccount)
		{
			return databaseProvider.SaveAccount(cAccount.Save()).Result;
		}
		
		// -------------------------------------------------------------------------------
		// AccountSave
		// -------------------------------------------------------------------------------		
		public bool AccountSave(CAccount cAccount)
		{
			return databaseProvider.SaveAccount(cAccount.Save()).Result;
		}
		
		// -------------------------------------------------------------------------------
		// AccountDelete
		// -------------------------------------------------------------------------------
		public bool AccountDelete(CAccount cAccount, bool hardDelete = false)
		{
			if (cAccount == null) return false;
			
			if (hardDelete)
			{
				// Delete the account immediately
				return databaseProvider.DeleteRows(DatabaseManager.tableAccounts, cAccount.sName).Result;
				
			}
			else
			{
				// Update the account and set 'deleted' to true
				return databaseProvider.SaveAccount(cAccount.Save()).Result;
			}
			
		}
		
		// -------------------------------------------------------------------------------
		// 
		// -------------------------------------------------------------------------------
		public bool AccountModify(string sName, BaseDataTable data, int nCode = 0)
		{
			return true;
		}
		
		// ===============================================================================
    	// ACTOR PLAYER RELATED
    	// ===============================================================================
		
		// -------------------------------------------------------------------------------
		// GetActorPlayerPreviews
		// -------------------------------------------------------------------------------		
		public SActorPlayerPreview[] GetActorPlayerPreviews(string sName)
		{
		
			if (String.IsNullOrWhiteSpace(sName)) return null;
			
			List<string> names = GetActorNamesOnAccount(sName);
			SActorPlayerPreview[] previews = new SActorPlayerPreview[names.Count];
			
			for (int i = 0; i < names.Count; ++i)
			{

				GameObject go = ActorPlayerLoad(names[i]);
				
				if (go.GetComponent<MetaSubsystem>().IsValid)
				{
					previews[i].sName = names[i];
					previews[i].sAspects = go.GetComponent<AspectSubsystem>().GetAspects();
				}
			
			}
			
			return previews;
		
		}
		
		// -------------------------------------------------------------------------------
		// ActorPlayerSave
		// -------------------------------------------------------------------------------
		public bool ActorPlayerSave(GameObject player)
		{
		
			if (player == null ||
				String.IsNullOrWhiteSpace(player.name)
				)
				return false;
			
			return databaseProvider.SaveActorPlayer(player).Result;
			
		}
		
		// -------------------------------------------------------------------------------
		// ActorPlayerLoad
		// -------------------------------------------------------------------------------
		public GameObject ActorPlayerLoad(string sName)
		{
		
			if (String.IsNullOrWhiteSpace(sName) ||
				!databaseProvider.RowExists(tableActorPlayers, fieldName, sName).Result
				)
				return null;
			
			
			GameObject actorObject = null;

			BaseDataTable data = databaseProvider.LoadAspects(sName).Result;
			
			// -- load the required prefab via the actors aspects
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				TemplateAspect tmpl;
				
				if (DataManager.dictAspect.TryGetValue(data.GetIdHash(i), out tmpl))
				{
					if (tmpl.actorPrefab)
					{
						actorObject = tmpl.actorPrefab;
						break;
					}
				}
				else
				{
					Debug.LogWarning("Skipped template '"+data.GetString(DatabaseManager.fieldName)+"' as it was not found in Library.");
				}
			
			}
			
			actorObject.GetComponent<MetaSubsystem>().Load(databaseProvider.LoadMeta(sName).Result);
			actorObject.GetComponent<AlignmentSubsystem>().Load(databaseProvider.LoadAlignments(sName).Result);
			actorObject.GetComponent<AspectSubsystem>().Load(databaseProvider.LoadAspects(sName).Result);
			actorObject.GetComponent<ProfessionSubsystem>().Load(databaseProvider.LoadProfessions(sName).Result);
			actorObject.GetComponent<AttributeSubsystem>().Load(databaseProvider.LoadAttributes(sName).Result);
			actorObject.GetComponent<CurrencySubsystem>().Load(databaseProvider.LoadCurrencies(sName).Result);
			actorObject.GetComponent<EnergySubsystem>().Load(databaseProvider.LoadEnergies(sName).Result);
			actorObject.GetComponent<EquipmentSubsystem>().Load(databaseProvider.LoadEquipment(sName).Result);
			actorObject.GetComponent<InventorySubsystem>().Load(databaseProvider.LoadInventory(sName).Result);
			actorObject.GetComponent<SkillSubsystem>().Load(databaseProvider.LoadSkills(sName).Result);
			actorObject.GetComponent<StatusSubsystem>().Load(databaseProvider.LoadStatus(sName).Result);
			
			return actorObject;
			
		}
		
		// -------------------------------------------------------------------------------
		// ActorPlayerDelete
		// -------------------------------------------------------------------------------
		public bool ActorPlayerDelete(string sName, bool hardDelete = false)
		{
		
			if (String.IsNullOrWhiteSpace(sName) ||
				!databaseProvider.RowExists(tableActorPlayers, fieldName, sName).Result
				)
				return false;
				
			if (hardDelete)
			{
				// Delete the player immediately
				return DeleteRows(DatabaseManager.tableActorPlayers, sName);
			}
			else
			{
				// Update the player and set 'deleted' to true
				return databaseProvider.UpdateRow(tableActorPlayers, fieldName, sName, fieldDeleted, 1).Result;
			}
			
		}
		
		// -------------------------------------------------------------------------------
		
		
	}
	
}