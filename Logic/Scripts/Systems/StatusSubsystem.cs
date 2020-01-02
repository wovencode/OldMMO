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
	// StatusSubsystem
	// ===================================================================================
	[System.Serializable]
	[NetworkSettings(sendInterval = 1f)]
	[DisallowMultipleComponent]
	public partial class StatusSubsystem : BaseSystem
	{
	
		public BaseStatus[] defaultStatus;
		
		public SyncListStatus syncStatus = new SyncListStatus();
		
		// -------------------------------------------------------------------------------
		// Init
		// -------------------------------------------------------------------------------
		public override void Init(GameObject _parent = null)
		{
			
			base.Init(_parent);
			
			syncStatus.Clear();
			
			foreach (BaseStatus status in defaultStatus)
			{
				if (status.template != null) {
					SStatus sStatus = new SStatus(status.template.GetId, status.level, status.duration);
					syncStatus.Add(sStatus);
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
			syncStatus.Clear();
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				TemplateStatus tmpl;
				
				if (DataManager.dictStatus.TryGetValue(data.GetIdHash(i), out tmpl))
				{
					SStatus sStatus = new SStatus(tmpl.GetId, data.GetLongAsInt(DatabaseManager.fieldLevel, i), data.GetDouble(DatabaseManager.fieldDuration, i));
					syncStatus.Add(sStatus);
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
			
			for (int i = 0; i < syncStatus.Count; ++i)
			{
				data.AddString(DatabaseManager.fieldName, 		parent.name, i);
				data.AddString(DatabaseManager.fieldId, 		syncStatus[i].name, i);
				data.AddInt(DatabaseManager.fieldLevel, 		syncStatus[i].nLevel, i);
				data.AddDouble(DatabaseManager.fieldDuration, 	syncStatus[i].dTimer, i);
			}
			
			return data;
		
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}