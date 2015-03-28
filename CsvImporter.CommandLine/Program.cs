using System;
using System.IO;
using System.Data;
using Npgsql;

namespace CsvImporter.CommandLine
{
    public class Program
    {
		public static TextWriter ErrorWriter = Console.Error;
		public static TextWriter OutWriter = Console.Out;

		private static void errorPrinter (Exception exp, bool verbose) {
			ErrorWriter.WriteLine (exp.Message);
			if (exp.InnerException != null) {
				errorPrinter (exp.InnerException, verbose);
			}

			if (verbose) {
				ErrorWriter.WriteLine (exp.StackTrace);
			}
		}

        public static int Main (string[] args)
        {
			var controller = new Controller (ErrorWriter);

			int retVal = 0;

			try
			{
				var msg = controller.Run(args);

				if (msg != string.Empty)
				{
					OutWriter.WriteLine(msg);
				}
			}
			catch (UserInputException uie)
			{
				ErrorWriter.WriteLine (uie.Message);
				retVal = 1;
			}
			catch (IOException ioe)
			{
				ErrorWriter.WriteLine ("There was an error reading the source file.");
				ErrorWriter.WriteLine (ioe.Message);
				retVal = 2;
			}
			// Standard SQL error?
			// Note, DataException is a SystemException
			catch (DataException de)
			{
				ErrorWriter.WriteLine ("There was an error with the database.");
				ErrorWriter.WriteLine (de.Message);
				retVal = 3;
			}
			// Apparently, Npgsql does not use DataException
			// Note, NpgsqlException is an ApplicationException
			catch (NpgsqlException de)
			{
				ErrorWriter.WriteLine ("There was an error with the database.");
				ErrorWriter.WriteLine (de.Message);
				retVal = 3;
			}
			catch (Exception e)
			{
				ErrorWriter.WriteLine (e.GetType());
				ErrorWriter.WriteLine ("There was an uncaught exception. This could be caused by a problem with your data, server, or the code.");
				ErrorWriter.WriteLine (e.Message);
				retVal = 4;
			}

			return retVal;
        }
    }
}
