// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// SSkill
	// ===================================================================================
	[Serializable]
	public partial struct SSkill
	{
		
		public int nId;
		public int nLevel;
		public double dTimer;

		[NonSerialized] string _name;
		[NonSerialized] string _title;
		[NonSerialized] string _description;
		
		// -------------------------------------------------------------------------------
		// SSkill (Constructor) 
		// -------------------------------------------------------------------------------
		public SSkill(int _nId, int _nLevel, double _fCooldown)
		{
			nId 			= _nId;	
			nLevel 			= _nLevel;
			dTimer			= NetworkTime.time + _fCooldown;
			_name			= "";
			_title			= "";
			_description 	= "";
		}
		
		// -------------------------------------------------------------------------------
		// Template
		// -------------------------------------------------------------------------------
		public TemplateSkill template
		{
			get
			{
				TemplateSkill tmpl;
				if (DataManager.dictSkill.TryGetValue(nId, out tmpl))
				{
					return tmpl;
				}
				else
				{
					Debug.LogWarning("[Skipping] Skill Template not found: '"+nId.ToString()+"'.");
					return tmpl;
				}
			}
		}
		
		// -------------------------------------------------------------------------------
		// name
		// -------------------------------------------------------------------------------
		public string name
		{
			get
			{
				if (String.IsNullOrWhiteSpace(_name))
					return	template.name;
				else
					return _name;
			}
		}
		
		// -------------------------------------------------------------------------------
		// title
		// -------------------------------------------------------------------------------
		public string title
		{
			get
			{
				if (String.IsNullOrWhiteSpace(_title))
					return	template.title;
				else
					return _title;
			}
		}
		
		// -------------------------------------------------------------------------------
		// description
		// -------------------------------------------------------------------------------
		public string description
		{
			get
			{
				if (String.IsNullOrWhiteSpace(_description))
					return	template.description;
				else
					return _description;
			}
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}