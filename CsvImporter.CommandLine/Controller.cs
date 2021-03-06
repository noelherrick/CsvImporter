﻿using System;
using CommandLineLib = CommandLine;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

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
		public delegate IRowStream ReadTableFunc (SourceConfiguration srcConfig, CsvFileConfiguration csvConfig);

		/// <summary>
		/// The type for a function that writes a table. A simple form of dependancy injection.
		/// </summary>
		public delegate void WriteTableFunc (DestinationConfiguration destConfig, DbConfiguration pgConfig, IRowStream stream);

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

			if (options.LastParserState != null && options.LastParserState.Errors.Any())
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

			var defaultTableNames = true;
			var index = 0;

			if (options.Names != null && options.Names.Count != 0)
			{
				if (options.Names.Count != options.Files.Count) {
					throw new UserInputException ("You must specifiy the same number of table names as files.");
				}

				defaultTableNames = false;
			}

			foreach (var file in options.Files)
			{
				string tableName = null;

				if (!defaultTableNames)
				{
					tableName = options.Names[index++];
				}

				var csvConfig = new CsvFileConfiguration () { Path = file };
				var srcConfig = new SourceConfiguration () { Name = tableName, HasHeaders = options.HasHeaders, BufferSize = options.BufferSize };

				var stream = readTable (srcConfig, csvConfig);

				var destConfig = new DestinationConfiguration ()
				{ TruncateDestinationTable = options.TruncateTable, CreateDestinationTable= options.CreateTable};
				var dbConfig = new DbConfiguration () { Engine = options.Engine, Port = options.Port, Hostname = options.Hostname, Database = options.Database, Username = options.Username, Password = options.Password };

				writeTable (destConfig, dbConfig, stream);
			}

			return "";
		}

		/// <summary>
		/// The default ReadTable function, namely, a CSV file source.
		/// </summary>
		/// <returns>The table that was read in.</returns>
		/// <param name="srcConfig">Source config.</param>
		/// <param name="csvConfig">Csv config.</param>
		public static IRowStream ReadTableDefault (SourceConfiguration srcConfig, CsvFileConfiguration csvConfig)
		{
			var csvSrc = new CsvFileStreamSource (csvConfig, srcConfig);
			return new RowStream (csvSrc, srcConfig.BufferSize);
		}

		/// <summary>
		/// Writes the table to the default destination, namely a Postgres table.
		/// </summary>
		/// <param name="destConfig">Destination config.</param>
		/// <param name="pgConfig">Postgres-specific config.</param>
		/// <param name="table">Table to write.</param>
		public static void WriteTableDefault (DestinationConfiguration destConfig, DbConfiguration pgConfig, IRowStream stream)
		{
			var dest = new DbDestination (destConfig, pgConfig);

			dest.WriteStream (stream);
		}
	}
}

