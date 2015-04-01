using System;
using CsvImporter.SqlTypes;
using System.Data.Common;

namespace CsvImporter
{
	public interface IDbAdapter
	{
		DbConnection GetConnection (DbConfiguration dbConfig);
		string GenerateTruncateStatement (IRowStream stream);
		string GenerateDdl (IRowStream stream);
		string GenerateInsertSql (IRowStream stream);
		string GetEngineSpecificName (string name);
	}
}

