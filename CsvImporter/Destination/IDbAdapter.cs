using System;
using CsvImporter.SqlTypes;
using System.Data.Common;

namespace CsvImporter
{
	public interface IDbAdapter
	{
		DbConnection GetConnection (DbConfiguration dbConfig);
		string GenerateTruncateStatement (TypedTable table);
		string GenerateDdl (TypedTable table);
		string GenerateInsertSql (TypedTable table);
		TypedTable ConvertTable (TypedTable table);
		string GetEngineSpecificName (string name);
	}
}

