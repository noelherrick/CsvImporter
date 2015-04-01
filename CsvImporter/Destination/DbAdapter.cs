using System;
using System.Linq;
using System.Data.Common;

namespace CsvImporter
{
	public abstract class DbAdapter : IDbAdapter
	{
		public abstract DbConnection GetConnection (DbConfiguration dbConfig);

		public string GenerateTruncateStatement (IRowStream stream)
		{
			return string.Format ("truncate table {0};", EscapeName(GetEngineSpecificName(stream.Name)));
		}

		public string GenerateDdl (IRowStream stream) {
			string colList = string.Join (",", stream.Headers.Select (x => EscapeName(GetEngineSpecificName(x.Name)) + " " + GetTypeName(x.Type)));

			return string.Format ("create table {0} ({1})", EscapeName(GetEngineSpecificName(stream.Name)), colList);
		}

		public string GenerateInsertSql (IRowStream stream)
		{
			string colList = string.Join (",", stream.Headers.Select(x => GetEngineSpecificName(x.Name)).Select (x => EscapeName(x)));
			string paramList = string.Join (",", stream.Headers.Select (x => "@" + x.Index));

			return string.Format ("insert into {0} ({1}) values ({2})", EscapeName(GetEngineSpecificName(stream.Name)), colList, paramList);
		}

		public abstract string GetTypeName (CsvImporter.SqlTypes.SqlType type);
		public abstract string GetEngineSpecificName (string name);
		public abstract string EscapeName (string name);
	}
}

