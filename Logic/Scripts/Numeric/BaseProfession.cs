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
	// BaseProfession
	//
	[System.Serializable]
	public partial class BaseProfession : BaseNumeric
	{
		
		public TemplateProfession template;
		public LevelBasedInt value;
		
	}
	
}