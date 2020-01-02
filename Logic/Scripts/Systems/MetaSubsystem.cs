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
	// MetaSubsystem
	// ===================================================================================
	[System.Serializable]
	[NetworkSettings(sendInterval = 1f)]
	[DisallowMultipleComponent]
	public partial class MetaSubsystem : BaseSystem
	{
		
		protected string sActorName;
		protected string sAccountName;
		protected bool bBanned;
		protected bool bDeleted;
		
		// -------------------------------------------------------------------------------
		// IsValid
		// -------------------------------------------------------------------------------
		public bool IsValid
		{
			get
			{
				return !bBanned && !bDeleted;
			}
		}
		
		// -------------------------------------------------------------------------------
		// Init
		// -------------------------------------------------------------------------------
		public void Init(GameObject _parent = null, string _sActorName="", string _sAccountName="", bool _bBanned = false, bool _bDeleted = false)
		{
			base.Init(_parent);
			
			if (_parent)
				sActorName = _parent.name;
			
			if (!String.IsNullOrWhiteSpace(_sActorName))
				sActorName = _sActorName;
			
			if (!String.IsNullOrWhiteSpace(_sAccountName))
				sAccountName = _sAccountName;
			
			bBanned 	= _bBanned;
			bDeleted 	= _bDeleted;
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
		}
		
		// -------------------------------------------------------------------------------
		// Save
		// -------------------------------------------------------------------------------
		public override BaseDataTable Save()
		{
			BaseDataTable data = new BaseDataTable();
			
			data.AddString(DatabaseManager.fieldName, 	sActorName);
			data.AddString(DatabaseManager.fieldId, 	sAccountName);
			data.AddBool(DatabaseManager.fieldBanned, 	bBanned);
			data.AddBool(DatabaseManager.fieldDeleted, 	bDeleted);
			
			return data;
		}
		
		
		// -------------------------------------------------------------------------------
		
	}
	
}