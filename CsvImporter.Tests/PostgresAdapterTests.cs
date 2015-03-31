using NUnit.Framework;
using System;
using System.Configuration;
using Npgsql;
using System.Linq;
using Dapper;
using CsvImporter.SqlTypes;

namespace CsvImporter.Tests
{
	[TestFixture ()]
	public class PostgresAdapterTests
	{
		private NpgsqlConnection conn;
		private PostgresAdapter pgAdapter = new PostgresAdapter ();
		private DbConfiguration dbConfig;
		private DbAdapterTests adapterTests;

		[TestFixtureSetUp()]
		public void TestFixtureSetUp ()
		{
			var engine = "postgres";

			var appSettings = ConfigurationManager.AppSettings;

			var hostname = appSettings ["postgres_hostname"];
			var database = appSettings ["postgres_database"];
			var username = appSettings ["postgres_username"];
			var password = appSettings ["postgres_password"];

			dbConfig = new DbConfiguration () { Engine = engine, Hostname = hostname, Database = database, Username = username, Password = password };

			conn = new NpgsqlConnection (pgAdapter.GetConnectionString (dbConfig));

			adapterTests = new DbAdapterTests (engine);
		}

		[TestFixtureTearDown()]
		public void TestFixtureTearDown()
		{
			conn.Close ();
			adapterTests.Dispose ();
		}

		[Test()]
		public void TestTableIsCreated ()
		{
			adapterTests.TestTableIsCreated ();
		}

		[Test()]
		public void TestRowsAreAdded ()
		{
			adapterTests.TestRowsAreAdded ();
		}

		[Test()]
		public void TestTruncationWorks ()
		{
			adapterTests.TestTruncationWorks ();
		}

		[Test()]
		public void TestTableIsCreatedWithPostgresNames ()
		{
			var types = new SqlType[] { new SqlTypes.Char (), new SqlTypes.Char (), new SqlTypes.Char (), new SqlTypes.Char () };
			var table = new TypedTable (new string[] {"A","CamelCase","finished_already"}, types);

			table.Name = "TestTableIsCreatedWithPostgresNames";

			var pgName = pgAdapter.GetEngineSpecificName (table.Name);

			try
			{
				var destConfig = new DestinationConfiguration (){ CreateDestinationTable=true};


				var pgDestination = new DbDestination (destConfig, dbConfig);

				pgDestination.WriteTable (table);

				var result1 = conn.Query("select * from information_schema.tables where table_name = @Name", new {Name=pgName});

				Assert.AreEqual(1, result1.Count());

				var result2 = conn.Query("select * from information_schema.columns where table_name = @Name and column_name in ('a', 'camel_case', 'finished_already')", new {Name=pgName});

				Assert.AreEqual(3, result2.Count());
			}
			finally
			{
				conn.Execute ("drop table " + pgAdapter.GetEngineSpecificName(table.Name) );
			}
		}
	}
}

