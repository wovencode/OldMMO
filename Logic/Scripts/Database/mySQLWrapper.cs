// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using Mono.Data.Sqlite; 				// copied from Unity/Mono/lib/mono/2.0 to Plugins
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using UnityEngine;
using System.Threading.Tasks;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// mySQLWrapper
	// ===================================================================================
	public partial class MySQLWrapper : IDatabaseProvider
	{
	
		protected DatabaseManager databaseManager;
		protected SqlConnection sqlConnection;
		protected string _connectionString = null;
		
		// -------------------------------------------------------------------------------
		// MySQLWrapper (Constructor)
		// -------------------------------------------------------------------------------
		public MySQLWrapper(DatabaseManager _databaseManager)
		{
			databaseManager = _databaseManager;
			AutoSetupDatabase();
			
		}
		
		// -------------------------------------------------------------------------------
		// MySQLWrapper (Destructor)
		// -------------------------------------------------------------------------------
		~MySQLWrapper()
		{
			Shutdown();
		}
				
		// -------------------------------------------------------------------------------
		// Shutdown
		// -------------------------------------------------------------------------------
		public void Shutdown()
		{
			sqlConnection.Close();
		}
		
		// -------------------------------------------------------------------------------
		// AutoSetupDatabase
		// -------------------------------------------------------------------------------
		public void AutoSetupDatabase()
		{
			SqlConnection sqlConnection = new SqlConnection(connectionString);
			sqlConnection.Open();
		}
		
		// -------------------------------------------------------------------------------
		// ConnectionString
		// -------------------------------------------------------------------------------
		protected string connectionString
    	{
			get
			{
				if (_connectionString == null)
				{
					MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
					{
						Server 			= databaseManager.dbHost,
						Database 		= databaseManager.dbName,
						UserID 			= databaseManager.dbUser,
						Password 		= databaseManager.dbPassword,
						Port 			= databaseManager.dbPort,
						CharacterSet 	= databaseManager.dbCharacterSet

					};
					_connectionString = connectionStringBuilder.ConnectionString;
				}

				return _connectionString;
			}
		}
		
		
		
		// -------------------------------------------------------------------------------
		// ExecuteScalar
		// -------------------------------------------------------------------------------
		public async Task<object> ExecuteScalar(string sql, params SqlParameter[] args)
		{
			try
			{
				using (SqlCommand command = new SqlCommand(sql, sqlConnection))
        		{
        		
            		foreach (SqlParameter param in args)
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
		// ExecuteSelect
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> ExecuteReader(string sql, params SqlParameter[] args)
		{
			BaseDataTable data = new BaseDataTable();

			try
			{
				
				SqlCommand command = new SqlCommand();
				
				foreach (SqlParameter param in args)
					command.Parameters.Add(param);
				
                SqlDataReader reader = await command.ExecuteReaderAsync();
				
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
		public async Task<bool> ExecuteNonQuery(string sql, params SqlParameter[] args)
		{
			try
			{
				SqlCommand command = new SqlCommand(sql, sqlConnection);
				
				foreach (SqlParameter param in args)
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
			var result = await ExecuteScalar(sql, new SqlParameter("@"+sField, sName.ToLower()));
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
			var result = await ExecuteScalar(sql, new SqlParameter("@"+sFieldName, sFieldValue.ToLower()));
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
			return await ExecuteNonQuery(sql, new SqlParameter("@"+sField, sValue.ToLower() ));
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
			return await ExecuteNonQuery(sql, new SqlParameter("@"+sField, sValue.ToLower() ));
		}
		
		// -------------------------------------------------------------------------------
		// DeleteRows
		// -------------------------------------------------------------------------------
		public async Task<bool> DeleteRows(string sTable, string sName)
		{
			string sql = "DELETE FROM {0}{1} WHERE {2} = @{2}";
			sql = string.Format(sql, DatabaseManager.tablePrefix, sTable, DatabaseManager.fieldName);
			return await ExecuteNonQuery(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName.ToLower()));
		}
		
		// -------------------------------------------------------------------------------
		// SelectRows
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> SelectRows(string sTargetField, string sTable, string sField, string sName)
		{
			string sql = "SELECT {0} FROM {1}{2} WHERE {3} = @{3}";
			sql = string.Format(sql, sTargetField, DatabaseManager.tablePrefix, sTable, sField);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName.ToLower()));
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
			var result = await ExecuteScalar(sql, new SqlParameter("@"+DatabaseManager.fieldId, sName.ToLower()));
			return Convert.ToInt32((long)result);
		}
		
		// -------------------------------------------------------------------------------
		// SelectActors
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> SelectActors(string sName)
		{
			string sql = "SELECT {0} FROM {1}{2} WHERE {3} = @{3} AND {4} != 1 AND {5} != 1";
			sql = string.Format(sql, DatabaseManager.fieldName, DatabaseManager.tablePrefix, DatabaseManager.tableActorPlayers, DatabaseManager.fieldId, DatabaseManager.fieldBanned, DatabaseManager.fieldDeleted);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldId, sName.ToLower()));
		}
		
		// ===============================================================================
		// CREATE TABLES
		// ===============================================================================
		
		
		// -------------------------------------------------------------------------------
		// CreateTableAccounts
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableAccounts()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL PRIMARY KEY, {4} VARCHAR({5}) NOT NULL, {6} VARCHAR({7}) NOT NULL, {8} INT NOT NULL, {9} VARCHAR({10}) NOT NULL, {11} INT NOT NULL, {12} INT NOT NULL, {13} INT NOT NULL, {14} INT NOT NULL, {15} INT NOT NULL)";
			sql = string.Format(sql, 
			DatabaseManager.tablePrefix, 
			DatabaseManager.tableAccounts, 
			DatabaseManager.fieldName, 
			DatabaseManager.maxLengthName, 
			DatabaseManager.fieldPassword, 
			DatabaseManager.maxLengthPassword, 
			DatabaseManager.fieldEmail, 
			DatabaseManager.maxLengthEmail, 
			DatabaseManager.fieldAction, 
			DatabaseManager.fieldDeviceId, 
			DatabaseManager.maxLengthDeviceId, 
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
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL, {4} VARCHAR({5}) NOT NULL, {6} INT NOT NULL, {7} INT NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, 
			DatabaseManager.tablePrefix, 
			DatabaseManager.tableActorPlayers, 
			DatabaseManager.fieldName,
			DatabaseManager.maxLengthName,
			DatabaseManager.fieldId, 
			DatabaseManager.maxLengthId,
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
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL, {4} VARCHAR({5}) NOT NULL, {6} INT NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableAlignments, DatabaseManager.fieldName, DatabaseManager.maxLengthName, DatabaseManager.fieldId, DatabaseManager.maxLengthId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}
		
		// -------------------------------------------------------------------------------
		// CreateTableAspects
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableAspects()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL, {4} VARCHAR({5}) NOT NULL, {6} INT NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableAspects, DatabaseManager.fieldName, DatabaseManager.maxLengthName, DatabaseManager.fieldId, DatabaseManager.maxLengthId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}
		
		// -------------------------------------------------------------------------------
		// CreateTableProfessions
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableProfessions()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL, {4} VARCHAR({5}) NOT NULL, {6} INT NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableProfessions, DatabaseManager.fieldName, DatabaseManager.maxLengthName, DatabaseManager.fieldId, DatabaseManager.maxLengthId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}
		
		// -------------------------------------------------------------------------------
		// CreateTableAttributes
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableAttributes()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL, {4} VARCHAR({5}) NOT NULL, {6} INT NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableAttributes, DatabaseManager.fieldName, DatabaseManager.maxLengthName, DatabaseManager.fieldId, DatabaseManager.maxLengthId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}
		
		// -------------------------------------------------------------------------------
		// CreateTableCurrencies
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableCurrencies()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL, {4} VARCHAR({5}) NOT NULL, {6} INT NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableCurrencies, DatabaseManager.fieldName, DatabaseManager.maxLengthName, DatabaseManager.fieldId, DatabaseManager.maxLengthId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}
		
		// -------------------------------------------------------------------------------
		// CreateTableEnergies
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableEnergies()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL, {4} VARCHAR({5}) NOT NULL, {6} INT NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableEnergies, DatabaseManager.fieldName, DatabaseManager.maxLengthName, DatabaseManager.fieldId, DatabaseManager.maxLengthId, DatabaseManager.fieldValue);
			return await ExecuteNonQuery(sql);
		}
		
		// -------------------------------------------------------------------------------
		// CreateTableEquipment
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableEquipment()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL, {4} VARCHAR({5}) NOT NULL, {6} INT NOT NULL, {7} INT NOT NULL, {8} INT NOT NULL, {9} INT NOT NULL, {10} INT NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableEquipment, DatabaseManager.fieldName, DatabaseManager.maxLengthName, DatabaseManager.fieldId, DatabaseManager.maxLengthId, DatabaseManager.fieldSlot, DatabaseManager.fieldAmount, DatabaseManager.fieldAmmo, DatabaseManager.fieldCharges, DatabaseManager.fieldLevel);
			return await ExecuteNonQuery(sql);
		}
		
		// -------------------------------------------------------------------------------
		// CreateTableInventory
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableInventory()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL, {4} VARCHAR({5}) NOT NULL, {6} INT NOT NULL, {7} INT NOT NULL, {8} INT NOT NULL, {9} INT NOT NULL, {10} INT NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableInventory, DatabaseManager.fieldName, DatabaseManager.maxLengthName, DatabaseManager.fieldId, DatabaseManager.maxLengthId, DatabaseManager.fieldSlot, DatabaseManager.fieldAmount, DatabaseManager.fieldAmmo, DatabaseManager.fieldCharges, DatabaseManager.fieldLevel);
			return await ExecuteNonQuery(sql);
		}
		
		// -------------------------------------------------------------------------------
		// CreateTableSkills
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableSkills()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL, {4} VARCHAR({5}) NOT NULL, {6} INT NOT NULL, {7} INT NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableSkills, DatabaseManager.fieldName, DatabaseManager.maxLengthName, DatabaseManager.fieldId, DatabaseManager.maxLengthId, DatabaseManager.fieldLevel, DatabaseManager.fieldDuration);
			return await ExecuteNonQuery(sql);
		}
		
		// -------------------------------------------------------------------------------
		// CreateTableStatus
		// -------------------------------------------------------------------------------
		public async Task<bool> CreateTableStatus()
		{
			string sql = "CREATE TABLE IF NOT EXISTS {0}{1} ({2} VARCHAR({3}) NOT NULL, {4} VARCHAR({5}) NOT NULL, {6} INT NOT NULL, {7} INT NOT NULL, PRIMARY KEY({2}, {4}))";
			sql = string.Format(sql, DatabaseManager.tablePrefix, DatabaseManager.tableStatus, DatabaseManager.fieldName, DatabaseManager.maxLengthName, DatabaseManager.fieldId, DatabaseManager.maxLengthId, DatabaseManager.fieldLevel, DatabaseManager.fieldDuration);
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
			return null;
		}*/
		
		// -------------------------------------------------------------------------------
		// LoadAccount
		// We can load any account either via the account name or via its email addresss (both unique)
		// We always compare Names with lowercase to prevent dupes like: Fhiz, fhiz, FHIZ
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadAccount(string sName)
		{
			string sql = "";
			
			sql = "SELECT {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9} FROM {10}{11} WHERE LOWER(`{12}`) = @{12}";
			sql = string.Format(sql, DatabaseManager.fieldName, DatabaseManager.fieldPassword, DatabaseManager.fieldEmail, DatabaseManager.fieldAction, DatabaseManager.fieldDeviceId, DatabaseManager.fieldBanned, DatabaseManager.fieldDeleted, DatabaseManager.fieldConfirmed, DatabaseManager.fieldCode, DatabaseManager.fieldDone, DatabaseManager.tablePrefix, DatabaseManager.tableAccounts, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName.ToLower()));
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
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadAlignments
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadAlignments(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableAlignments, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadAspects
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadAspects(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableAspects, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadProfessions
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadProfessions(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableProfessions, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadAttributes
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadAttributes(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableAttributes, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadCurrencies
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadCurrencies(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableCurrencies, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadEnergies
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadEnergies(string sName)
		{
			string sql = "SELECT {0}, {1} FROM {2}{3} WHERE {4} = @{4}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldValue, DatabaseManager.tablePrefix, DatabaseManager.tableEnergies, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadEquipment
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadEquipment(string sName)
		{
			string sql = "SELECT {0}, {1}, {2}, {3}, {4}, {5} FROM {6}{7} WHERE {8} = @{8}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldSlot, DatabaseManager.fieldAmount, DatabaseManager.fieldAmmo, DatabaseManager.fieldCharges, DatabaseManager.fieldLevel, DatabaseManager.tablePrefix, DatabaseManager.tableEquipment, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadInventory
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadInventory(string sName)
		{
			string sql = "SELECT {0}, {1}, {2}, {3}, {4}, {5} FROM {6}{7} WHERE {8} = @{8}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldSlot, DatabaseManager.fieldAmount, DatabaseManager.fieldAmmo, DatabaseManager.fieldCharges, DatabaseManager.fieldLevel, DatabaseManager.tablePrefix, DatabaseManager.tableInventory, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadSkills
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadSkills(string sName)
		{
			string sql = "SELECT {0}, {1}, {2} FROM {3}{4} WHERE {5} = @{5}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldLevel, DatabaseManager.fieldDuration, DatabaseManager.tablePrefix, DatabaseManager.tableSkills, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName));
		}
		
		// -------------------------------------------------------------------------------
		// LoadStatus
		// -------------------------------------------------------------------------------
		public async Task<BaseDataTable> LoadStatus(string sName)
		{
			string sql = "SELECT {0}, {1}, {2} FROM {3}{4} WHERE {5} = @{5}";
			sql = string.Format(sql, DatabaseManager.fieldId, DatabaseManager.fieldLevel, DatabaseManager.fieldDuration, DatabaseManager.tablePrefix, DatabaseManager.tableStatus, DatabaseManager.fieldName);
			return await ExecuteReader(sql, new SqlParameter("@"+DatabaseManager.fieldName, sName));
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
								new SqlParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName)),
								new SqlParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId)),
								new SqlParameter("@"+DatabaseManager.fieldBanned, 	data.GetInt(DatabaseManager.fieldBanned)),
								new SqlParameter("@"+DatabaseManager.fieldDeleted, 	data.GetInt(DatabaseManager.fieldDeleted))
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
								new SqlParameter("@"+DatabaseManager.fieldName, 		data.GetString(DatabaseManager.fieldName)),
								new SqlParameter("@"+DatabaseManager.fieldPassword, 	data.GetString(DatabaseManager.fieldPassword)),
								new SqlParameter("@"+DatabaseManager.fieldEmail, 		data.GetString(DatabaseManager.fieldEmail)),
								new SqlParameter("@"+DatabaseManager.fieldAction, 		data.GetString(DatabaseManager.fieldAction)),
								new SqlParameter("@"+DatabaseManager.fieldDeviceId, 	data.GetString(DatabaseManager.fieldDeviceId)),
								new SqlParameter("@"+DatabaseManager.fieldBanned, 		data.GetBoolAsInt(DatabaseManager.fieldBanned)),
								new SqlParameter("@"+DatabaseManager.fieldDeleted, 		data.GetBoolAsInt(DatabaseManager.fieldDeleted)),
								new SqlParameter("@"+DatabaseManager.fieldConfirmed, 	data.GetBoolAsInt(DatabaseManager.fieldConfirmed)),
								new SqlParameter("@"+DatabaseManager.fieldDone,		 	data.GetBoolAsInt(DatabaseManager.fieldDone)),
								new SqlParameter("@"+DatabaseManager.fieldCode, 		data.GetInt(DatabaseManager.fieldCode))
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
									new SqlParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqlParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqlParameter("@"+DatabaseManager.fieldValue, 	data.GetInt(DatabaseManager.fieldValue, i))
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
									new SqlParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqlParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqlParameter("@"+DatabaseManager.fieldValue, 	data.GetInt(DatabaseManager.fieldValue, i))
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
									new SqlParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqlParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqlParameter("@"+DatabaseManager.fieldValue, 	data.GetInt(DatabaseManager.fieldValue, i))
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
									new SqlParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqlParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqlParameter("@"+DatabaseManager.fieldValue, 	data.GetInt(DatabaseManager.fieldValue, i))
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
									new SqlParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqlParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId,i)),
									new SqlParameter("@"+DatabaseManager.fieldValue, 	data.GetInt(DatabaseManager.fieldValue,i))
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
									new SqlParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqlParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqlParameter("@"+DatabaseManager.fieldValue, 	data.GetInt(DatabaseManager.fieldValue, i))
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
									new SqlParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqlParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqlParameter("@"+DatabaseManager.fieldSlot, 	data.GetInt(DatabaseManager.fieldSlot, i)),
									new SqlParameter("@"+DatabaseManager.fieldAmount, 	data.GetInt(DatabaseManager.fieldAmount, i)),
									new SqlParameter("@"+DatabaseManager.fieldAmmo, 	data.GetInt(DatabaseManager.fieldAmmo, i)),
									new SqlParameter("@"+DatabaseManager.fieldCharges, 	data.GetInt(DatabaseManager.fieldCharges, i)),
									new SqlParameter("@"+DatabaseManager.fieldLevel, 	data.GetInt(DatabaseManager.fieldLevel, i))
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
									new SqlParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqlParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqlParameter("@"+DatabaseManager.fieldSlot, 	data.GetInt(DatabaseManager.fieldSlot, i)),
									new SqlParameter("@"+DatabaseManager.fieldAmount, 	data.GetInt(DatabaseManager.fieldAmount, i)),
									new SqlParameter("@"+DatabaseManager.fieldAmmo, 	data.GetInt(DatabaseManager.fieldAmmo, i)),
									new SqlParameter("@"+DatabaseManager.fieldCharges, 	data.GetInt(DatabaseManager.fieldCharges, i)),
									new SqlParameter("@"+DatabaseManager.fieldLevel, 	data.GetInt(DatabaseManager.fieldLevel, i))
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
									new SqlParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqlParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqlParameter("@"+DatabaseManager.fieldLevel, 	data.GetInt(DatabaseManager.fieldLevel, i)),
									new SqlParameter("@"+DatabaseManager.fieldDuration, data.GetInt(DatabaseManager.fieldDuration, i))
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
									new SqlParameter("@"+DatabaseManager.fieldName, 	data.GetString(DatabaseManager.fieldName, i)),
									new SqlParameter("@"+DatabaseManager.fieldId, 		data.GetString(DatabaseManager.fieldId, i)),
									new SqlParameter("@"+DatabaseManager.fieldLevel, 	data.GetInt(DatabaseManager.fieldLevel, i)),
									new SqlParameter("@"+DatabaseManager.fieldDuration, data.GetInt(DatabaseManager.fieldDuration, i))
									);
			}
			
			data.Cleanup();
			
			return true;
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}