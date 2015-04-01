using System;
using System.Collections.Generic;

namespace CsvImporter
{
	public class MemoryStreamSource : IStreamSource
	{
		public string GetTableName ()
		{
			return name;
		}

		public string[] GetColumnNames ()
		{
			return columnNames;
		}

		public IEnumerable<string[]> GetStringArrayEnumerable ()
		{
			return rows;
		}

		private string name;
		private string[] columnNames;
		private string[][] rows;

		public MemoryStreamSource (string name, string[] columnNames, string[][] rows)
		{
			this.name = name;
			this.columnNames = columnNames;
			this.rows = rows;
		}
	}
}

