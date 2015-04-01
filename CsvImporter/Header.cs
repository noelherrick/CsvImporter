using System;
using CsvImporter.SqlTypes;

namespace CsvImporter
{
	/// <summary>
	/// Data object for headers.
	/// </summary>
	public class Header
	{
		public string Name  { get; set; }
		public SqlType Type { get; set; }
		public int Index { get; set; }
	}
}

