using System;
using System.Linq;
using CsvImporter.SqlTypes;
using Npgsql;

namespace CsvImporter
{
    public class PostgresDestination
    {
        private DestinationConfiguration config;
        private PostgresConfiguration pgConfig;

        public PostgresDestination (DestinationConfiguration config, PostgresConfiguration pgConfig)
        {
            this.config = config;
            this.pgConfig = pgConfig;
        }

        public void WriteTable (TypedTable table)
        {
			table = table.AsPostgresTable ();

            using (var conn = new NpgsqlConnection(pgConfig.ConnectionString)) {
                conn.Open ();

                if (config.CreateDestinationTable)
                {
                    var ddlSql = generateDdl (table);
                    //Console.WriteLine(sqlDdl);

                    var createCmd = new NpgsqlCommand(ddlSql, conn);


					createCmd.ExecuteScalar ();
                }
                else if (config.TruncateDestinationTable)
                {
                    var truncateSql = generateTruncateStatement (table);

                    //Console.WriteLine(truncateSql);

                    var truncateCmd = new NpgsqlCommand(truncateSql, conn);

                    truncateCmd.ExecuteScalar ();
                }

                string colList = string.Join (",", table.Headers.ToList().Select (x => "\"" + x.Name  + "\""));
				string paramList = string.Join (",", table.Headers.ToList().Select (x => "@" + x.Index));

                var sql = string.Format ("insert into \"{0}\" ({1}) values ({2})", table.Name, colList, paramList);

                foreach (var row in table)
                {
                    var cmd = new NpgsqlCommand(sql, conn);

					foreach (var cell in row.GetCells())
                    {
						cmd.Parameters.Add(new NpgsqlParameter(cell.Index.ToString(), cell.Value));
                    }

					cmd.ExecuteScalar ();
                }
            }
        }

        private string generateTruncateStatement (TypedTable table)
        {
            return string.Format ("truncate table \"{0}\";", table.Name);
        }

        private string generateDdl (TypedTable table) {
            string colList = string.Join (",", table.Headers.ToList().Select (x => "\"" + x.Name  + "\""+ " " + GetPostgresTypeName(x.Type)));

            return string.Format ("create table \"{0}\" ({1})", table.Name, colList);
        }

        private static string GetPostgresTypeName (SqlType type) {
            var category = type.TypeCategory;

            if (category == TypeCategory.DECIMAL) {
                // TODO: We need to see what kind of decimal it is - this might complicate the data type dectection
                var dec = type as SqlTypes.Decimal;
                return string.Format("decimal({0},{1})", dec.Width, dec.RightOfPoint);
            } else if (category == TypeCategory.INTEGER) {
                return "integer";
                // TODO: Need to get better mapping for dates & times
            } else if (category == TypeCategory.DATE) {
                return "date";
            } else if (category == TypeCategory.TIME) {
                return "time";
            } else {
                return "text";
            }
        }
    }
}

