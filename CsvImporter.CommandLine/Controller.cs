using System;
using CommandLineLib = CommandLine;
using System.IO;
using System.Text;
using System.Linq;

namespace CsvImporter.CommandLine
{
	/// <summary>
	/// The class that helps to controll the CommandLine tool.
	/// </summary>
	public class Controller
	{
		private static CommandLineLib.Parser parser;
		private ReadTableFunc readTable;
		private WriteTableFunc writeTable; 

		/// <summary>
		/// The type for a function that reads a table. A simple form of dependancy injection.
		/// </summary>
		public delegate TypedTable ReadTableFunc (SourceConfiguration srcConfig, CsvFileConfiguration csvConfig);

		/// <summary>
		/// The type for a function that writes a table. A simple form of dependancy injection.
		/// </summary>
		public delegate void WriteTableFunc (DestinationConfiguration destConfig, PostgresConfiguration pgConfig, TypedTable table);

		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.CommandLine.Controller"/> class.
		/// </summary>
		/// <param name="commandLineTextWriter">Command line text writer.</param>
		/// <param name="readTable">A function that writes a table.</param>
		/// <param name="writeTable">A function that writes a table.</param>
		public Controller (TextWriter commandLineTextWriter,  ReadTableFunc readTable, WriteTableFunc writeTable)
		{
			parser = new CommandLineLib.Parser (with => {
				with.HelpWriter = commandLineTextWriter;
				with.IgnoreUnknownArguments = true;
			});

			this.readTable = readTable;
			this.writeTable = writeTable;

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.CommandLine.Controller"/> class with default source and destination.
		/// </summary>
		/// <param name="commandLineTextWriter">Command line text writer.</param>
		public Controller (TextWriter commandLineTextWriter)
			: this (commandLineTextWriter, ReadTableDefault, WriteTableDefault)
		{

		}

		/// <summary>
		/// Runs the program with the specified args.
		/// </summary>
		/// <param name="args">Arguments.</param>
		public string Run (string[] args)
		{
			// Why the manual parsing?
			// The CommandLine library has no way to intercept specific flags before parsing
			if (args.Contains("--help"))
			{
				var argDto = new Arguments ();

				return argDto.GetUsage();
			}

			if (args.Contains("--version") || args.Contains("-V"))
			{
				var asm = System.Reflection.Assembly.GetExecutingAssembly ();

				return asm.GetName().Name + " (" + asm.GetName().Version.ToString() + ")";
			}

			var options = new Arguments ();

			parser.ParseArguments (args, options);

			if (options.LastParserState != null && options.LastParserState.Errors.Count > 0)
			{
				var sb = new StringBuilder();

				foreach (var err in options.LastParserState.Errors) {
					if (err.ViolatesRequired) {
						sb.AppendFormat("-{0} was not supplied.", err.BadOption.LongName).AppendLine();
					} else {
						sb.AppendFormat("-{0} is not in the right format.", err.BadOption.LongName).AppendLine();
					}
				}

				sb.Append(options.GetUsage ());

				throw new UserInputException (sb.ToString());
			}
				
			var connString = string.Format ("Server={0};Port={1};Database={2};User Id={3};Password={4};",
				options.Host, options.Port, options.Database, options.User, options.Password);

			foreach (var file in options.Files)
			{
				var csvConfig = new CsvFileConfiguration () { Path = file };
				var srcConfig = new SourceConfiguration () { HasHeaders = options.HasHeaders };

				var table = readTable (srcConfig, csvConfig);

				var destConfig = new DestinationConfiguration ()
				{ TruncateDestinationTable = options.TruncateTable, CreateDestinationTable= options.CreateTable};
				var pgConfig = new PostgresConfiguration () { ConnectionString = connString };

				writeTable (destConfig, pgConfig, table);
			}

			return "";
		}

		/// <summary>
		/// The default ReadTable function, namely, a CSV file source.
		/// </summary>
		/// <returns>The table that was read in.</returns>
		/// <param name="srcConfig">Source config.</param>
		/// <param name="csvConfig">Csv config.</param>
		public static TypedTable ReadTableDefault (SourceConfiguration srcConfig, CsvFileConfiguration csvConfig)
		{
			var src = new CsvFileSource (csvConfig, srcConfig);
			return src.ReadTable ();
		}

		/// <summary>
		/// Writes the table to the default destination, namely a Postgres table.
		/// </summary>
		/// <param name="destConfig">Destination config.</param>
		/// <param name="pgConfig">Postgres-specific config.</param>
		/// <param name="table">Table to write.</param>
		public static void WriteTableDefault (DestinationConfiguration destConfig, PostgresConfiguration pgConfig, TypedTable table)
		{
			var dest = new PostgresDestination (destConfig, pgConfig);

			dest.WriteTable (table);
		}
	}
}

