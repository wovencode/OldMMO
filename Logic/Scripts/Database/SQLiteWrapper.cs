// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Mono.Data.Sqlite; 				// copied from Unity/Mono/lib/mono/2.0 to Plugins
using UnityEngine;
using System.Threading.Tasks;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// SQLiteWrapper
	// ===================================================================================
	public partial class SQLiteWrapper : IDatabaseProvider
	{
	
		protected DatabaseManager databaseManager;
		protected SqliteConnection sqliteConnection;
		protected string sDatabasePath;
		
		protected const string sDatabaseSuffix = ".sqlite";
		
		// -------------------------------------------------------------------------------
		// SQLiteWrapper (Constructor)
		// -------------------------------------------------------------------------------
		public SQLiteWrapper(DatabaseManager _databaseManager)
		{
			databaseManager = _databaseManager;
			AutoSetupDatabase();
		}
		
		// -------------------------------------------------------------------------------
		// SQLiteWrapper (Destructor)
		// -------------------------------------------------------------------------------
		~SQLiteWrapper()
		{
			Shutdown();
		}
		
		// -------------------------------------------------------------------------------
		// Shutdown
		// -------------------------------------------------------------------------------
		public void Shutdown()
		{
			sqliteConnection.Close();
		}
		
		// -------------------------------------------------------------------------------
		// AutoSetupDatabase
		// -------------------------------------------------------------------------------
		public void AutoSetupDatabase()
		{
		
#if UNITY_EDITOR
    		sDatabasePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, databaseManager.dbFileName+sDatabaseSuffix);
#elif UNITY_ANDROID
    		sDatabasePath = Path.Combine(Application.persistentDataPath, databaseManager.dbFileName+sDatabaseSuffix);
#elif UNITY_IOS
    		sDatabasePath = Path.Combine(Application.persistentDataPath, databaseManager.dbFileName+sDatabaseSuffix);
#else
    		sDatabasePath = Path.Combine(Application.dataPath, databaseManager.dbFileName+sDatabaseSuffix);
#endif

			if(!File.Exists(sDatabasePath))
            	SqliteConnection.CreateFile(sDatabasePath);
            
            sqliteConnection = new SqliteConnection("URI=file:" + sDatabasePath);
        	sqliteConnection.Open();
        
		}
		
		// -------------------------------------------------------------------------------
		// ExecuteScalar
		// -------------------------------------------------------------------------------
		public async Task<object> ExecuteScalar(string sql, params SqliteParameter[] args)
		{
			
			try
			{
	
				using (SqliteCommand command = new SqliteCommand(sql, sqliteConnection))
				{
	
					foreach (SqliteParameter param in args)
						command.Parameters.Add(param);
					
					object result = await command.ExecuteScalarAsync();
					return result;
				}
			
			}
			catch (Exception exc)
			{
				Debug.Log(exc.Message);
				return null;
			}
			
		}
		
		// -------------------------------------------------------------------------------
		// ExecuteReader
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> ExecuteReader(string sql, params SqliteParameter[] args)
		{
		
			BaseDataTable data = new BaseDataTable();
			
			try
			{
				
				SqliteCommand command = new SqliteCommand(sql, sqliteConnection);
				
				foreach (SqliteParameter param in args)
					command.Parameters.Add(param);
				
				SqliteDataReader reader = await command.ExecuteReaderAsync() as SqliteDataReader;
				
                data.Load(reader);
                reader.Close();
                
				return data;
			}
			catch (Exception exc)
			{
				Debug.Log(exc.Message);
				return data;
			}
			
		}
				
		// -------------------------------------------------------------------------------
		// ExecuteNonQuery
		// -------------------------------------------------------------------------------
		public async Task<bool> ExecuteNonQuery(string sql, params SqliteParameter[] args)
		{
			try
			{
				SqliteCommand command = new SqliteCommand(sql, sqliteConnection);
				
				foreach (SqliteParameter param in args)
					command.Parameters.Add(param);
				
				int result = await command.ExecuteNonQueryAsync();
				
				return (result != 0) ? true : false;
			}
			catch (Exception exc)
			{
				Debug.Log(exc.Message);
				return false;
			}
		}
		
		// ===============================================================================
		// VARIOUS METHODS
		// ===============================================================================
		
		
		// -------------------------------------------------------------------------------
		// RowCount
		// We always compare Names with lowercase to prevent dupes like: Fhiz, fhiz, FHIZ
		// -------------------------------------------------------------------------------
		public async Task<int> RowCount(string sTable, string sField, string sName)
		{
			string sql = "SELECT Count(*) FROM {0}{1} WHERE LOWER(`{2}`) = @{2}";
			sql = string.Format(sql, DatabaseManager.tablePrefix, sTable, sField);
			var result = await ExecuteScalar(sql, new SqliteParameter("@"+sField, sName.ToLower()));
			return Convert.ToInt32((long)result);
		}
		
		
		// -------------------------------------------------------------------------------
		// RowExists
		// We always compare Names with lowercase to prevent dupes like: Fhiz, fhiz, FHIZ
		// -------------------------------------------------------------------------------
		public async Task<bool> RowExists(string sTable, string sFieldName, string sFieldValue)
		{
			string sql = "SELECT Count(*) FROM {0}{1} WHERE LOWER(`{2}`) = @{2}";
			sql = string.Format(sql, DatabaseManager.tablePrefix, sTable, sFieldName);
			var result = await ExecuteScalar(sql, new SqliteParameter("@"+sFieldName, sFieldValue.ToLower()));
			return ((long)result) == 1;
		}
		
		
		// -------------------------------------------------------------------------------
		// UpdateRow
		// -------------------------------------------------------------------------------
		public async Task<bool> UpdateRow(string sTable, string sField, string sValue, string sTargetField, string sTargetValue)
		{
			string sql = "UPDATE {0}{1} SET {2}={3} WHERE {4} = @{4}";
			sql = string.Format(sql, 
				DatabaseManager.tablePrefix,
				sTable,
				sTargetField,
				sTargetValue,
				sField
			);
			return await ExecuteNonQuery(sql, new SqliteParameter("@"+sField, sValue.ToLower()));
		}


		// -------------------------------------------------------------------------------
		// UpdateRow
		// -------------------------------------------------------------------------------
		public async Task<bool> UpdateRow(string sTable, string sField, string sValue, string sTargetField, int nTargetValue)
		{
			string sql = "UPDATE {0}{1} SET {2}={3} WHERE {4} = @{4}";
			sql = string.Format(sql, 
				DatabaseManager.tablePrefix,
				sTable,
				sTargetField,
				nTargetValue,
				sField
			);
			return await ExecuteNonQuery(sql, new SqliteParameter("@"+sField, sValue.ToLower()));
		}
		
		
		// -------------------------------------------------------------------------------
		// DeleteRows
		// -------------------------------------------------------------------------------
		public async Task<bool> DeleteRows(string sTable, string sName)
		{
			string sql = "DELETE FROM {0}{1} WHERE {2} = @{2}";
			sql = string.Format(sql, DatabaseManager.tablePrefix, sTable, DatabaseManager.fieldName);
			return await ExecuteNonQuery(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName.ToLower()));
		}
		
		// -------------------------------------------------------------------------------
		// SelectRows
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> SelectRows(string sTargetField, string sTable, string sField, string sName)
		{
			string sql = "SELECT {0} FROM {1}{2} WHERE {3} = @{3}";
			sql = string.Format(sql, sTargetField, DatabaseManager.tablePrefix, sTable, sField);
			return await ExecuteReader(sql, new SqliteParameter("@"+sField, sName.ToLower()));
		}
		
		// ===============================================================================
		// ACTOR RELATED METHODS
		// ===============================================================================
		
		// -------------------------------------------------------------------------------
		// ActorCount
		// We always compare Names with lowercase to prevent dupes like: Fhiz, fhiz, FHIZ
		// -------------------------------------------------------------------------------
		public async Task<int> ActorCount(string sName)
		{
			string sql = "SELECT Count(*) FROM {0}{1} WHERE LOWER(`{2}`) = @{2} AND {3} != 1 AND {4} != 1";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableActorPlayers, DatabaseManager.fieldId, DatabaseManager.fieldBanned, DatabaseManager.fieldDeleted);
			var result = await ExecuteScalar(sql, new SqliteParameter("@"+DatabaseManager.fieldId, sName.ToLower()));
			return Convert.ToInt32((long)result);
		}
		
		// -------------------------------------------------------------------------------
		// SelectActors
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> SelectActors(string sName)
		{
			string sql = "SELECT {0} FROM {1}{2} WHERE {3} = @{3} AND {4} != 1 AND {5} != 1";
			sql = string.Format(sql, DatabaseManager.fieldName, DatabaseManager.tablePrefix, DatabaseManager.tableActorPlayers, DatabaseManager.fieldId, DatabaseManager.fieldBanned, DatabaseManager.fieldDeleted);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldId, sName.ToLower()));
		}
		
		// ===============================================================================
		// CREATE TABLES
		// ===============================================================================
				
		// -------------------------------------------------------------------------------
		// CreateTableAccounts
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableAccounts()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL PRIMARY KEY, {3} TEXT NOT NULL, {4} TEXT NOT NULL, {5} INTEGER NOT NULL, {6} TEXT NOT NULL, {7} INTEGER NOT NULL, {8} INTEGER NOT NULL, {9} INTEGER NOT NULL, {10} INTEGER NOT NULL, {11} INTEGER NOT NULL)";
			sql = string.Format(sql, 
				DatabaseManager.tablePrefix, 
				DatabaseManager.tableAccounts, 
				DatabaseManager.fieldName, 
				DatabaseManager.fieldPassword, 
				DatabaseManager.fieldEmail, 
				DatabaseManager.fieldAction, 
				DatabaseManager.fieldDeviceId, 
				DatabaseManager.fieldBanned, 
				DatabaseManager.fieldDeleted, 
				DatabaseManager.fieldConfirmed, 
				DatabaseManager.fieldDone, 
				DatabaseManager.fieldCode);
			return await ExecuteNonQuery(sql);
		}

		// -------------------------------------------------------------------------------
		// CreateTableMeta
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableMeta()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL PRIMARY KEY, {3} TEXT NOT NULL, {4} INTEGER NOT NULL, {5} INTEGER NOT NULL)";
			sql = string.Format(sql, 
			DatabaseManager.tablePrefix, 
			DatabaseManager.tableActorPlayers, 
			DatabaseManager.fieldName,
			DatabaseManager.fieldId,
			DatabaseManager.fieldBanned, 
			DatabaseManager.fieldDeleted
			);
			return await ExecuteNonQuery(sql);
		}

		// -------------------------------------------------------------------------------
		// CreateTableAlignments
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableAlignments()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL, {3} TEXT NOT NULL, {4} INTEGER NOT NULL, PRIMARY KEY({2}, {3}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableAlignments, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}

		// -------------------------------------------------------------------------------
		// CreateTableAspects
		// -------------------------------------------------------------------------------
		public async Task <bool> CreateTableAspects()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL, {3} TEXT NOT NULL, {4} INTEGER NOT NULL, PRIMARY KEY({2}, {3}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableAspects, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}

		// -------------------------------------------------------------------------------
		// CreateTableProfessions
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableProfessions()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL, {3} TEXT NOT NULL, {4} INTEGER NOT NULL, PRIMARY KEY({2}, {3}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableProfessions, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}

		// -------------------------------------------------------------------------------
		// CreateTableAttributes
		// -------------------------------------------------------------------------------
		public async Task <bool> CreateTableAttributes()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL, {3} TEXT NOT NULL, {4} INTEGER NOT NULL, PRIMARY KEY({2}, {3}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableAttributes, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}

		// -------------------------------------------------------------------------------
		// CreateTableCurrencies
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableCurrencies()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL, {3} TEXT NOT NULL, {4} INTEGER NOT NULL, PRIMARY KEY({2}, {3}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableCurrencies, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}

		// -------------------------------------------------------------------------------
		// CreateTableEnergies
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableEnergies()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL, {3} TEXT NOT NULL, {4} INTEGER NOT NULL, PRIMARY KEY({2}, {3}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableEnergies, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}

		// -------------------------------------------------------------------------------
		// CreateTableEquipment
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableEquipment()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL, {3} TEXT NOT NULL, {4} INTEGER NOT NULL, {5} INTEGER NOT NULL, {6} INTEGER NOT NULL, {7} INTEGER NOT NULL, {8} INTEGER NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableEquipment, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldSlot, DatabaseManager.fieldAmount, DatabaseManager.fieldAmmo, DatabaseManager.fieldCharges, DatabaseManager.fieldLevel);
			return await ExecuteNonQuery(sql);
		}

		// -------------------------------------------------------------------------------
		// CreateTableInventory
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableInventory()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL, {3} TEXT NOT NULL, {4} INTEGER NOT NULL, {5} INTEGER NOT NULL, {6} INTEGER NOT NULL, {7} INTEGER NOT NULL, {8} INTEGER NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableInventory, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldSlot, DatabaseManager.fieldAmount, DatabaseManager.fieldAmmo, DatabaseManager.fieldCharges, DatabaseManager.fieldLevel);
			return await ExecuteNonQuery(sql);
		}
		
		// -------------------------------------------------------------------------------
		// CreateTableSkills
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableSkills()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL, {3} TEXT NOT NULL, {4} INTEGER NOT NULL, {5} INTEGER NOT NULL, PRIMARY KEY({2}, {3}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableSkills, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldLevel, DatabaseManager.fieldDuration);
			return await ExecuteNonQuery(sql);
		}
		
		// -------------------------------------------------------------------------------
		// CreateTableStatus
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableStatus()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} TEXT NOT NULL, {3} TEXT NOT NULL, {4} INTEGER NOT NULL, {5} INTEGER NOT NULL, PRIMARY KEY({2}, {3}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableStatus, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldLevel, DatabaseManager.fieldDuration);
			return await ExecuteNonQuery(sql);
		}

		// ===============================================================================
		// LOAD TABLES
		// ===============================================================================

		// -------------------------------------------------------------------------------
		// LoadActorPlayer
		// -------------------------------------------------------------------------------
		/*public GameObject> LoadActorPlayer(string sName)
		{
		
			
			
			return actorObject;
		}*/
		
		
		
		// -------------------------------------------------------------------------------
		// LoadAccount
		// We can load any account either via the account name or via its email address (both unique)
		// We always compare Names with lowercase to prevent dupes like: Fhiz, fhiz, FHIZ
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadAccount(string sName)
		{
			string sql = "";

			sql = "SELECT {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9} FROM {10}{11} WHERE LOWER(`{12}`) = @{12}";
			sql = string.Format(sql, 
			DatabaseManager.fieldName, 
			DatabaseManager.fieldPassword, 
			DatabaseManager.fieldEmail, 
			DatabaseManager.fieldDeviceId, 
			DatabaseManager.fieldAction, 
			DatabaseManager.fieldBanned, 
			DatabaseManager.fieldDeleted, 
			DatabaseManager.fieldConfirmed,
			DatabaseManager.fieldCode, 
			DatabaseManager.fieldDone, 
			DatabaseManager.tablePrefix, 
			DatabaseManager.tableAccounts, 
			DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName.ToLower()));
		}

		// -------------------------------------------------------------------------------
		// LoadMeta
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadMeta(string sName)
		{
			string sql = "SELECT {0}, {1}, {2}, {3} FROM {4}{5} WHERE {6} = @{6}";
			sql = string.Format(sql,
			DatabaseManager.fieldName,
			DatabaseManager.fieldId,
			DatabaseManager.fieldBanned,
			DatabaseManager.fieldDeleted,
			DatabaseManager.tablePrefix,
			DatabaseManager.tableActorPlayers,
			DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName));
		}

		// -------------------------------------------------------------------------------
		// LoadAlignments
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadAlignments(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableAlignments, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadAspects
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadAspects(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableAspects, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadProfessions
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadProfessions(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableProfessions, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadAttributes
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadAttributes(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableAttributes, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadCurrencies
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadCurrencies(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableCurrencies, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadEnergies
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadEnergies(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableEnergies, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadEquipment
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadEquipment(string sName)
		{
			string sql = "SELECT {0}, {1}, {2}, {3}, {4}, {5} FROM {6}{7} WHERE {8} = @{8}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldSlot, DatabaseManager.fieldAmount, DatabaseManager.fieldAmmo, DatabaseManager.fieldCharges, DatabaseManager.fieldLevel, DatabaseManager.tablePrefix, DatabaseManager.tableEquipment, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadInventory
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadInventory(string sName)
		{
			string sql = "SELECT {0}, {1}, {2}, {3}, {4}, {5} FROM {6}{7} WHERE {8} = @{8}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldSlot, DatabaseManager.fieldAmount, DatabaseManager.fieldAmmo, DatabaseManager.fieldCharges, DatabaseManager.fieldLevel, DatabaseManager.tablePrefix, DatabaseManager.tableInventory, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadSkills
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadSkills(string sName)
		{
			string sql = "SELECT {0}, {1}, {2} FROM {3}{4} WHERE {5} = @{5}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldLevel, DatabaseManager.fieldDuration, DatabaseManager.tablePrefix, DatabaseManager.tableSkills, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadStatus
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadStatus(string sName)
		{
			string sql = "SELECT {0}, {1}, {2} FROM {3}{4} WHERE {5} = @{5}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldLevel, DatabaseManager.fieldDuration, DatabaseManager.tablePrefix, DatabaseManager.tableStatus, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqliteParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// ===============================================================================
		// SAVE TABLES
		// ===============================================================================
		
		// -------------------------------------------------------------------------------
		// SaveActors
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveActors(BaseDataTable data)
		{
			return true;
		}
		
		// -------------------------------------------------------------------------------
		// SaveActorPlayer
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveActorPlayer(GameObject player)
		{
			
			SaveMeta(player.GetComponent<MetaSubsystem>().Save());
			SaveAlignments(player.GetComponent<AlignmentSubsystem>().Save());
			SaveAspects(player.GetComponent<AspectSubsystem>().Save());
			SaveProfessions(player.GetComponent<ProfessionSubsystem>().Save());
			SaveAttributes(player.GetComponent<AttributeSubsystem>().Save());
			SaveCurrencies(player.GetComponent<CurrencySubsystem>().Save());
			SaveEnergies(player.GetComponent<EnergySubsystem>().Save());
			SaveEquipment(player.GetComponent<EquipmentSubsystem>().Save());
			SaveInventory(player.GetComponent<InventorySubsystem>().Save());
			SaveSkills(player.GetComponent<SkillSubsystem>().Save());
			SaveStatus(player.GetComponent<StatusSubsystem>().Save());
			
			return true;
		}
		
		// -------------------------------------------------------------------------------
		// SaveMeta
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveMeta(BaseDataTable data)
		{
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableActorPlayers, data.GetString(DatabaseManager.fieldName));
						
			string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4}, @{5})";
			sql = string.Format(sql,
			DatabaseManager.tablePrefix,
			DatabaseManager.tableActorPlayers,
			DatabaseManager.fieldName,
			DatabaseManager.fieldId,
			DatabaseManager.fieldBanned, 
			DatabaseManager.fieldDeleted
			);

			await ExecuteNonQuery(sql, 
								new SqliteParameter("@"+DatabaseManager.fieldName, 			data.GetString(DatabaseManager.fieldName)),
								new SqliteParameter("@"+DatabaseManager.fieldId, 			data.GetString(DatabaseManager.fieldId)),
								new SqliteParameter("@"+DatabaseManager.fieldBanned, 		data.GetBoolAsInt(DatabaseManager.fieldBanned)),
								new SqliteParameter("@"+DatabaseManager.fieldDeleted, 		data.GetBoolAsInt(DatabaseManager.fieldDeleted))
								);
			data.Cleanup();
			
			return true;
		}
		
		// -------------------------------------------------------------------------------
		// SaveAccount
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveAccount(BaseDataTable data)
		{
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableAccounts, data.GetString(DatabaseManager.fieldName));
			
			string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4}, @{5}, @{6}, @{7}, @{8}, @{9}, @{10}, @{11})";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableAccounts, DatabaseManager.fieldName, DatabaseManager.fieldPassword, DatabaseManager.fieldEmail, DatabaseManager.fieldAction, DatabaseManager.fieldDeviceId, DatabaseManager.fieldBanned, DatabaseManager.fieldDeleted, DatabaseManager.fieldConfirmed, DatabaseManager.fieldDone, DatabaseManager.fieldCode);
			await ExecuteNonQuery(sql, 
								new SqliteParameter("@"+DatabaseManager.fieldName, 			data.GetString(DatabaseManager.fieldName)),
								new SqliteParameter("@"+DatabaseManager.fieldPassword, 		data.GetString(DatabaseManager.fieldPassword)),
								new SqliteParameter("@"+DatabaseManager.fieldEmail, 		data.GetString(DatabaseManager.fieldEmail)),
								new SqliteParameter("@"+DatabaseManager.fieldAction, 		data.GetInt(DatabaseManager.fieldAction)),
								new SqliteParameter("@"+DatabaseManager.fieldDeviceId, 		data.GetString(DatabaseManager.fieldDeviceId)),
								new SqliteParameter("@"+DatabaseManager.fieldBanned, 		data.GetBoolAsInt(DatabaseManager.fieldBanned)),
								new SqliteParameter("@"+DatabaseManager.fieldDeleted, 		data.GetBoolAsInt(DatabaseManager.fieldDeleted)),
								new SqliteParameter("@"+DatabaseManager.fieldConfirmed, 	data.GetBoolAsInt(DatabaseManager.fieldConfirmed)),
								new SqliteParameter("@"+DatabaseManager.fieldDone,		 	data.GetBoolAsInt(DatabaseManager.fieldDone)),
								new SqliteParameter("@"+DatabaseManager.fieldCode, 			data.GetInt(DatabaseManager.fieldCode))
								);
			
			data.Cleanup();
			
			return true;
		}
		
		// -------------------------------------------------------------------------------
		// SaveAlignments
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveAlignments(BaseDataTable data)
		{
		
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableAlignments, data.GetString(DatabaseManager.fieldName));
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4})";
				sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableAlignments, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
				await ExecuteNonQuery(sql, 
									new SqliteParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqliteParameter("@"+DatabaseManager.fieldId, 	data.GetString(DatabaseManager.fieldId, i)),
									new SqliteParameter("@"+DatabaseManager.fieldValue, data.GetInt(DatabaseManager.fieldValue, i))
									);
			}
			
			data.Cleanup();
			
			return true;
		}
		
		// -------------------------------------------------------------------------------
		// SaveAspects
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveAspects(BaseDataTable data)
		{
		
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableAspects, data.GetString(DatabaseManager.fieldName));
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{

				string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4})";
				sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableAspects, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
				await ExecuteNonQuery(sql, 
									new SqliteParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqliteParameter("@"+DatabaseManager.fieldId, 	data.GetString(DatabaseManager.fieldId, i)),
									new SqliteParameter("@"+DatabaseManager.fieldValue, data.GetInt(DatabaseManager.fieldValue, i))
									);
			}
			
			data.Cleanup();
			
			return true;
		}
		
		// -------------------------------------------------------------------------------
		// SaveProfessions
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveProfessions(BaseDataTable data)
		{
		
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableProfessions, data.GetString(DatabaseManager.fieldName));
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4})";
				sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableProfessions, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
				await ExecuteNonQuery(sql, 
									new SqliteParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqliteParameter("@"+DatabaseManager.fieldId, 	data.GetString(DatabaseManager.fieldId, i)),
									new SqliteParameter("@"+DatabaseManager.fieldValue, data.GetInt(DatabaseManager.fieldValue, i))
									);
			}
			
			data.Cleanup();
			
			return true;
		}
		
		// -------------------------------------------------------------------------------
		// SaveAttributes
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveAttributes(BaseDataTable data)
		{
		
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableAttributes, data.GetString(DatabaseManager.fieldName));
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4})";
				sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableAttributes, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
				await ExecuteNonQuery(sql, 
									new SqliteParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqliteParameter("@"+DatabaseManager.fieldId, 	data.GetString(DatabaseManager.fieldId, i)),
									new SqliteParameter("@"+DatabaseManager.fieldValue, data.GetInt(DatabaseManager.fieldValue, i))
									);
			}
			
			data.Cleanup();
			
			return true;
		}
		
		// -------------------------------------------------------------------------------
		// SaveCurrencies
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveCurrencies(BaseDataTable data)
		{
		
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableCurrencies, data.GetString(DatabaseManager.fieldName));
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4})";
				sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableCurrencies, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
				await ExecuteNonQuery(sql, 
									new SqliteParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqliteParameter("@"+DatabaseManager.fieldId, 	data.GetString(DatabaseManager.fieldId, i)),
									new SqliteParameter("@"+DatabaseManager.fieldValue, data.GetInt(DatabaseManager.fieldValue, i))
									);
			}
			
			data.Cleanup();
			
			return true;
			
		}
		
		// -------------------------------------------------------------------------------
		// SaveEnergies
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveEnergies(BaseDataTable data)
		{
		
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableEnergies, data.GetString(DatabaseManager.fieldName));
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4})";
				sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableEnergies, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue);
				await ExecuteNonQuery(sql, 
									new SqliteParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqliteParameter("@"+DatabaseManager.fieldId, 	data.GetString(DatabaseManager.fieldId, i)),
									new SqliteParameter("@"+DatabaseManager.fieldValue, data.GetInt(DatabaseManager.fieldValue, i))
									);
			}
			
			data.Cleanup();
			
			return true;
			
		}
		
		// -------------------------------------------------------------------------------
		// SaveEquipment
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveEquipment(BaseDataTable data)
		{
		
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableEquipment, data.GetString(DatabaseManager.fieldName));
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4}, @{5}, @{6}, @{7}, @{8})";
				sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableEquipment, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldSlot, DatabaseManager.fieldAmount, DatabaseManager.fieldAmmo, DatabaseManager.fieldCharges, DatabaseManager.fieldLevel);
				await ExecuteNonQuery(sql, 
									new SqliteParameter("@"+DatabaseManager.fieldName, 		data.GetString(DatabaseManager.fieldName, i)),
									new SqliteParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqliteParameter("@"+DatabaseManager.fieldSlot, 		data.GetInt(DatabaseManager.fieldSlot, i)),
									new SqliteParameter("@"+DatabaseManager.fieldAmount, 	data.GetInt(DatabaseManager.fieldAmount, i)),
									new SqliteParameter("@"+DatabaseManager.fieldAmmo, 		data.GetInt(DatabaseManager.fieldAmmo, i)),
									new SqliteParameter("@"+DatabaseManager.fieldCharges, 	data.GetInt(DatabaseManager.fieldCharges, i)),
									new SqliteParameter("@"+DatabaseManager.fieldLevel, 	data.GetInt(DatabaseManager.fieldLevel, i))
									);
			}
			
			data.Cleanup();
			
			return true;
			
		}
		
		// -------------------------------------------------------------------------------
		// SaveInventory
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveInventory(BaseDataTable data)
		{
		
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableInventory, data.GetString(DatabaseManager.fieldName));
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4}, @{5}, @{6}, @{7}, @{8})";
				sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableInventory, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.fieldAmount, DatabaseManager.fieldAmmo, DatabaseManager.fieldCharges, DatabaseManager.fieldLevel);
				await ExecuteNonQuery(sql, 
									new SqliteParameter("@"+DatabaseManager.fieldName, 		data.GetString(DatabaseManager.fieldName, i)),
									new SqliteParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqliteParameter("@"+DatabaseManager.fieldSlot, 		data.GetInt(DatabaseManager.fieldSlot, i)),
									new SqliteParameter("@"+DatabaseManager.fieldAmount, 	data.GetInt(DatabaseManager.fieldAmount, i)),
									new SqliteParameter("@"+DatabaseManager.fieldAmmo, 		data.GetInt(DatabaseManager.fieldAmmo, i)),
									new SqliteParameter("@"+DatabaseManager.fieldCharges, 	data.GetInt(DatabaseManager.fieldCharges, i)),
									new SqliteParameter("@"+DatabaseManager.fieldLevel, 	data.GetInt(DatabaseManager.fieldLevel, i))
									);
			}
			
			data.Cleanup();
			
			return true;
		}
		
		// -------------------------------------------------------------------------------
		// SaveSkills
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveSkills(BaseDataTable data)
		{
			
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableSkills, data.GetString(DatabaseManager.fieldName));
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4})";
				sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableSkills, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldLevel, DatabaseManager.fieldDuration);
				await ExecuteNonQuery(sql, 
									new SqliteParameter("@"+DatabaseManager.fieldName, 		data.GetString(DatabaseManager.fieldName, i)),
									new SqliteParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqliteParameter("@"+DatabaseManager.fieldLevel, 	data.GetInt(DatabaseManager.fieldLevel, i)),
									new SqliteParameter("@"+DatabaseManager.fieldDuration, 	data.GetInt(DatabaseManager.fieldDuration, i))
									);
			}
			
			data.Cleanup();
			
			return true;
		}
		
		// -------------------------------------------------------------------------------
		// SaveStatus
		// -------------------------------------------------------------------------------
		public async Task<bool> SaveStatus(BaseDataTable data)
		{
		
			if (!data.IsEmpty)
				DeleteRows(DatabaseManager.tableStatus, data.GetString(DatabaseManager.fieldName));
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				string sql = "INSERT INTO {0}{1} VALUES (@{2}, @{3}, @{4})";
				sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableStatus, DatabaseManager.fieldName, DatabaseManager.fieldId, DatabaseManager.fieldLevel, DatabaseManager.fieldDuration);
				await ExecuteNonQuery(sql, 
									new SqliteParameter("@"+DatabaseManager.fieldName, 		data.GetString(DatabaseManager.fieldName, i)),
									new SqliteParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqliteParameter("@"+DatabaseManager.fieldLevel, 	data.GetInt(DatabaseManager.fieldLevel, i)),
									new SqliteParameter("@"+DatabaseManager.fieldDuration, 	data.GetInt(DatabaseManager.fieldDuration, i))
									);
			}
			
			data.Cleanup();
			
			return true;
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}