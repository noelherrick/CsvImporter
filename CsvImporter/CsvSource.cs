using System;
using System.IO;
using System.Linq;
using CsvHelper;
using System.Collections.Generic;

namespace CsvImporter
{
    public interface ISource
    {
        TypedTable ReadTable ();
    }

    public class CsvFileSource : ISource
    {
        private CsvFileConfiguration fileConfig;
        private SourceConfiguration srcConfig;

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

    public class CsvSource : ISource
    {
        private SourceConfiguration config;
        private TextReader reader;

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

