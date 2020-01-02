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
	// BaseStatus
	//
	[System.Serializable]
	public partial class BaseStatus : BaseNumeric
	{
		
		public TemplateStatus template;
		public int level;
		public float duration;
		
	}
	
}