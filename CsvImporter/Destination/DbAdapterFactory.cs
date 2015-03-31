using System;

namespace CsvImporter
{
	public class DbAdapterFactory
	{
		public static IDbAdapter GetDbAdapter(string engineName)
		{
			if (engineName == "postgres")
			{
				return new PostgresAdapter();
			}
			else if (engineName == "mysql")
			{
				return new MySqlAdapter ();
			}

			return new PostgresAdapter();
		}
	}
}

