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
		private DbConnection conn;
		private TextWriter errorWriter;
		private TextWriter outWriter;
		private readonly string testDb = "csv_importer";
		private readonly string testServer = "127.0.0.1";
		private readonly string testUser = "csv_importer";
		private readonly string testPassword = "csv_importer";
		private IList<string> filesToDelete = new List<string> ();

		[TestFixtureSetUp]
		public void SetUpFixture ()
		{
			conn = new NpgsqlConnection ("Server="+testServer+";Port=5432;Database="+testDb+";User Id="+testUser+";Password="+testPassword+";");
		}

		private string getConnectionArgs () {
			return "-e postgres -h "+testServer+" -p 5432 -d "+testDb+" -u "+testUser+" -W "+testPassword;
		}

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

		[TearDown]
		public void TearDown ()
		{
			foreach (var file in filesToDelete) {
				File.Delete (file);
			}

			var tables = conn.Query<string> ("select table_name from information_schema.tables where table_schema = 'public';");

			foreach (var table in tables) {
				conn.Execute ("drop Table " + table);
			}
		}

		[Test ()]
		public void VersionPrintsVersionLong ()
		{
			var args = new string[] { "--version" };

			int exitCode = Program.Main (args);

			Assert.AreEqual (0, exitCode);
			Assert.AreNotEqual(-1, outWriter.ToString().IndexOf("CsvImporter"));
			Assert.AreEqual("", errorWriter.ToString());
		}

		[Test ()]
		public void VersionPrintsVersionShort ()
		{
			var args = new string[] { "-V" };

			int exitCode = Program.Main (args);

			Assert.AreEqual (0, exitCode);
			Assert.AreNotEqual(-1, outWriter.ToString().IndexOf("CsvImporter"));
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

