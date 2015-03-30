using System;
using System.Text.RegularExpressions;

namespace CsvImporter
{
	/// <summary>
	/// Static class for Postgres naming function.
	/// </summary>
	public static class PostgresNaming
	{
		/// <summary>
		/// Converts a CamelCase name to snake case name. It has no effect on names that
		///  are already snake case. Postgres automatically converts schema names (tables, columns, etc.) to lowercase.
		/// </summary>
		/// <returns>The Postgres (snake case) name.</returns>
		/// <param name="name">Name. Any name that must be in snake case.</param>
		public static string ToPostgresName (String name)
		{
			name = Regex.Replace(name, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);

			name = Regex.Replace(name, "\\s", "");

			return name.ToLower(); 
		}
	}
}

