// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;
using System.Data;
using OpenMMO.Groundwork;
using UnityEngine;

namespace OpenMMO.Groundwork
{
	
	// ===================================================================================
	// BaseDataTable
	// ===================================================================================
	public partial class BaseDataTable : DataTable
	{
		
		// -------------------------------------------------------------------------------
		// Cleanup
		// -------------------------------------------------------------------------------
		public void Cleanup()
		{
			Clear();
			GC.SuppressFinalize(this);
		}
		
		// -------------------------------------------------------------------------------
		// IsEmpty
		// -------------------------------------------------------------------------------
		public bool IsEmpty
		{
			get
			{
				return Rows.Count == 0;
			}
		}
		
		// ===============================================================================
		// GENERIC SETTERS
		// ===============================================================================
		
		// -------------------------------------------------------------------------------
		// SetString
		// -------------------------------------------------------------------------------
		public void AddString(string fieldName, string fieldValue, int _row = 0)
		{
		
			if (!Columns.Contains(fieldName))
				Columns.Add(fieldName, typeof(string));
			
			if (Rows.Count < _row+1)
			{
				DataRow row = NewRow();
				row[fieldName] = fieldValue;
				Rows.Add(row);
			}
			else
			{
				Rows[_row][fieldName] = fieldValue;
			}
			
		}
		
		// -------------------------------------------------------------------------------
		// AddInt
		// -------------------------------------------------------------------------------
		public void AddInt(string fieldName, long fieldValue, int _row = 0)
		{
			if (!Columns.Contains(fieldName))
				Columns.Add(fieldName, typeof(int));
			
			if (Rows.Count < _row+1)
			{
				DataRow row = NewRow();
				row[fieldName] = (int)fieldValue;
				Rows.Add(row);
			}
			else
			{
				Rows[_row][fieldName] = fieldValue;
			}
			
		}
		
		// -------------------------------------------------------------------------------
		// AddDouble
		// -------------------------------------------------------------------------------
		public void AddDouble(string fieldName, double fieldValue, int _row = 0)
		{
			
			if (!Columns.Contains(fieldName))
				Columns.Add(fieldName, typeof(double));
			
			if (Rows.Count < _row+1)
			{
				DataRow row = NewRow();
				row[fieldName] = (double)fieldValue;
				Rows.Add(row);
			}
			else
			{
				Rows[_row][fieldName] = fieldValue;
			}
			
		}
		
		// -------------------------------------------------------------------------------
		// AddBool
		// -------------------------------------------------------------------------------
		public void AddBool(string fieldName, int fieldValue, int _row = 0)
		{
			
			if (!Columns.Contains(fieldName))
				Columns.Add(fieldName, typeof(bool));
			
			if (Rows.Count < _row+1)
			{
				DataRow row = NewRow();
				row[fieldName] = (fieldValue > 0 ? true : false);
				Rows.Add(row);
			}
			else
			{
				Rows[_row][fieldName] = fieldValue;
			}
			
		}
		
		// -------------------------------------------------------------------------------
		// AddBool
		// -------------------------------------------------------------------------------
		public void AddBool(string fieldName, bool fieldValue, int _row = 0)
		{
			
			if (!Columns.Contains(fieldName))
				Columns.Add(fieldName, typeof(bool));
			
			if (Rows.Count < _row+1)
			{
				DataRow row = NewRow();
				row[fieldName] = fieldValue;
				Rows.Add(row);
			}
			else
			{
				Rows[_row][fieldName] = fieldValue;
			}
			
		}
		
		// ===============================================================================
		// GENERIC GETTERS
		// ===============================================================================
		
		// -------------------------------------------------------------------------------
		// GetString
		// -------------------------------------------------------------------------------
		public string GetString(string fieldName, int row = 0)
		{
			return (string)Rows[row][fieldName];
		}
		
		// -------------------------------------------------------------------------------
		// GetInt
		// -------------------------------------------------------------------------------
		public int GetInt(string fieldName, int row = 0)
		{
			return (int)Rows[row][fieldName];
		}
		
		// -------------------------------------------------------------------------------
		// GetLongAsInt
		// -------------------------------------------------------------------------------
		public int GetLongAsInt(string fieldName, int row = 0)
		{
			return Convert.ToInt32((long)Rows[row][fieldName]);
		}
		
		// -------------------------------------------------------------------------------
		// GetLongAsInt
		// -------------------------------------------------------------------------------
		public long GetIntAsLong(string fieldName, int row = 0)
		{
			return Convert.ToInt64((long)Rows[row][fieldName]);
		}
		
		// -------------------------------------------------------------------------------
		// GetBool
		// -------------------------------------------------------------------------------
		public bool GetBool(string fieldName, int row = 0)
		{
			return (bool)Rows[row][fieldName];
		}
		
		// -------------------------------------------------------------------------------
		// GetBoolFromInt
		// -------------------------------------------------------------------------------
		public bool GetBoolFromInt(string fieldName, int row = 0)
		{
			return Convert.ToInt32((long)Rows[row][fieldName]) == 1 ? true : false;
		}
		
		// -------------------------------------------------------------------------------
		// GetBoolAsInt
		// -------------------------------------------------------------------------------
		public int GetBoolAsInt(string fieldName, int row = 0)
		{
			return ((bool)Rows[row][fieldName]) == true ? 1 : 0;
		}
		
		// -------------------------------------------------------------------------------
		// GetDouble
		// -------------------------------------------------------------------------------
		public double GetDouble(string fieldName, int row = 0)
		{
			return (double)Rows[row][fieldName];
		}
		
		// -------------------------------------------------------------------------------
		// GetIdHash
		// -------------------------------------------------------------------------------
		public int GetIdHash(int row = 0)
		{
			string s = (string)Rows[row][DatabaseManager.fieldId];
			return s.GetFNVHashCode();
		}
		
		// -------------------------------------------------------------------------------
		
	}
	
}