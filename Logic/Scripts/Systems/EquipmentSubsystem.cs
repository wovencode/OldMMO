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
	// EquipmentSystem
	// ===================================================================================
	[System.Serializable]
	[NetworkSettings(sendInterval = 1f)]
	[DisallowMultipleComponent]
	public partial class EquipmentSubsystem : BaseSystem
	{
		
		public BaseItem[] defaultEquipment;
		
		public SyncListEquipment syncEquipment = new SyncListEquipment();
		
		// -------------------------------------------------------------------------------
		// Init
		// -------------------------------------------------------------------------------
		public void Init(GameObject _parent = null, int level = 1)
		{
			
			base.Init(_parent);
			
			syncEquipment.Clear();
			
			foreach (BaseItem equipment in defaultEquipment)
			{
				if (equipment.template != null) {
					SItem sItem = new SItem(equipment.template.GetId, equipment.template.slot, equipment.template.amount, equipment.template.ammo, equipment.template.charges, equipment.template.level);
					syncEquipment.Add(sItem);
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
			syncEquipment.Clear();
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				TemplateItem tmpl;
				
				if (DataManager.dictItem.TryGetValue(data.GetIdHash(i), out tmpl))
				{
					SItem sItem = new SItem(tmpl.GetId, data.GetLongAsInt(DatabaseManager.fieldSlot, i), data.GetLongAsInt(DatabaseManager.fieldAmount, i), data.GetLongAsInt(DatabaseManager.fieldAmmo, i), data.GetLongAsInt(DatabaseManager.fieldCharges, i), data.GetLongAsInt(DatabaseManager.fieldLevel, i));
					syncEquipment.Add(sItem);
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
			
			for (int i = 0; i < syncEquipment.Count; ++i)
			{
				data.AddString(DatabaseManager.fieldName, 	parent.name, 				i);
				data.AddString(DatabaseManager.fieldId, 	syncEquipment[i].name, 		i);
				data.AddInt(DatabaseManager.fieldId, 		syncEquipment[i].nSlot, 	i);
				data.AddInt(DatabaseManager.fieldId, 		syncEquipment[i].nAmount, 	i);
				data.AddInt(DatabaseManager.fieldId, 		syncEquipment[i].nAmmo, 	i);
				data.AddInt(DatabaseManager.fieldId, 		syncEquipment[i].nCharges, 	i);
				data.AddInt(DatabaseManager.fieldLevel, 	syncEquipment[i].nLevel, 	i);
				
			}
			
			return data;
		
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}