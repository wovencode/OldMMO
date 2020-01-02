// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// DataManager
	/// <summary>
	/// This neutral Manager class is present on both the client and the server and acts
	/// as a library to store all Scriptable Object templates using dictionaries. It's
	/// only purpose is to provide quick, cross-project access to that data. It is not
	/// linked to or dependant of any other Manager.
	/// </summary>
	// ===================================================================================
	[DisallowMultipleComponent]
	public partial class DataManager : BaseNeutralBehaviour
	{
		
		public static Dictionary<int, TemplateAlignment>		dictAlignment;
		public static Dictionary<int, TemplateAspect> 			dictAspect;
		public static Dictionary<int, TemplateAttribute> 		dictAttribute;
		public static Dictionary<int, TemplateBlueprint> 		dictBlueprint;
		public static Dictionary<int, TemplateCurrency> 		dictCurrency;
		public static Dictionary<int, TemplateEnergy> 			dictEnergy;
		public static Dictionary<int, TemplateItem> 			dictItem;
		public static Dictionary<int, TemplateProfession> 		dictProfession;
		public static Dictionary<int, TemplateSkill> 			dictSkill;
		public static Dictionary<int, TemplateStatus> 			dictStatus;
		public static Dictionary<int, TemplateType> 			dictType;
		
		// -------------------------------------------------------------------------------
		// Awake
		// -------------------------------------------------------------------------------
		void Awake()
		{
			dictAlignment	= Resources.LoadAll<TemplateAlignment>("").ToDictionary(template => template.GetId, template => template);
			dictAspect		= Resources.LoadAll<TemplateAspect>("").ToDictionary(template => template.GetId, template => template);
			dictAttribute	= Resources.LoadAll<TemplateAttribute>("").ToDictionary(template => template.GetId, template => template);
			dictBlueprint	= Resources.LoadAll<TemplateBlueprint>("").ToDictionary(template => template.GetId, template => template);
			dictCurrency	= Resources.LoadAll<TemplateCurrency>("").ToDictionary(template => template.GetId, template => template);
			dictEnergy		= Resources.LoadAll<TemplateEnergy>("").ToDictionary(template => template.GetId, template => template);
			dictItem		= Resources.LoadAll<TemplateItem>("").ToDictionary(template => template.GetId, template => template);
			dictProfession	= Resources.LoadAll<TemplateProfession>("").ToDictionary(template => template.GetId, template => template);
			dictSkill		= Resources.LoadAll<TemplateSkill>("").ToDictionary(template => template.GetId, template => template);
			dictStatus		= Resources.LoadAll<TemplateStatus>("").ToDictionary(template => template.GetId, template => template);
			dictType		= Resources.LoadAll<TemplateType>("").ToDictionary(template => template.GetId, template => template);
		}
		
	}
	
}