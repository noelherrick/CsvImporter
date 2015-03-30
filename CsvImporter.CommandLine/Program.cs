using System;
using System.IO;
using System.Data;
using Npgsql;

namespace CsvImporter.CommandLine
{
	/// <summary>
	/// The main class for the CommandLine function.
	/// </summary>
    public class Program
    {
		/// <summary>
		/// The writer to send errors to. Defaults to standard error.
		/// </summary>
		public static TextWriter ErrorWriter = Console.Error;

		/// <summary>
		/// The writer to send output to. Defaults to standard out.
		/// </summary>
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

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		/// <returns>The exit code that is given to the operating system after the program ends.</returns>
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
