// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// IDatabaseProvider
	// ===================================================================================
	public interface IDatabaseProvider
	{
		
		void AutoSetupDatabase();
		void Shutdown();
		
		Task<bool> CreateTableAccounts();
		Task<bool> CreateTableMeta();
		Task<bool> CreateTableAlignments();
		Task<bool> CreateTableAspects();
		Task<bool> CreateTableProfessions();
		Task<bool> CreateTableAttributes();
		Task<bool> CreateTableCurrencies();
		Task<bool> CreateTableEnergies();
		Task<bool> CreateTableEquipment();
		Task<bool> CreateTableInventory();
		Task<bool> CreateTableSkills();
		Task<bool> CreateTableStatus();
		
		Task<int> ActorCount(string sName);
		Task<BaseDataTable> SelectActors(string sName);
		
		Task<int> RowCount(string sTable, string sField, string sName);
		Task<bool> RowExists(string sTable, string sFieldName, string sFieldValue);
		
		Task<bool> DeleteRows(string sTable, string sName);
		Task<BaseDataTable> SelectRows(string sTargetField, string sTable, string sField, string sName);
		Task<bool> UpdateRow(string sTable, string sField, string sValue, string sTargetField, string sTargetValue);
		Task<bool> UpdateRow(string sTable, string sField, string sValue, string sTargetField, int nTargetValue);
		
		Task<BaseDataTable> LoadMeta(string sName);
		Task<BaseDataTable> LoadAlignments(string sName);
		Task<BaseDataTable> LoadAspects(string sName);
		Task<BaseDataTable> LoadProfessions(string sName);
		Task<BaseDataTable> LoadAccount(string sName);
		Task<BaseDataTable> LoadAttributes(string sName);
		Task<BaseDataTable> LoadCurrencies(string sName);
		Task<BaseDataTable> LoadEnergies(string sName);
		Task<BaseDataTable> LoadEquipment(string sName);
		Task<BaseDataTable> LoadInventory(string sName);
		Task<BaseDataTable> LoadSkills(string sName);
		Task<BaseDataTable> LoadStatus(string sName);
		
		Task<bool> SaveActors(BaseDataTable data);
		Task<bool> SaveActorPlayer(GameObject player);
		Task<bool> SaveAccount(BaseDataTable data);
		Task<bool> SaveMeta(BaseDataTable data);
		Task<bool> SaveAlignments(BaseDataTable data);
		Task<bool> SaveAspects(BaseDataTable data);
		Task<bool> SaveProfessions(BaseDataTable data);
		Task<bool> SaveAttributes(BaseDataTable data);
		Task<bool> SaveCurrencies(BaseDataTable data);
		Task<bool> SaveEnergies(BaseDataTable data);
		Task<bool> SaveEquipment(BaseDataTable data);
		Task<bool> SaveInventory(BaseDataTable data);
		Task<bool> SaveSkills(BaseDataTable data);
		Task<bool> SaveStatus(BaseDataTable data);
		
	}
	
}