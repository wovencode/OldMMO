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
	// BaseAttribute
	// ===================================================================================
	[System.Serializable]
	public partial class BaseAttribute : BaseNumeric
	{
		
		public TemplateAttribute template;
		public LevelBasedInt value;
		
	}
	
}