// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// InventorySubsystem
	// ===================================================================================
	[System.Serializable]
	[NetworkSettings(sendInterval = 1f)]
	[DisallowMultipleComponent]
	public partial class InventorySubsystem : BaseSystem
	{
	
		public BaseItem[] defaultInventory;
		
		public SyncListInventory syncInventory = new SyncListInventory();
		
		// -------------------------------------------------------------------------------
		// Init
		// -------------------------------------------------------------------------------
		public void Init(GameObject _parent = null, int level = 1)
		{
			
			base.Init(_parent);
			
			syncInventory.Clear();
			
			int i = 0;
			
			foreach (BaseItem item in defaultInventory)
			{
				if (item.template != null) {
					SItem sItem = new SItem(item.template.GetId, i, item.template.amount, item.template.ammo, item.template.charges, item.template.level);
					syncInventory.Add(sItem);
					i++;
				}
			}
		}
		
		// -------------------------------------------------------------------------------
		// Overwrite
		// -------------------------------------------------------------------------------
		public override void Overwrite(List<TemplateAspect> listAspects)
		{
		
		}		
		
		// -------------------------------------------------------------------------------
		// Load 
		// -------------------------------------------------------------------------------
		public override void Load(BaseDataTable data)
		{
			syncInventory.Clear();
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				TemplateItem tmpl;
				
				if (DataManager.dictItem.TryGetValue(data.GetIdHash(i), out tmpl))
				{
					SItem sItem = new SItem(tmpl.GetId, data.GetLongAsInt(DatabaseManager.fieldSlot, i), data.GetLongAsInt(DatabaseManager.fieldAmount, i), data.GetLongAsInt(DatabaseManager.fieldAmmo, i), data.GetLongAsInt(DatabaseManager.fieldCharges, i), data.GetLongAsInt(DatabaseManager.fieldLevel, i));
					syncInventory.Add(sItem);
				}
				else
				{
					Debug.LogWarning("Skipped template '"+data.GetString(DatabaseManager.fieldName)+"' as it was not found in Library.");
				}
			
			}
			
			data.Cleanup();
		}
		
		// -------------------------------------------------------------------------------
		// Save
		// -------------------------------------------------------------------------------
		public override BaseDataTable Save()
		{
		
			BaseDataTable data = new BaseDataTable();
			
			for (int i = 0; i < syncInventory.Count; ++i)
			{
				data.AddString(DatabaseManager.fieldName, 	parent.name, 				i);
				data.AddString(DatabaseManager.fieldId, 	syncInventory[i].name, 		i);
				data.AddInt(DatabaseManager.fieldId, 		syncInventory[i].nSlot, 	i);
				data.AddInt(DatabaseManager.fieldId, 		syncInventory[i].nAmount, 	i);
				data.AddInt(DatabaseManager.fieldId, 		syncInventory[i].nAmmo, 	i);
				data.AddInt(DatabaseManager.fieldId, 		syncInventory[i].nCharges, 	i);
				data.AddInt(DatabaseManager.fieldLevel, 	syncInventory[i].nLevel, 	i);
				
			}
			
			return data;
		
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}