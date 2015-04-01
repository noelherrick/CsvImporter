using System;
using System.Linq;
using System.Collections.Generic;
using CsvImporter.SqlTypes;
using SimpleTable;

namespace CsvImporter
{
	public class MemoryRowStream : IRowStream
	{
		public IEnumerable<Row> GetRowEnumerable ()
		{
			return rows;
		}

		public string[] ColumnNames {
			get {
				return columnNames;
			}
		}

		public SqlType[] DataTypes {
			get {
				return columnTypes;
			}
		}

		public string Name {
			get {
				return name;
			}
		}

		public IEnumerable<Header> Headers {
			get {
				int index = 0;
				return ColumnNames.Zip (DataTypes, (x, y) => new Header() { Name = x, Type = y, Index = index++ });
			}
		}

		private string name;
		private string[] columnNames;
		private SqlType[] columnTypes;
		private Row[] rows;

		public MemoryRowStream (string name, string[] columnNames, SqlType[] columnTypes)
		{
			this.name = name;
			this.columnNames = columnNames;
			this.columnTypes = columnTypes;
			rows = new Row[0];
		}


	}
}

