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
	
	//
	// BaseEnergy
	//
	[System.Serializable]
	public partial class BaseEnergy : BaseNumeric
	{
		
		public TemplateEnergy template;
		public LevelBasedInt value;
		
	}
	
}