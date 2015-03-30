using System;

namespace CsvImporter
{
	/// <summary>
	/// Postgres-specific configuration for the destination.
	/// </summary>
    public class PostgresConfiguration
    {
		/// <summary>
		/// Gets or sets the connection string. Needs to be in this format: Server=;Port=;Database=;User Id=;Password=;
		/// </summary>
		/// <value>The connection string.</value>
        public string ConnectionString {get;set;}
    }
}

