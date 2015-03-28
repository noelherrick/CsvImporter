namespace CsvImporter
{
    using System;
    using System.IO;
    using CsvHelper;
    using System.Collections.Generic;
    using System.Linq;

    class MainClass
    {
        public static void Main (string[] args)
        {
            // Variables that will be passed in
            var fileName = @"/Users/noelh/Projects/CsvImporter/test2.csv";
            var hasHeaders = false;
			var connectionString = "Server=127.0.0.1;Port=5432;Database=csv_convert_tests;User Id=csv_convert_tests;Password=csv_convert_tests;";
            // End config vars

            
        }
    }
}
