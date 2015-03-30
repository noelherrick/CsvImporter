using System;
using System.Linq;

namespace CsvImporter
{
	/// <summary>
	/// Source configuration for all source types.
	/// </summary>
    public class SourceConfiguration
    {
		/// <summary>
		/// Gets or sets a value indicating whether the source has a header row.
		/// </summary>
		/// <value><c>true</c> if the source has a header row; otherwise, <c>false</c>.</value>
        public bool HasHeaders {get;set;}

		/// <summary>
		/// Gets or sets the name of the source.
		/// </summary>
		/// <value>The name.</value>
        public string Name { get; set;}
    }
}

