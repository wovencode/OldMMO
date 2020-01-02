// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Rendering;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// ProfessionSubsystem
	// ===================================================================================
	[System.Serializable]
	[NetworkSettings(sendInterval = 1f)]
	[DisallowMultipleComponent]
	public partial class ProfessionSubsystem : BaseSystem
	{
		
		public BaseProfession[] defaultProfessions;
		
		public SyncListProfession syncProfessions = new SyncListProfession();
		
		// -------------------------------------------------------------------------------
		// Init
		// -------------------------------------------------------------------------------
		public void Init(GameObject _parent = null, int level = 1)
		{
			
			base.Init(_parent);
			
			syncProfessions.Clear();
			
			foreach (BaseProfession profession in defaultProfessions)
			{
				if (profession.template != null) {
					SProfession sProfession = new SProfession(profession.template.GetId, profession.value.Get(level));
					syncProfessions.Add(sProfession);
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
			syncProfessions.Clear();
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				TemplateProfession tmpl;
				
				if (DataManager.dictProfession.TryGetValue(data.GetIdHash(i), out tmpl))
				{
					SProfession sProfession = new SProfession(tmpl.GetId, data.GetLongAsInt(DatabaseManager.fieldValue, i));
					syncProfessions.Add(sProfession);
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
			
			for (int i = 0; i < syncProfessions.Count; ++i)
			{
				data.AddString(DatabaseManager.fieldName, 	parent.name, i);
				data.AddString(DatabaseManager.fieldId, 	syncProfessions[i].name, i);
				data.AddInt(DatabaseManager.fieldValue, 	syncProfessions[i].nValue, i);
			}
			
			return data;
		
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}