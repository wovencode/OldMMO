// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Collections;
using UnityEngine;
using OpenMMO.Groundwork;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// Tools
	// ===================================================================================
	public static partial class Tools
	{

		// -----------------------------------------------------------------------------------
		// IntArrayToString
		// -----------------------------------------------------------------------------------
		public static string IntArrayToString(int[] array, char delimiter = ';') {
			if (array == null || array.Length == 0) return null;
			string arrayString = "";
			for (int i = 0; i < array.Length; i++) {
				arrayString += array[i].ToString();
				if (i < array.Length-1)
					arrayString += delimiter;
			}
			return arrayString;
		}
		
		// -----------------------------------------------------------------------------------
		// StringArrayToString
		// -----------------------------------------------------------------------------------
		public static string StringArrayToString(string[] array, char delimiter = ';') {
			if (array == null || array.Length == 0) return null;
			string arrayString = "";
			for (int i = 0; i < array.Length; i++) {
				arrayString += array[i].ToString();
				if (i < array.Length-1)
					arrayString += delimiter;
			}
			return arrayString;
		}
	
		// -----------------------------------------------------------------------------------
		// IntStringToArray
		// -----------------------------------------------------------------------------------
		public static int[] IntStringToArray(string array, char delimiter = ';') {
			if (String.IsNullOrWhiteSpace(array)) return null;
			string[] tokens = array.Split(delimiter);
			int[] arrayInt = Array.ConvertAll<string, int>(tokens, int.Parse);
			return arrayInt;
		}	
	
		// -------------------------------------------------------------------------------
		// HashPassword
		// -------------------------------------------------------------------------------
		public static string HashPassword(string text)  
		{  
			using(var sha256 = SHA256.Create())  
			{  
				var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));  
				return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();  
			}  
		}  
		
		// -------------------------------------------------------------------------------
		// GetDeviceId
		// -------------------------------------------------------------------------------
       	public static string GetDeviceId
       	{
       		get
       		{
       			return SystemInfo.deviceUniqueIdentifier;
       		}
       	}
       	
		// -------------------------------------------------------------------------------
		// GetVersion
		// -------------------------------------------------------------------------------
       	public static string GetVersion
       	{
       		get
       		{
       			return Application.version;
       		}
       	}
       	
		// -------------------------------------------------------------------------------
		// Quit
		// -------------------------------------------------------------------------------
   		public static void Quit()
		{
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}