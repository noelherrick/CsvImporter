using System;
using CommandLineLib = CommandLine;
using CommandLine.Text;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

namespace CsvImporter.CommandLine
{
	/// <summary>
	/// Declares the parameters for the commandline tool using the CommandLine library.
	/// </summary>
	public class Arguments {
		private static readonly string SUMMARY =
			@"The CsvImporter tool takes CSV and imports them into a SQL database.";
			
		/// <summary>
		/// Gets or sets the destination database host.
		/// </summary>
		/// <value>The host.</value>
		[CommandLineLib.Option('h', "host", DefaultValue = "127.0.0.1",
			HelpText = "The database server.")]
		public string Hostname { get; set; }

		/// <summary>
		/// Gets or sets the destination database port.
		/// </summary>
		/// <value>The port.</value>
		[CommandLineLib.Option('p', "port",
			HelpText = "The database server port.")]
		public string Port { get; set; }

		/// <summary>
		/// Gets or sets the destination database name.
		/// </summary>
		/// <value>The database.</value>
		[CommandLineLib.Option('d', "database", Required = true,
			HelpText = "The database name.")]
		public string Database { get; set; }

		/// <summary>
		/// Gets or sets the destination database user.
		/// </summary>
		/// <value>The user.</value>
		[CommandLineLib.Option('u', "user", Required = true,
			HelpText = "The database user.")]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the destination database password.
		/// </summary>
		/// <value>The password.</value>
		[CommandLineLib.Option('W', "password", Required = true,
			HelpText = "The database password.")]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the destination database engine.
		/// </summary>
		/// <value>The engine.</value>
		[CommandLineLib.Option('e', "engine", DefaultValue = "postgres",
			HelpText = "The database engine (oracle, mysql, postgres, sqlserver, sqlite).")]
		public string Engine { get; set; }

		/// <summary>
		/// Gets or sets the names of the tables to use at the destination database.
		/// </summary>
		/// <value>The names.</value>
		[CommandLineLib.OptionList('n', "names",
			HelpText = "The names of the tables to use at the destination database.")]
		public IList<string> Names { get; set; }

		/// <summary>
		/// Gets or sets a value tell the command line program to print verbose logging
		/// </summary>
		/// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
		[CommandLineLib.Option('v', "verbose", DefaultValue = false,
			HelpText = "Prints all messages to standard output.")]
		public bool Verbose { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the source CSV-file has headers.
		/// </summary>
		/// <value><c>true</c> if the source CSV-file has headers; otherwise, <c>false</c>.</value>
		[CommandLineLib.Option('H', "headers", DefaultValue = false,
			HelpText = "Whether the first line is a header row.")]
		public bool HasHeaders { get; set; }

		/// <summary>
		/// Gets or sets the source file paths.
		/// </summary>
		/// <value>The files.</value>
		[CommandLineLib.OptionList('f', "file", Required = true,
			HelpText = "The files to import into the database.")]
		public IList<string> Files { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to truncate the destination table.
		/// </summary>
		/// <value><c>true</c> to truncate the destination table; otherwise, <c>false</c>.</value>
		[CommandLineLib.Option('t', "truncate", DefaultValue = false,
			HelpText = "Whether to truncate the table first.")]
		public bool TruncateTable { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to create the destination table.
		/// </summary>
		/// <value><c>true</c> to create the destination table; otherwise, <c>false</c>.</value>
		[CommandLineLib.Option('c', "create", DefaultValue = false,
			HelpText = "Whether to create the table first.")]
		public bool CreateTable { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to print the version and exit
		/// </summary>
		/// <value><c>true</c> if  to print the version and exit; otherwise, <c>false</c>.</value>
		[CommandLineLib.Option('V', "version", DefaultValue = false,
			HelpText = "Whether to create the table first.")]
		public bool Version { get; set; }

		/// <summary>
		/// Gets or sets the last state of the CommandLine parser.
		/// </summary>
		/// <value>The last state of the parser.</value>
		[CommandLineLib.ParserState]
		public CommandLineLib.IParserState LastParserState { get; set; }

		/// <summary>
		/// Gets the usage message for the command line lib.
		/// </summary>
		/// <returns>The usage.</returns>
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

