using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleTable
{
	public class Row : IEnumerable<string>
	{
		public IEnumerator<string> GetEnumerator ()
		{
			return cells.AsEnumerable ().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return cells.GetEnumerator();
		}

		private string[] cells;
		private Dictionary<string, int> columnNameIndex;

		public Row (string[] cells, Dictionary<string, int> columnNameIndex)
		{
			this.cells = cells;
			this.columnNameIndex = columnNameIndex;
		}

		public string GetCell (string columnName)
		{
			int index = columnNameIndex [columnName];
			return cells [index];
		}

		public string GetCell (int index)
		{
			return cells [index];
		}

		public IEnumerable<Cell> GetCells ()
		{
			foreach (var name in columnNameIndex)
			{
				yield return new Cell (name.Value, name.Key, cells[name.Value]);
			}

			yield break;
		}

		public string this[int index]
		{
			get
			{
				return GetCell (index);
			}
		}
	}

	public class Cell
	{
		public int Index { get; private set;}
		public string ColumnName { get; private set;}
		public string Value { get; private set;}

		public Cell (int Index, string ColumnName, string Value)
		{
			this.Index = Index;
			this.ColumnName = ColumnName;
			this.Value = Value;
		}
	}
}

