using System;
using CsvImporter.SqlTypes;
using System.Text.RegularExpressions;
using System.Data.Common;
using Npgsql;

namespace CsvImporter
{
	public class PostgresAdapter : DbAdapter
	{
		public override DbConnection GetConnection (DbConfiguration dbConfig)
		{
			return new NpgsqlConnection (GetConnectionString(dbConfig));
		}

		public string GetConnectionString (DbConfiguration dbConfig)
		{
			dbConfig.Port = dbConfig.Port ?? "5432";

			return string.Format ("Server={0};Port={1};Database={2};User Id={3};Password={4};",
				dbConfig.Hostname, dbConfig.Port, dbConfig.Database, dbConfig.Username, dbConfig.Password);
		}

		public override string GetTypeName (CsvImporter.SqlTypes.SqlType type)
		{
			var category = type.TypeCategory;

			if (category == TypeCategory.DECIMAL) {
				var dec = type as SqlTypes.Decimal;
				return string.Format("decimal({0},{1})", dec.Width, dec.RightOfPoint);
			} else if (category == TypeCategory.INTEGER) {
				return "integer";
			} else if (category == TypeCategory.DATE) {
				return "date";
			} else if (category == TypeCategory.TIME) {
				return "time";
			} else if (category == TypeCategory.TIMEZ) {
				return "timez";
			} else if (category == TypeCategory.TIMESTAMP) {
				return "timestamp";
			} else if (category == TypeCategory.TIMESTAMPZ) {
				return "timestampz";
			} else {
				return "text";
			}
		}

		public override string GetEngineSpecificName (string name)
		{
			name = Regex.Replace(name, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);

			name = Regex.Replace(name, "\\s", "");

			return name.ToLower(); 
		}

		public override string EscapeName (string name)
		{
			return "\"" + name + "\"";
		}
	}
}

