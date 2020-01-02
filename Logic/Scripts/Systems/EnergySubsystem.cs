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
	// EnergySubsystem
	// ===================================================================================
	[System.Serializable]
	[NetworkSettings(sendInterval = 1f)]
	[DisallowMultipleComponent]
	public partial class EnergySubsystem : BaseSystem
	{
	
		public BaseEnergy[] defaultEnergies;

		public SyncListEnergy syncEnergies = new SyncListEnergy();
		
		// -------------------------------------------------------------------------------
		// Init
		// -------------------------------------------------------------------------------
		public void Init(GameObject _parent = null, int level = 1)
		{
			
			base.Init(_parent);
			
			syncEnergies.Clear();
			
			foreach (BaseEnergy energy in defaultEnergies)
			{
				if (energy.template != null) {
					SEnergy sEnergy = new SEnergy(energy.template.GetId, energy.value.Get(level));
					syncEnergies.Add(sEnergy);
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
			syncEnergies.Clear();
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				TemplateEnergy tmpl;
				
				if (DataManager.dictEnergy.TryGetValue(data.GetIdHash(i), out tmpl))
				{
					SEnergy sEnery = new SEnergy(tmpl.GetId, data.GetLongAsInt(DatabaseManager.fieldValue, i));
					syncEnergies.Add(sEnery);
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
			
			for (int i = 0; i < syncEnergies.Count; ++i)
			{
				data.AddString(DatabaseManager.fieldName, 	parent.name, 				i);
				data.AddString(DatabaseManager.fieldId, 	syncEnergies[i].name, 		i);
				data.AddInt(DatabaseManager.fieldValue, 	syncEnergies[i].nValue, 	i);
			}
			
			return data;
		
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}