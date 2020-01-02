// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEngine;
using OpenMMO.Groundwork;
using UnityEditor;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// EditorTools
	// ===================================================================================
	[InitializeOnLoad]
	public static partial class EditorTools
	{
		
		// -------------------------------------------------------------------------------
		// AddScriptingDefine
		// -------------------------------------------------------------------------------
       	public static void AddScriptingDefine(string define)
       	{
			BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			string definestring = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
			string[] defines = definestring.Split (';');

			if (ArrayContains(defines, define))
				return;

			PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, (definestring + ";" + define));
			
			Debug.Log("<b>" + define + "</b> added to Scripting Define Symbols for selected build target (" + EditorUserBuildSettings.activeBuildTarget.ToString() + ").");
		}
		
		// -------------------------------------------------------------------------------
		// RemoveScriptingDefine
		// -------------------------------------------------------------------------------
		public static void RemoveScriptingDefine(string define)
		{
       	
       		BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			string definestring = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
			string[] defines = definestring.Split (';');

			defines = RemoveFromArray(defines, define);

			definestring = string.Join(";", defines);

			PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, (definestring));
       		
       		Debug.Log("<b>" + define + "</b> removed from Scripting Define Symbols for selected build target (" + EditorUserBuildSettings.activeBuildTarget.ToString() + ").");
       	}
       	
       	// -------------------------------------------------------------------------------
		// ArrayContains
		// -------------------------------------------------------------------------------
       	static bool ArrayContains(string[] defines, string define) {
			foreach (string def in defines) {
				if (def == define)
					return true;
			}
			return false;
		}
		
       	// -------------------------------------------------------------------------------
		// RemoveFromArray
		// -------------------------------------------------------------------------------
		static string[] RemoveFromArray(string[] defines, string define) {
			return defines.Where(x => x != define).ToArray();
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}
#endif