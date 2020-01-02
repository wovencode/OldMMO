// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using UnityEngine;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	//
	// BaseClientBehaviour
	//
	[DisallowMultipleComponent]
	public abstract class BaseClientBehaviour : MonoBehaviour, IClientComponent
	{
	
		//
		// Awake
		//
		void Awake()
		{
#if !OM_CLIENT
			Destroy(this.gameObject);
#endif
		}
	 
    }
	
}