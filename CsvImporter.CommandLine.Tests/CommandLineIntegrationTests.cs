using NUnit.Framework;
using System;
using System.Data.Common;
using System.IO;
using Npgsql;
using Dapper;
using System.Collections.Generic;

namespace CsvImporter.CommandLine.Tests
{
	[TestFixture ()]
	public class CommandLineIntegrationTests
	{
		private TextWriter errorWriter;
		private TextWriter outWriter;

		[TestFixtureTearDown]
		public void TearDownFixture ()
		{
		}

		[SetUp()]
		public void SetUp () {
			errorWriter = new StringWriter ();

			Program.ErrorWriter = errorWriter;

			outWriter = new StringWriter ();

			Program.OutWriter = outWriter;
		}

		[Test ()]
		public void VersionPrintsVersionLong ()
		{
			var args = new string[] { "--version" };

			int exitCode = Program.Main (args);

			Assert.AreEqual (0, exitCode);
			Assert.AreNotEqual(-1, outWriter.ToString().IndexOf("csv-importer"));
			Assert.AreEqual("", errorWriter.ToString());
		}

		[Test ()]
		public void VersionPrintsVersionShort ()
		{
			var args = new string[] { "-V" };

			int exitCode = Program.Main (args);

			Assert.AreEqual (0, exitCode);
			Assert.AreNotEqual(-1, outWriter.ToString().IndexOf("csv-importer"));
			Assert.AreEqual("", errorWriter.ToString());
		}

		[Test()]
		public void HelpPrintsHelp () {
			var args = new string[] { "--help" };

			int exitCode = Program.Main (args);

			Assert.AreEqual (0, exitCode);
			Assert.AreNotEqual(-1, outWriter.ToString().IndexOf("CsvImporter"));
			Assert.AreNotEqual(-1, outWriter.ToString().IndexOf("Options"));
			Assert.AreEqual("", errorWriter.ToString());
		}
	}
}

