using System;
using System.Collections.Generic;

namespace SimpleTable
{
	/// <summary>
	/// Represents a table.
	/// </summary>
	public class Table : List<Row>
	{
		private Dictionary<string, int> columnNameIndex = new Dictionary<string, int>();

		public string[] ColumnNames { get; private set; }

		public Table (string[] columnNames)
		{
			this.ColumnNames = columnNames;

			for (int i = 0; i < columnNames.Length; i++)
			{
				columnNameIndex.Add (columnNames [i], i);
			}
		}

		public void Add (string[] cells)
		{
			this.Add (new Row (cells, columnNameIndex));
		}
	}
}
