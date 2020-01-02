// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// ISubsystem
	// ===================================================================================
	public interface ISubsystem
	{
		
		void Init(GameObject _parent = null);
		void Overwrite(List<TemplateAspect> listAspects);
		
		void Load(BaseDataTable data);
		BaseDataTable Save();

	}
	
	// ===================================================================================
	
}