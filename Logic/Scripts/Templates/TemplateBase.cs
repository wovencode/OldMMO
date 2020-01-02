// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using UnityEngine;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// TemplateBase
	// ===================================================================================
	public abstract class TemplateBase : ScriptableObject
	{
		
		[Tooltip("Name, as displayed to the public. Not unique.")]
		public string title;
		[Tooltip("Description, as displayed to the public (tooltips etc.).")]
		[TextArea(5,5)]public string description;
		[Tooltip("Icon, as displayed to the public.")]
		public Sprite icon;
		
		protected int id = 0;
		protected string _tooltip;
		
		// -------------------------------------------------------------------------------
		// GetId
		// -------------------------------------------------------------------------------
		/// <summary>
		/// We access Templates based on Scriptable Objects by a Id number instead of the
		/// template name. This implementation is more efficient, also note that we cannot
		/// store a reference to a Scriptable Object in a struct, so we need a simple type
		/// to access it instead.
		/// </summary>
		public int GetId
		{
			get {
				// We generate a stable hash code once the Id of this template is requested
				// and cache it. Note that the name of the scriptable object is used and
				// not the title of the template. As the template name does not change during
				// runtime. Do not store the Id permanently (eg. in the database) as it might
				// change during sessions.
				if (id == 0)
					id = name.GetFNVHashCode();
				return id;
			}
		}
		
		// -------------------------------------------------------------------------------
		// GenerateToolTip
		// -------------------------------------------------------------------------------
		public string GenerateToolTip
		{
			get
			{
			
				if (String.IsNullOrWhiteSpace(_tooltip))
			 		_tooltip = "<b>" + title + "</b>\n" + description;
			
				return _tooltip;
			
			}
		}

	}
	
}