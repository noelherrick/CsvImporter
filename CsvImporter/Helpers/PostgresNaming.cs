using System;
using System.Text.RegularExpressions;

namespace CsvImporter
{
	public class PostgresNaming
	{
		public static string ToPostgresName (String name)
		{
			name = Regex.Replace(name, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);

			name = Regex.Replace(name, "\\s", "");

			return name.ToLower(); 
		}
	}
}

