using System;
using CommandLineLib = CommandLine;
using CommandLine.Text;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

namespace CsvImporter.CommandLine
{
	public class Arguments {
		private static readonly string SUMMARY =
			@"The CsvImporter tool takes CSV and imports them into a SQL database.";
			
		[CommandLineLib.Option('h', "host", DefaultValue = "127.0.0.1",
			HelpText = "The database server.")]
		public string Host { get; set; }

		[CommandLineLib.Option('p', "port",
			HelpText = "The database server port.")]
		public string Port { get; set; }

		[CommandLineLib.Option('d', "database", Required = true,
			HelpText = "The database name.")]
		public string Database { get; set; }

		[CommandLineLib.Option('u', "user", Required = true,
			HelpText = "The database user.")]
		public string User { get; set; }

		[CommandLineLib.Option('W', "password", Required = true,
			HelpText = "The database password.")]
		public string Password { get; set; }

		[CommandLineLib.Option('e', "engine",
			HelpText = "The database engine (oracle, mysql, postgres, sqlserver, sqlite).")]
		public string Engine { get; set; }

		[CommandLineLib.Option('v', "verbose", DefaultValue = false,
			HelpText = "Prints all messages to standard output.")]
		public bool Verbose { get; set; }

		[CommandLineLib.Option('H', "headers", DefaultValue = false,
			HelpText = "Whether the first line is a header row.")]
		public bool HasHeaders { get; set; }

		[CommandLineLib.OptionList('f', "file", Required = true,
			HelpText = "The files to import into the database.")]
		public IList<string> Files { get; set; }

		[CommandLineLib.Option('t', "truncate", DefaultValue = false,
			HelpText = "Whether to truncate the table first.")]
		public bool TruncateTable { get; set; }

		[CommandLineLib.Option('c', "create", DefaultValue = false,
			HelpText = "Whether to create the table first.")]
		public bool CreateTable { get; set; }

		[CommandLineLib.Option('V', "version", DefaultValue = false,
			HelpText = "Whether to create the table first.")]
		public bool Version { get; set; }

		[CommandLineLib.ParserState]
		public CommandLineLib.IParserState LastParserState { get; set; }

		[CommandLineLib.HelpOption]
		public virtual string GetUsage()
		{
			var asm = System.Reflection.Assembly.GetExecutingAssembly ();

			var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

			var help = new HelpText {
				Heading = new HeadingInfo(asm.GetName().Name, asm.GetName().Version.ToString()),
				AdditionalNewLineAfterOption = true,
				AddDashesToOption = true };
			help.AddPreOptionsLine ("Copyright (c) " + versionInfo.LegalCopyright);
			help.AddPreOptionsLine(SUMMARY);
			help.AddPreOptionsLine (String.Empty);
			help.AddPreOptionsLine ("Options");
			help.AddPreOptionsLine (String.Empty);
			help.AddOptions(this);
			return help;
		}
	}
}

