using System;

namespace CsvImporter.SqlTypes
{
    /// <summary>
	/// Type category for SQL data types. Lower numbers mean a broader category
    /// </summary>
	public enum TypeCategory
    {
        CHAR,
		TIMESTAMPZ,
		TIMESTAMP,
        DATE,
		TIMEZ,
        TIME,
        DECIMAL,
        INTEGER
    }
}

