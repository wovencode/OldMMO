// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Mirror;
using OpenMMO.Groundwork;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// CurrencySubsystem
	// ===================================================================================
	[System.Serializable]
	[NetworkSettings(sendInterval = 1f)]
	[DisallowMultipleComponent]
	public partial class CurrencySubsystem : BaseSystem
	{
		
		public BaseCurrency[] defaultCurrencies;
		
		public SyncListCurrency syncCurrencies = new SyncListCurrency();
		
		// -------------------------------------------------------------------------------
		// Init
		// -------------------------------------------------------------------------------
		public void Init(GameObject _parent = null, int level = 1)
		{
			
			base.Init(_parent);
			
			syncCurrencies.Clear();
			
			foreach (BaseCurrency currency in defaultCurrencies)
			{
				if (currency.template != null) {
					SCurrency sCurrency = new SCurrency(currency.template.GetId, currency.value.Get(level));
					syncCurrencies.Add(sCurrency);
				}
			}
		}

		// -------------------------------------------------------------------------------
		// Overwrite
		// -------------------------------------------------------------------------------
		public override void Overwrite(List<TemplateAspect> listAspects)
		{
		
		}
		
		// -------------------------------------------------------------------------------
		// Load 
		// -------------------------------------------------------------------------------
		public override void Load(BaseDataTable data)
		{
			syncCurrencies.Clear();
			
			for (int i = 0; i < data.Rows.Count; ++i)
			{
				TemplateCurrency tmpl;
				
				if (DataManager.dictCurrency.TryGetValue(data.GetIdHash(i), out tmpl))
				{
					SCurrency sCurrency = new SCurrency(tmpl.GetId, data.GetLongAsInt(DatabaseManager.fieldValue, i));
					syncCurrencies.Add(sCurrency);
				}
				else
				{
					Debug.LogWarning("Skipped template '"+data.GetString(DatabaseManager.fieldName)+"' as it was not found in Library.");
				}
			
			}
			
			data.Cleanup();
		}
		
		// -------------------------------------------------------------------------------
		// Save
		// -------------------------------------------------------------------------------
		public override BaseDataTable Save()
		{
			
			BaseDataTable data = new BaseDataTable();
			
			for (int i = 0; i < syncCurrencies.Count; ++i)
			{
				data.AddString(DatabaseManager.fieldName, 	parent.name, 				i);
				data.AddString(DatabaseManager.fieldId, 	syncCurrencies[i].name, 	i);
				data.AddInt(DatabaseManager.fieldValue, 	syncCurrencies[i].nValue, 	i);
			}
			
			return data;
		
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}