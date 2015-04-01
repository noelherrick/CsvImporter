using System;
using System.Linq;
using System.Collections.Generic;
using SimpleTable;
using CsvImporter.SqlTypes;

namespace CsvImporter
{
	public interface IRowStream
	{
		string[] ColumnNames { get; }
		SqlType[] DataTypes { get; }
		string Name {get;}
		IEnumerable<Header> Headers {get;}
		IEnumerable<Row> GetRowEnumerable ();
	}

	public class RowStream : IRowStream
	{
		private Dictionary<string, int> columnNameIndex = new Dictionary<string, int>();

		public string[] ColumnNames { get; private set; }

		public SqlType[] DataTypes { get; private set; }

		public string Name { get; private set; }

		/// <summary>
		/// Gets the headers. Each header has a name, a type, and a column position number.
		/// </summary>
		/// <value>The headers.</value>
		public IEnumerable<Header> Headers
		{
			get
			{
				int index = 0;
				return ColumnNames.Zip (DataTypes, (x, y) => new Header() { Name = x, Type = y, Index = index++ });
			}
		}

		private string[][] rowBuffer;
		private int bufferSize;
		private int bufferIndex = 0;
		private int bufferLength;
		private bool finished = false;
		private IEnumerator<string[]> enm;

		public RowStream (IStreamSource source, int bufferSize = 10000)
		{
			Name = source.GetTableName ();

			this.bufferSize = bufferSize;
			this.bufferLength = bufferSize;

			rowBuffer = new string [bufferSize][];

			var rawRows = source.GetStringArrayEnumerable ();

			enm = rawRows.GetEnumerator ();

			fillBuffer ();

			ColumnNames = source.GetColumnNames();

			for (int i = 0; i < ColumnNames.Length; i++)
			{
				columnNameIndex.Add (ColumnNames [i], i);
			}
		}

		private void fillBuffer ()
		{
			bufferIndex = 0;

			for (int i = 0; i < bufferSize; i++)
			{
				if (enm.MoveNext ()) {
					var rawRow = enm.Current;

					// Check the types for this row
					var rowSqlTypes = rawRow.Select (c => TypeInferer.InferCellDataType (c));

					// If this is the first row, set the sqlTypes array to rowTypes,
					// otherwise decide which type is the broadest
					var sqlTypesEnum = DataTypes == null ? rowSqlTypes : rowSqlTypes.Zip (DataTypes, (x, y) => x.GetBroadestType (y));

					DataTypes = sqlTypesEnum.ToArray ();

					rowBuffer [i] = rawRow;
				} else {
					bufferLength = i;
					finished = true;
					break;
				}
			}
		}

		public IEnumerable<Row> GetRowEnumerable ()
		{
			while (!finished || bufferIndex < bufferLength)
			{
				if (bufferIndex < bufferLength) {
					yield return new Row (rowBuffer [bufferIndex++], columnNameIndex);
				}
				else if (!finished)
				{
					fillBuffer ();
				}
			}

			yield break;
		}
	}
}

