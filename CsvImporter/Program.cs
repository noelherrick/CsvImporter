using System;
using System.IO;
using CsvHelper;
using System.Collections.Generic;

namespace CsvImporter
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// Variables that will be passed in
			var fileName = @"/Users/noelh/Projects/CsvImporter/test2.csv";
			var hasHeaders = false;
			// End config vars

			var table = new Table ();

			using (var reader = File.OpenText(fileName) )
			{
				var csv = new CsvReader(reader);

				csv.Configuration.HasHeaderRecord = hasHeaders;

				bool first = true;

				var rows = new List<List<string>>();

				while (csv.Read ()) {
					if (first) {
						if (hasHeaders) {
							table.Headers = csv.FieldHeaders;
						} else {
							int length = csv.CurrentRecord.Length;

							table.Headers = generateHeaders(length);
						}

						first = false;
					}

					rows.Add (new List<string>(csv.CurrentRecord));
				}

				table.Rows = rows;
			}


		}

		private static List<string> generateHeaders (int length, int charsToUse = 26) {
			var headers = new List<string> (length);

			string prefix = "";

			char[] letters = {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z' };

			int letterLoop = 0;
			int prefixLoop = 0;

			for (int i = 1; i <= length; i++) {

				headers.Add (prefix + letters [letterLoop]);

				if (i % charsToUse == 0) {
					if (prefixLoop == charsToUse) {
						prefixLoop = 0;
					}

					prefix += letters [prefixLoop];
					letterLoop = 0;
					prefixLoop++;
				} else {
					letterLoop++;
				}
			}

			return headers;
		}
	}
}
