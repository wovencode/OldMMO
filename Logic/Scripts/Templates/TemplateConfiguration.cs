// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

#if UNITY_EDITOR

using UnityEngine;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// TemplateConfiguration
	// ===================================================================================
	[CreateAssetMenu(menuName="OpenMMO - Config/Configuration", fileName="New Configuration")]
	public partial class TemplateConfiguration : ScriptableObject
	{
		
		public bool isServer = true;
		public bool isClient = true;
		
		protected const string OM_SERVER = "OM_SERVER";
		protected const string OM_CLIENT = "OM_CLIENT";
	
		// -------------------------------------------------------------------------------
		// OnValidate
		// -------------------------------------------------------------------------------

		public void OnValidate()
		{
		
			if (isServer && !isClient)
			{
				EditorTools.RemoveScriptingDefine(OM_CLIENT);
				EditorTools.AddScriptingDefine(OM_SERVER);
			}
			else if (isClient && !isServer)
			{
				EditorTools.RemoveScriptingDefine(OM_SERVER);
				EditorTools.AddScriptingDefine(OM_CLIENT);
			}
			else if (isClient && isServer)
			{
				EditorTools.AddScriptingDefine(OM_CLIENT);
				EditorTools.AddScriptingDefine(OM_SERVER);
			}
		
		}
	
		// -------------------------------------------------------------------------------
		
	}
	
}
#endif	