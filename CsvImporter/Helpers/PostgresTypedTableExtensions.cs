using System;
using System.Linq;


namespace CsvImporter
{
	/// <summary>
	/// Static class for Postgres TypedTable extensions.
	/// </summary>
	public static class PostgresTypedTableExtensions
	{
		/// <summary>
		/// Converts a TypedTable to a table with Postgres-names <see cref="CsvImporter.PostgresNaming"/>.
		/// </summary>
		/// <returns>A TypedTable with Postgres names.</returns>
		/// <param name="table">Table.</param>
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

