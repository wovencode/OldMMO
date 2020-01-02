// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using UnityEngine;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	//
	// TemplateAspect
	//
	[CreateAssetMenu(menuName="OpenMMO - Aspect/Aspect", fileName="New Aspect")]
	public partial class TemplateAspect : TemplateBase
	{
		
		public string family;
		
		public GameObject actorPrefab;
		
		public SceneLocation[] startingLocations;
		
		public BaseAlignment[]	alignmentModifiers;
		public BaseAttribute[] 	attributeModifiers;
		public BaseEnergy[] 	energyModifiers;
		public BaseCurrency[] 	currencyModifiers;
		public BaseElement[] 	elementModifiers;
		public BaseProfession[] professionModifiers;
		public BaseSkill[] 		skillModifiers;
		public BaseItem[] 		itemModifiers;

	}
	
}