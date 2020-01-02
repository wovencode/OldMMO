// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using UnityEngine;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	//
	// TemplateBase
	//
	[CreateAssetMenu(menuName="OpenMMO - Item/General Item", fileName="New General Item")]
	public partial class TemplateItem : TemplateBase
	{
		
		public int slot;
		public int amount;
		public int ammo;
		public int charges;
		public int level;
		

	}
	
}