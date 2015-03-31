using System;
using System.Linq;
using CsvImporter.SqlTypes;

namespace CsvImporter
{
	/// <summary>
	/// The Postgres destination. Creates or truncates tables and inserts data.
	/// </summary>
    public class DbDestination
    {
        private DestinationConfiguration config;
        private DbConfiguration dbConfig;

		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.DbDestination"/> class.
		/// </summary>
		/// <param name="config">Config.</param>
		/// <param name="pgConfig">Postgres-specific config.</param>
		public DbDestination (DestinationConfiguration config, DbConfiguration dbConfig)
        {
            this.config = config;
			this.dbConfig = dbConfig;
        }

		/// <summary>
		/// Writes the table to the destination database. It will also create or truncate the table.
		/// </summary>
		/// <param name="table">Table.</param>
        public void WriteTable (TypedTable table)
        {
			var dbAdapter = DbAdapterFactory.GetDbAdapter (dbConfig.Engine);

			table = dbAdapter.ConvertTable (table);

			using (var conn = dbAdapter.GetConnection(dbConfig)) {
                conn.Open ();

                if (config.CreateDestinationTable)
                {
					var ddlSql = dbAdapter.GenerateDdl (table);
                    //Console.WriteLine(sqlDdl);

					var createCmd = conn.CreateCommand ();
					createCmd.CommandText = ddlSql;

					createCmd.ExecuteScalar ();
                }
                else if (config.TruncateDestinationTable)
                {
					var truncateSql = dbAdapter.GenerateTruncateStatement(table);

                    //Console.WriteLine(truncateSql);

					var truncateCmd  = conn.CreateCommand ();
					truncateCmd.CommandText = truncateSql;

                    truncateCmd.ExecuteScalar ();
                }

				var insertSql = dbAdapter.GenerateInsertSql (table);

                foreach (var row in table)
                {
					var cmd = conn.CreateCommand ();
					cmd.CommandText = insertSql;

					foreach (var cell in row.GetCells())
                    {
						var param = cmd.CreateParameter ();

						param.ParameterName = cell.Index.ToString ();
						param.Value = cell.Value;

						cmd.Parameters.Add(param);
                    }

					cmd.ExecuteScalar ();
                }
            }
        }
    }
}

