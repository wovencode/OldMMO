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
	// FSMState
	// =======================================================================================
	public abstract class FSMState
	{

		protected Dictionary<FSMTransition, FSMStateID> map = new Dictionary<FSMTransition, FSMStateID>();
		protected FSMStateID FSMStateID;
	
		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------
		public virtual FSMStateID state {
			get { return FSMStateID; }
			set { FSMStateID = value; }
		}
	
		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------
		public virtual int ID {
			get { return (int)FSMStateID; }
			set { FSMStateID = (FSMStateID)value; }
		}

		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------
		public void AddTransition(FSMTransition trans, FSMStateID id)
		{
			// Check if anyone of the args is invalid
			if (trans == FSMTransition.NullTransition)
			{
				Debug.LogError("FSMState ERROR: NullTransition is not allowed for a real FSMTransition");
				return;
			}
 
			if (id == FSMStateID.NullStateID)
			{
				Debug.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
				return;
			}
 
			// Since this is a Deterministic FSM,
			//   check if the current FSMTransition was already inside the map
			if (map.ContainsKey(trans))
			{
				Debug.LogError("FSMState ERROR: State " + FSMStateID.ToString() + " already has FSMTransition " + trans.ToString() + 
							   "Impossible to assign to another state");
				return;
			}
 
			map.Add(trans, id);
		}
		
		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------
		/// <summary>
		/// This method deletes a pair FSMTransition-state from this state's map.
		/// If the FSMTransition was not inside the state's map, an ERROR message is printed.
		/// </summary>
		public void DeleteTransition(FSMTransition trans)
		{
			// Check for NullTransition
			if (trans == FSMTransition.NullTransition)
			{
				Debug.LogError("FSMState ERROR: NullTransition is not allowed");
				return;
			}
 
			// Check if the pair is inside the map before deleting
			if (map.ContainsKey(trans))
			{
				map.Remove(trans);
				return;
			}
			Debug.LogError("FSMState ERROR: FSMTransition " + trans.ToString() + " passed to " + FSMStateID.ToString() + 
						   " was not on the state's FSMTransition list");
		}
		
		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------
		/// <summary>
		/// This method returns the new state the FSM should be if
		///    this state receives a FSMTransition and 
		/// </summary>
		public FSMStateID GetOutputState(FSMTransition trans)
		{
			// Check if the map has this FSMTransition
			if (map.ContainsKey(trans))
			{
				return map[trans];
			}
			return FSMStateID.NullStateID;
		}
 		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------

		/// <summary>
		/// This method is used to set up the State condition before entering it.
		/// It is called automatically by the FSMSystem class before assigning it
		/// to the current state.
		/// </summary>
		public virtual void DoBeforeEntering() { }
 		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------

		/// <summary>
		/// This method is used to make anything necessary, as reseting variables
		/// before the FSMSystem changes to another one. It is called automatically
		/// by the FSMSystem before changing to a new state.
		/// </summary>
		public virtual void DoBeforeLeaving() { } 
 		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------

		/// <summary>
		/// This method decides if the state should FSMTransition to another on its list
		/// NPC is a reference to the object that is controlled by this class
		/// </summary>
		public abstract void Reason(GameObject player, GameObject npc);
 		
 		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------

		/// <summary>
		/// This method controls the behavior of the NPC in the game World.
		/// Every action, movement or communication the NPC does should be placed here
		/// NPC is a reference to the object that is controlled by this class
		/// </summary>
		public abstract void Act(GameObject player, GameObject npc);
 
	}
	
	// -----------------------------------------------------------------------------------
	
}

// =======================================================================================
