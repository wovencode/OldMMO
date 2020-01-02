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
	// AlignmentSubsystem
	// ===================================================================================
	[System.Serializable]
	[NetworkSettings(sendInterval = 1f)]
	[DisallowMultipleComponent]
	public partial class AlignmentSubsystem : BaseSystem
	{
		
		public List<TemplateAlignment> defaultAlignments = new List<TemplateAlignment>();
		public SyncListAlignment syncAlignments = new SyncListAlignment();
		
		// -------------------------------------------------------------------------------
		// Init
		// -------------------------------------------------------------------------------
		public void Init(GameObject _parent = null, List<TemplateAlignment> _defaultAlignments = null)
		{
			base.Init(_parent);
			syncAlignments.Clear();
			defaultAlignments.AddRange(_defaultAlignments);
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
			syncAlignments.Clear();
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				TemplateAlignment tmpl;
				
				if (DataManager.dictAlignment.TryGetValue(data.GetIdHash(i), out tmpl))
				{
					SAlignment sAlignment = new SAlignment(tmpl.GetId, data.GetLongAsInt(DatabaseManager.fieldValue, i));
					syncAlignments.Add(sAlignment);
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
			
			for (int i = 0; i < syncAlignments.Count; ++i)
			{
				data.AddString(DatabaseManager.fieldName, 	parent.name, 				i);
				data.AddString(DatabaseManager.fieldId, 	syncAlignments[i].name, 	i);
				data.AddInt(DatabaseManager.fieldValue, 	syncAlignments[i].nValue, 	i);
			}
			
			return data;
			
		}
				
		// -------------------------------------------------------------------------------
		
	}
	
}