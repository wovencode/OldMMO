// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using UnityEngine;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	//
	// BaseServerBehaviour
	//
	[DisallowMultipleComponent]
	public abstract class BaseServerBehaviour : MonoBehaviour, IServerComponent
	{
	
		//
		// Awake
		//
		void Awake()
		{
#if !OM_SERVER
			Destroy(this.gameObject);
#endif
		}
        
    }
	
}