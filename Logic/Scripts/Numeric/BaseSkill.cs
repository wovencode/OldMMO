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
	// BaseSkill
	//
	[System.Serializable]
	public partial class BaseSkill : BaseNumeric
	{
		
		public TemplateSkill template;
		public int level;
		
		
	}
	
}