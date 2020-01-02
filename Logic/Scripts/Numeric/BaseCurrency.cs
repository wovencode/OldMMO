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
	// BaseCurrency
	//
	[System.Serializable]
	public partial class BaseCurrency : BaseNumeric
	{
		
		public TemplateCurrency template;
		public LevelBasedInt value;
		
	}
	
}