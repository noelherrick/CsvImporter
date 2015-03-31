using System;
using System.Linq;
using System.Data.Common;

namespace CsvImporter
{
	public abstract class DbAdapter : IDbAdapter
	{
		public abstract DbConnection GetConnection (DbConfiguration dbConfig);

		public string GenerateTruncateStatement (TypedTable table)
		{
			return string.Format ("truncate table {0};", EscapeName(table.Name));
		}

		public string GenerateDdl (TypedTable table) {
			string colList = string.Join (",", table.Headers.ToList().Select (x => EscapeName(x.Name) + " " + GetTypeName(x.Type)));

			return string.Format ("create table {0} ({1})", EscapeName(table.Name), colList);
		}

		public string GenerateInsertSql (TypedTable table)
		{
			string colList = string.Join (",", table.Headers.ToList().Select (x => EscapeName(x.Name)));
			string paramList = string.Join (",", table.Headers.ToList().Select (x => "@" + x.Index));

			return string.Format ("insert into {0} ({1}) values ({2})", EscapeName(table.Name), colList, paramList);
		}

		public TypedTable ConvertTable (TypedTable table)
		{
			var convertedColumnNames = table.ColumnNames.Select (x => GetEngineSpecificName(x)).ToArray();
			var convertedColumnTypes = table.DataTypes.ToArray();
			var convertedTable = new TypedTable (convertedColumnNames, convertedColumnTypes);

			convertedTable.Name = GetEngineSpecificName(table.Name);

			convertedTable.AddRange (table);

			return convertedTable;
		}

		public abstract string GetTypeName (CsvImporter.SqlTypes.SqlType type);
		public abstract string GetEngineSpecificName (string name);
		public abstract string EscapeName (string name);
	}
}

