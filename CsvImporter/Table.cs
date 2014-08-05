using System;
using System.Collections.Generic;

namespace CsvImporter
{
	public class Table
	{
		public string Name { get; set; }
		public IEnumerable<string> Headers { get; set; }
		public IEnumerable<IEnumerable<string>> Rows { get; set; }
	}
}

