using System;
using System.IO;
using System.Linq;
using CsvHelper;
using System.Collections.Generic;

namespace CsvImporter
{
	/// <summary>
	/// The interface that every source must implement.
	/// </summary>
    public interface ISource
    {
		/// <summary>
		/// Reads the underlying source, producing a TypedTable instance.
		/// </summary>
		/// <returns>The table.</returns>
        TypedTable ReadTable ();
    }

	/// <summary>
	/// A CSV-file source.
	/// </summary>
    public class CsvFileSource : ISource
    {
        private CsvFileConfiguration fileConfig;
        private SourceConfiguration srcConfig;

		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.CsvFileSource"/> class.
		/// </summary>
		/// <param name="fileConfig">File config.</param>
		/// <param name="srcConfig">Source config.</param>
        public CsvFileSource (CsvFileConfiguration fileConfig, SourceConfiguration srcConfig)
        {
            // If a name wasn't passed in, use the filename without extension
            if (srcConfig.Name == null) {
                srcConfig.Name = System.IO.Path.GetFileNameWithoutExtension (fileConfig.Path);
            }

            this.fileConfig = fileConfig;
            this.srcConfig = srcConfig;
        }

        public TypedTable ReadTable ()
        {
            using (var reader = File.OpenText (fileConfig.Path))
            {
                return (new CsvSource (reader, srcConfig)).ReadTable();
            }
        }
    }

	/// <summary>
	/// A generic CSV source. Parses CSV from any TextReader into a TypedTable.
	/// </summary>
    public class CsvSource : ISource
    {
        private SourceConfiguration config;
        private TextReader reader;

		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.CsvSource"/> class.
		/// </summary>
		/// <param name="reader">Reader for the underlying data source</param>
		/// <param name="config">Config.</param>
        public CsvSource (TextReader reader, SourceConfiguration config)
        {
            this.config = config;
            this.reader = reader;
        }

        public TypedTable ReadTable ()
        {
			TypedTable table = new TypedTable(new string[0]);

            var csv = new CsvReader(reader);

            csv.Configuration.HasHeaderRecord = config.HasHeaders;

            bool first = true;

            while (csv.Read ()) {
                if (first) {
					string[] headerNames;

                    if (config.HasHeaders) {
						headerNames = csv.FieldHeaders;
                    } else {
                        int length = csv.CurrentRecord.Length;

						headerNames = generateHeaderNames(length);
                    }

					table = new TypedTable (headerNames);

                    first = false;
                }

				table.Add (csv.CurrentRecord);
            }

			table.Name = config.Name;

            return table;
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
}

