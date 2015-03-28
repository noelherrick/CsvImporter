using System;
using CommandLineLib = CommandLine;
using System.IO;
using System.Text;
using System.Linq;

namespace CsvImporter.CommandLine
{
	public class Controller
	{
		private static CommandLineLib.Parser parser;

		public delegate void WriteTableFunc (DestinationConfiguration destConfig, PostgresConfiguration pgConfig, TypedTable table);
		public delegate TypedTable ReadTableFunc (SourceConfiguration srcConfig, CsvFileConfiguration csvConfig);

		private WriteTableFunc writeTable; 
		private ReadTableFunc readTable;

		public Controller (TextWriter commandLineTextWriter,  ReadTableFunc readTable, WriteTableFunc writeTable)
		{
			parser = new CommandLineLib.Parser (with => {
				with.HelpWriter = commandLineTextWriter;
				with.IgnoreUnknownArguments = true;
			});

			this.readTable = readTable;
			this.writeTable = writeTable;

		}

		public Controller (TextWriter commandLineTextWriter)
			: this (commandLineTextWriter, ReadTableDefault, WriteTableDefault)
		{

		}

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

		public static TypedTable ReadTableDefault (SourceConfiguration srcConfig, CsvFileConfiguration csvConfig)
		{
			var src = new CsvFileSource (csvConfig, srcConfig);
			return src.ReadTable ();
		}

		public static void WriteTableDefault (DestinationConfiguration destConfig, PostgresConfiguration pgConfig, TypedTable table)
		{
			var dest = new PostgresDestination (destConfig, pgConfig);

			dest.WriteTable (table);
		}
	}
}

