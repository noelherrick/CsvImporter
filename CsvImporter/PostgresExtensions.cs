using System;
using System.Linq;

namespace CsvImporter
{
	public static class PostgresExtensions
	{
		public static string generatePgDdl (this Table table) {
			string colList = string.Join (",", table.Headers.ToList().ForEach (x => "\"" + x + "\""));

			return string.Format ("create table {0} ({1})", table.Name, colList);
		}
	}
}

