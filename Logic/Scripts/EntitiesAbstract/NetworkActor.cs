// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork {

	// =======================================================================================
	// NETWORK ACTOR
	// =======================================================================================
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(NavMeshAgent))]
	[DisallowMultipleComponent]
	public abstract partial class NetworkActor : NetworkEntity
	{
		
	}
 
	// =======================================================================================
	
}
