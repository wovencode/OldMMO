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
	// AspectSubsystem
	// ===================================================================================
	[System.Serializable]
	[NetworkSettings(sendInterval = 1f)]
	[DisallowMultipleComponent]
	public partial class AspectSubsystem : BaseSystem
	{
		
		public List<TemplateAspect> defaultAspects = new List<TemplateAspect>();
		public SyncListAspect syncAspects = new SyncListAspect();
		
		/* cache */
		protected GameObject _actorPrefab;
		
		// -------------------------------------------------------------------------------
		// Init
		// -------------------------------------------------------------------------------
		public void Init(GameObject _parent = null, List<TemplateAspect> _defaultAspects = null)
		{
			base.Init(_parent);
			syncAspects.Clear();
			defaultAspects.AddRange(_defaultAspects);
			
			for (int i = 0; i < defaultAspects.Count; ++i)
			{
				SAspect sAspect = new SAspect(defaultAspects[i].GetId);
				syncAspects.Add(sAspect);
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
			syncAspects.Clear();
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				TemplateAspect tmpl;
				
				if (DataManager.dictAspect.TryGetValue(data.GetIdHash(i), out tmpl))
				{
					SAspect sAspect = new SAspect(tmpl.GetId, data.GetLongAsInt(DatabaseManager.fieldValue, i));
					syncAspects.Add(sAspect);
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
			
			for (int i = 0; i < syncAspects.Count; ++i)
			{
				data.AddString(DatabaseManager.fieldName, 	parent.name, 			i);
				data.AddString(DatabaseManager.fieldId, 	syncAspects[i].name, 	i);
				data.AddInt(DatabaseManager.fieldValue, 	syncAspects[i].nValue, 	i);
			}
			
			return data;
			
		}
		
		// -------------------------------------------------------------------------------
		// GetAspects
		// -------------------------------------------------------------------------------
		public int[] GetAspects()
		{
		
			int[] aspects = new int[syncAspects.Count];
			
			for (int i = 0; i < syncAspects.Count; ++i)
				aspects[i] = syncAspects[i].name.GetFNVHashCode();

			return aspects;
		
		}
		
		// -------------------------------------------------------------------------------
		// GetPrefab
		// -------------------------------------------------------------------------------
		public GameObject GetPrefab()
		{
			
			if (_actorPrefab) return _actorPrefab;
			
			foreach (SAspect aspect in syncAspects)
			{
				
				TemplateAspect tmpl;
				
				if (DataManager.dictAspect.TryGetValue(aspect.nId, out tmpl))
				{
					if (tmpl.actorPrefab)
					{
						_actorPrefab = tmpl.actorPrefab;
						return _actorPrefab;
					}
				}
			
			}
			
			return null;
		
		}		
		
		// -------------------------------------------------------------------------------
		
	}
	
}