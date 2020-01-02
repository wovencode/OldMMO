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
	// SProfession
	// ===================================================================================
	[Serializable]
	public partial struct SProfession
	{
		
		public int nId;
		public int nValue;

		[NonSerialized] string _name;
		[NonSerialized] string _title;
		[NonSerialized] string _description;
		
		// -------------------------------------------------------------------------------
		// SProfession (Constructor) 
		// -------------------------------------------------------------------------------
		public SProfession(int _nId, int _nValue)
		{
			nId 			= _nId;	
			nValue 			= _nValue;
			_name			= "";
			_title			= "";
			_description 	= "";
		}
		
		// -------------------------------------------------------------------------------
		// Template
		// -------------------------------------------------------------------------------
		public TemplateProfession template
		{
			get
			{
				TemplateProfession tmpl;
				if (DataManager.dictProfession.TryGetValue(nId, out tmpl))
				{
					return tmpl;
				}
				else
				{
					Debug.LogWarning("[Skipping] Profession Template not found: '"+nId.ToString()+"'.");
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