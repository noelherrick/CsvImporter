using System;
using SimpleTable;
using System.Collections.Generic;

namespace CsvImporter
{
	public interface IStreamSource
	{
		//void InitializeStream ();
		string GetTableName ();
		string[] GetColumnNames ();
		IEnumerable<string[]> GetStringArrayEnumerable ();
	}
}

