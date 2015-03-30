using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleTable
{
	/// <summary>
	/// Represents a row. Allows retrieval by Enumerator, column index, and column name
	/// </summary>
	public class Row : IEnumerable<string>
	{
		private string[] cells;
		private Dictionary<string, int> columnNameIndex;

		public Row (string[] cells, Dictionary<string, int> columnNameIndex)
		{
			this.cells = cells;
			this.columnNameIndex = columnNameIndex;
		}

		/// <summary>
		/// Gets the cell by column name. Will return exception if the column name is not present.
		/// </summary>
		/// <returns>The named cell.</returns>
		/// <param name="columnName">Column name.</param>
		public string GetCell (string columnName)
		{
			int index = columnNameIndex [columnName];
			return cells [index];
		}

		/// <summary>
		/// Gets the cell by index. Will return exception if the index is out of range
		/// </summary>
		/// <returns>The indicated cell.</returns>
		/// <param name="index">Index.</param>
		public string GetCell (int index)
		{
			return cells [index];
		}

		/// <summary>
		/// Gets the cells as an IEnumerable<Cell>
		/// </summary>
		/// <returns>The cells.</returns>
		public IEnumerable<Cell> GetCells ()
		{
			foreach (var name in columnNameIndex)
			{
				yield return new Cell (name.Value, name.Key, cells[name.Value]);
			}

			yield break;
		}

		/// <summary>
		/// Gets the <see cref="SimpleTable.Row"/> at the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public string this[int index]
		{
			get
			{
				return GetCell (index);
			}
		}

		public IEnumerator<string> GetEnumerator ()
		{
			return cells.AsEnumerable ().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return cells.GetEnumerator();
		}
	}

	/// <summary>
	/// Represents a single cell inside a row.
	/// </summary>
	public class Cell
	{
		/// <summary>
		/// Gets the index of the column
		/// </summary>
		/// <value>The index.</value>
		public int Index { get; private set;}

		/// <summary>
		/// Gets the name of the column.
		/// </summary>
		/// <value>The name of the column.</value>
		public string ColumnName { get; private set;}

		/// <summary>
		/// Gets the cell value.
		/// </summary>
		/// <value>The value.</value>
		public string Value { get; private set;}

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleTable.Cell"/> class.
		/// </summary>
		/// <param name="Index">Index.</param>
		/// <param name="ColumnName">Column name.</param>
		/// <param name="Value">Value.</param>
		public Cell (int Index, string ColumnName, string Value)
		{
			this.Index = Index;
			this.ColumnName = ColumnName;
			this.Value = Value;
		}
	}
}

