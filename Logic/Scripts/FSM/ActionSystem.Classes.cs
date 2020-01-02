// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork {

	// =======================================================================================
	// 
	// =======================================================================================
	public enum ActionTransition
	{
		NullTransition = 0, // Use this transition to represent a non-existing transition in your system
		Idle,
		Dead,
		Move,
		Trade,
		Craft,
		Cast,
		Stun
	}
 
	// =======================================================================================
	// 
	// =======================================================================================
	public enum ActionStateID
	{
		NullStateID = 0, // Use this ID to represent a non-existing State in your system	
		Idle,
		Dead,
		Move,
		Trade,
		Craft,
		Cast,
		Stun
	}
}
