using System;

namespace CsvImporter
{
	/// <summary>
	/// Configures the destination for every database engine
	/// </summary>
    public class DestinationConfiguration
    {
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CsvImporter.DestinationConfiguration"/> creates a table at destination database.
		/// </summary>
		/// <value><c>true</c> to create the destination table; otherwise, <c>false</c>.</value>
        public bool CreateDestinationTable { get; set;}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CsvImporter.DestinationConfiguration"/> truncates the
		/// destination table.
		/// </summary>
		/// <value><c>true</c> to truncate destination table; otherwise, <c>false</c>.</value>
        public bool TruncateDestinationTable { get; set;}
    }
}

