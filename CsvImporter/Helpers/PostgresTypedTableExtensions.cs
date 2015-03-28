using System;
using System.Linq;


namespace CsvImporter
{
	public static class PostgresTypedTableExtensions
	{ 
		public static TypedTable AsPostgresTable (this TypedTable table)
		{
			var pgColumnNames = table.ColumnNames.Select (x => PostgresNaming.ToPostgresName(x)).ToArray();
			var pgColumnTypes = table.DataTypes.ToArray();
			var pgTableName = PostgresNaming.ToPostgresName(table.Name);

			var pgTable = new TypedTable (pgColumnNames, pgColumnTypes);
			pgTable.Name = pgTableName;

			pgTable.AddRange (table);

			return pgTable;
		}
	}
}

