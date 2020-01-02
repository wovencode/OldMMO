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
	// SkillSubsystem
	// ===================================================================================
	[System.Serializable]
	[NetworkSettings(sendInterval = 1f)]
	[DisallowMultipleComponent]
	public partial class SkillSubsystem : BaseSystem
	{
	
		public BaseSkill[] defaultSkills;
		
		public SyncListSkill syncSkills = new SyncListSkill();
		
		// -------------------------------------------------------------------------------
		// Init
		// -------------------------------------------------------------------------------
		public void Init(GameObject _parent = null, int level = 1)
		{
			
			base.Init(_parent);
			
			syncSkills.Clear();
			
			foreach (BaseSkill skill in defaultSkills)
			{
				if (skill.template != null) {
					SSkill sSkill = new SSkill(skill.template.GetId, skill.level, 0);
					syncSkills.Add(sSkill);
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
			syncSkills.Clear();
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				TemplateSkill tmpl;
				
				if (DataManager.dictSkill.TryGetValue(data.GetIdHash(i), out tmpl))
				{
					SSkill sSkill = new SSkill(tmpl.GetId, data.GetLongAsInt(DatabaseManager.fieldLevel, i), data.GetDouble(DatabaseManager.fieldDuration, i));
					syncSkills.Add(sSkill);
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
			
			for (int i = 0; i < syncSkills.Count; ++i)
			{
				data.AddString(DatabaseManager.fieldName, 		parent.name, i);
				data.AddString(DatabaseManager.fieldId, 		syncSkills[i].name, i);
				data.AddInt(DatabaseManager.fieldLevel, 		syncSkills[i].nLevel, i);
				data.AddDouble(DatabaseManager.fieldDuration, 	syncSkills[i].dTimer, i);
			}
			
			return data;
		
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}