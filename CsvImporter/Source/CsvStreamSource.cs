using System;
using System.IO;
using CsvHelper;
using System.Collections.Generic;

namespace CsvImporter
{
	public class CsvStreamSource : IStreamSource
	{
		private SourceConfiguration srcConfig;
		private CsvReader csv;
		private string[] headers = null;

		public CsvStreamSource (TextReader reader, SourceConfiguration srcConfig)
		{
			this.srcConfig = srcConfig;

			csv = new CsvReader(reader);

			csv.Configuration.HasHeaderRecord = srcConfig.HasHeaders;
		}

		public string GetTableName ()
		{
			return srcConfig.Name;
		}

		public string[] GetColumnNames ()
		{
			return headers;
		}

		public IEnumerable<string[]> GetStringArrayEnumerable ()
		{
			bool first = true;

			while (csv.Read ()) {
				if (first)
				{
					if (srcConfig.HasHeaders)
					{
						headers = csv.FieldHeaders;
					}
					else
					{
						int length = csv.CurrentRecord.Length;

						headers = generateHeaderNames(length);
					}

					first = false;
				}

				yield return csv.CurrentRecord;
			}

			yield break;
		}

		private static string[] generateHeaderNames (int length, int charsToUse = 26) {
			var headers = new string[length];

			string prefix = "";

			char[] letters = {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z' };

			int letterLoop = 0;
			int prefixLoop = 0;

			for (int i = 1; i <= length; i++) {

				headers[i-1] = prefix + letters [letterLoop];

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

	public class CsvFileStreamSource : CsvStreamSource
	{
		public CsvFileStreamSource (CsvFileConfiguration fileConfig, SourceConfiguration srcConfig)
			: base (File.OpenText (fileConfig.Path), configureDefaultName(fileConfig, srcConfig))
		{
		}

		private static SourceConfiguration configureDefaultName (CsvFileConfiguration fileConfig, SourceConfiguration srcConfig)
		{
			// If a name wasn't passed in, use the filename without extension
			if (srcConfig.Name == null) {
				srcConfig.Name = System.IO.Path.GetFileNameWithoutExtension (fileConfig.Path);
			}

			return srcConfig;
		}
	}
}

