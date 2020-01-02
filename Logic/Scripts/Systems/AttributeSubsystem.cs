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
	// AttributeSubsystem
	// ===================================================================================
	[System.Serializable]
	[NetworkSettings(sendInterval = 1f)]
	[DisallowMultipleComponent]
	public partial class AttributeSubsystem : BaseSystem
	{
		
		public BaseAttribute[] defaultAttributes;
		
		public SyncListAttribute syncAttributes = new SyncListAttribute();
		
		// -------------------------------------------------------------------------------
		// Init
		// -------------------------------------------------------------------------------
		public void Init(GameObject _parent = null, int level = 1)
		{
			
			base.Init(_parent);
			
			syncAttributes.Clear();
			
			foreach (BaseAttribute attribute in defaultAttributes)
			{
				if (attribute.template != null)
				{
					SAttribute sAttribute = new SAttribute(attribute.template.GetId, attribute.value.Get(level));
					syncAttributes.Add(sAttribute);
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
			syncAttributes.Clear();
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				TemplateAttribute tmpl;
				
				if (DataManager.dictAttribute.TryGetValue(data.GetIdHash(i), out tmpl))
				{
					SAttribute sAttribute = new SAttribute(tmpl.GetId, data.GetLongAsInt(DatabaseManager.fieldValue, i));
					syncAttributes.Add(sAttribute);
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
			
			for (int i = 0; i < syncAttributes.Count; ++i)
			{
				data.AddString(DatabaseManager.fieldName, 	parent.name, 				i);
				data.AddString(DatabaseManager.fieldId, 	syncAttributes[i].name, 	i);
				data.AddInt(DatabaseManager.fieldValue, 	syncAttributes[i].nValue, 	i);
			}
			
			return data;
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}