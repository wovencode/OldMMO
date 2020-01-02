// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using UnityEngine;
using System;
using System.Text.RegularExpressions;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// Extensions
	// ===================================================================================
	public static partial class Extensions
	{
		
		// -------------------------------------------------------------------------------
		// GetHashCode
		// -------------------------------------------------------------------------------
		/// <summary>
		/// Default implementation of string.GetHashCode is not consistent on different
		/// platforms (x32/x64) and frameworks. FNV-1a - (Fowler/Noll/Vo) is a fast,
    	/// consistent, non-cryptographic hash algorithm with good dispersion.
    	/// (see http://isthe.com/chongo/tech/comp/fnv/#FNV-1a)
    	/// </summary>
		public static int GetFNVHashCode(this string sText)
		{
			if (sText == null)
				return 0;
			
			int nLength = sText.Length;
			int nHash = nLength;
			
			for (int i = 0; i != nLength; ++i)
				nHash = (nHash ^ sText[i]) * 16777619;
				
			return nHash;
		}
		
		// -------------------------------------------------------------------------------
		// validateEmail
		// -------------------------------------------------------------------------------
		public static bool validateEmail(this string sText, int minLength = 4, int maxLength = 255) {
			return (
				!String.IsNullOrWhiteSpace(sText) &&
				sText.Length >= minLength &&
				sText.Length <= maxLength &&
				Regex.IsMatch(sText, @"^[a-zA-Z0-9_.@-]+$")
				);
		}
		
		// -------------------------------------------------------------------------------
		// validateText
		// -------------------------------------------------------------------------------
		public static bool validateText(this string sText, int maxLength = 1024) {
			return (
				!String.IsNullOrWhiteSpace(sText) &&
				sText.Length >= 1 &&
				sText.Length <= maxLength &&
				Regex.IsMatch(sText, @"^[a-zA-Z0-9_]+$")
				);
		}

		// -------------------------------------------------------------------------------
		// validateName
		// -------------------------------------------------------------------------------
		public static bool validateName(this string sText, int minLength = 4, int maxLength = 255) {
			return (
				!String.IsNullOrWhiteSpace(sText) &&
				sText.Length >= minLength &&
				sText.Length <= maxLength &&
				Regex.IsMatch(sText, @"^[a-zA-Z0-9_]+$")
				);
		}

		// -------------------------------------------------------------------------------
		// validatePassword
		// -------------------------------------------------------------------------------
		public static bool validatePassword(this string sText, int minLength = 4, int maxLength = 255) {
			return (
				!String.IsNullOrWhiteSpace(sText) &&
				sText.Length >= minLength &&
				sText.Length <= maxLength &&
				Regex.IsMatch(sText, @"^[a-zA-Z0-9_]+$")
				);
		}
			
		// -------------------------------------------------------------------------------
		
	}
	
}