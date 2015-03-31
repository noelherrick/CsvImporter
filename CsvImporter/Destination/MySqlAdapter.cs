using System;
using System.Linq;
using CsvImporter.SqlTypes;
using System.Text.RegularExpressions;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace CsvImporter
{
	public class MySqlAdapter : DbAdapter
	{
		public override DbConnection GetConnection (DbConfiguration dbConfig)
		{
			return new MySqlConnection (GetConnectionString(dbConfig));
		}

		public new string GenerateDdl (TypedTable table) {
			string colList = string.Join (",", table.Headers.ToList().Select (x => EscapeName(x.Name) + " " + GetTypeName(x.Type)));

			return string.Format ("create table {0} ({1}) character set utf8 collate utf8_general_ci", EscapeName(table.Name), colList);
		}

		public string GetConnectionString (DbConfiguration dbConfig)
		{
			dbConfig.Port = dbConfig.Port ?? "3306";

			return string.Format ("Server={0};Port={1};Database={2};Uid={3};Pwd={4};",
				dbConfig.Hostname, dbConfig.Port, dbConfig.Database, dbConfig.Username, dbConfig.Password);
		}

		public override string GetTypeName (CsvImporter.SqlTypes.SqlType type)
		{
			var category = type.TypeCategory;

			if (category == TypeCategory.DECIMAL) {
				var dec = type as SqlTypes.Decimal;
				return string.Format ("decimal({0},{1})", dec.Width, dec.RightOfPoint);
			} else if (category == TypeCategory.INTEGER) {
				return "integer";
			} else if (category == TypeCategory.DATE) {
				return "date";
			} else if (category == TypeCategory.TIME) {
				return "time";
			} else if (category == TypeCategory.TIMEZ) {
				return "time";
			} else if (category == TypeCategory.TIMESTAMP) {
				return "datetime";
			} else if (category == TypeCategory.TIMESTAMPZ) {
				return "datetime";
			} else if (category == TypeCategory.CHAR) {
				var charType = type as SqlTypes.Char;
				return "varchar("+charType.Width+")"; 
			} else {
				return "varchar(8000)";
			}
		}

		public override string GetEngineSpecificName (string name)
		{
			return Regex.Replace(name, "\\s", ""); 
		}

		public override string EscapeName (string name)
		{
			return "`" + name + "`";
		}
	}
}

