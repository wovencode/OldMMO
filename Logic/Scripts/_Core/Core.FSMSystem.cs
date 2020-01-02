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
	// FSMSystem
	// =======================================================================================
	[DisallowMultipleComponent]
	public class FSMSystem : MonoBehaviour
	{
		private List<FSMState> states;
 
		// The only way one can change the state of the FSM is by performing a transition
		// Don't change the CurrentState directly
		private FSMStateID currentStateID;
		
	    // -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------
		public int CurrentStateID {
			get { return (int)currentStateID; }
		}
		
    	// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------
		private FSMState currentState;
		/*
		public int CurrentState {
			get { return currentState.ID; }
			set { currentState.ID = (FSMStateID)value; }
		}*/
 
		// -----------------------------------------------------------------------------------
		// 
		// ----------------------------------------------------------------------------------- 
		public FSMSystem()
		{
			states = new List<FSMState>();
		}
 
		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------
		public void AddState(FSMState s)
		{
			// Check for Null reference before deleting
			if (s == null)
			{
				Debug.LogError("FSM ERROR: Null reference is not allowed");
			}
 
			// First State inserted is also the Initial state,
			//   the state the machine is in when the simulation begins
			if (states.Count == 0)
			{
				states.Add(s);
				currentState = s;
				currentState.ID = s.ID;
				return;
			}
 
			// Add the state to the List if it's not inside it
			foreach (FSMState state in states)
			{
				if (state.ID == s.ID)
				{
					Debug.LogError("FSM ERROR: Impossible to add state " + s.ID.ToString() + 
								   " because state has already been added");
					return;
				}
			}
			states.Add(s);
		}
 
		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------
		public void DeleteState(FSMStateID id)
		{
			// Check for NullState before deleting
			if (id == FSMStateID.NullStateID)
			{
				Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
				return;
			}
 
			// Search the List and delete the state if it's inside it
			foreach (FSMState state in states)
			{
				if (state.state == id)
				{
					states.Remove(state);
					return;
				}
			}
			Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() + 
						   ". It was not on the list of states");
		}
 
		// -----------------------------------------------------------------------------------
		// 
		// -----------------------------------------------------------------------------------
		public void PerformTransition(FSMTransition trans)
		{
			// Check for NullTransition before changing the current state
			if (trans == FSMTransition.NullTransition)
			{
				Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
				return;
			}
 
			// Check if the currentState has the transition passed as argument
			FSMStateID id = currentState.GetOutputState(trans);
			if (id == FSMStateID.NullStateID)
			{
				Debug.LogError("FSM ERROR: State " + currentStateID.ToString() +  " does not have a target state " + 
							   " for transition " + trans.ToString());
				return;
			}
 
			// Update the currentStateID and currentState		
			currentStateID = id;
			foreach (FSMState state in states)
			{
				if (state.state == currentStateID)
				{
					// Do the post processing of the state before setting the new one
					currentState.DoBeforeLeaving();
 
					currentState = state;
 
					// Reset the state to its desired condition before it can reason or act
					currentState.DoBeforeEntering();
					break;
				}
			}
 
		}
 		
 		// -----------------------------------------------------------------------------------
 		
	}
	
	// -----------------------------------------------------------------------------------
}

// =======================================================================================