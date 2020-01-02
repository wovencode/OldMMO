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
	// SStatistic
	// ===================================================================================
	[Serializable]
	public partial struct SStatistic
	{
		
		public int nId;
		public short nValue;
		
		[NonSerialized] string _name;
		[NonSerialized] string _title;
		[NonSerialized] string _description;
		
		// -------------------------------------------------------------------------------
		// SStatistic (Constructor) 
		// -------------------------------------------------------------------------------

	}
	
}