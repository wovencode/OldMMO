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
	// BaseItem
	// ===================================================================================
	[System.Serializable]
	public partial class BaseItem : BaseNumeric
	{
		
		public TemplateItem template;
		public int slot;
		public int amount;
		public int ammo;
		public int charges;
		public int level;
		
	}
	
}