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
	public class PostgresDestinationTests
	{
		private NpgsqlConnection conn;
		private string connString;

		[TestFixtureSetUp()]
		public void TestFixtureSetUp ()
		{
			var appSettings = ConfigurationManager.AppSettings;

			connString = appSettings ["csv_importer"];

			conn = new NpgsqlConnection (connString);
		}

		[TestFixtureTearDown()]
		public void TestFixtureTearDown()
		{
			conn.Close ();
		}

		[Test()]
		public void TestTableIsCreated ()
		{
			var types = new SqlType[] { new SqlTypes.Char (), new SqlTypes.Char (), new SqlTypes.Char (), new SqlTypes.Char () };
			var table = new TypedTable (new string[] {"a","b","c"}, types);

			table.Name = PostgresNaming.ToPostgresName("TestTableIsCreated");

			try
			{
				var destConfig = new DestinationConfiguration (){ CreateDestinationTable=true};
				var pgConfig = new PostgresConfiguration () { ConnectionString = connString };

				var pgDestination = new PostgresDestination (destConfig, pgConfig);

				pgDestination.WriteTable (table);

				var result = conn.Query("select * from information_schema.tables where table_name = @Name", new {Name=table.Name});

				Assert.AreEqual(1, result.Count());
			}
			finally
			{
				conn.Execute ("drop table " + table.Name );
			}
		}

		[Test()]
		public void TestRowsAreAdded ()
		{
			var table = new TypedTable (new string[] {"a","b","c"});

			table.Add (new string[] {"1","1","1"});
			table.Add (new string[] {"2","2","2"});
			table.Add (new string[] {"3","3","3"});

			table.Name = PostgresNaming.ToPostgresName("TestRowsAreAdded");

			try
			{
				var destConfig = new DestinationConfiguration (){ CreateDestinationTable=true};
				var pgConfig = new PostgresConfiguration () { ConnectionString = connString };

				var pgDestination = new PostgresDestination (destConfig, pgConfig);

				pgDestination.WriteTable (table);

				var result = conn.Query<int>("select count(*) as a from " + table.Name).First();

				Assert.AreEqual(3, result);
			}
			finally
			{
				conn.Execute ("drop table " + table.Name );
			}
		}

		[Test()]
		public void TestTruncationWorks ()
		{
			var table1 = new TypedTable (new string[] {"a","b","c"});

			table1.Add (new string[] {"1","1","1"});
			table1.Add (new string[] {"2","2","2"});
			table1.Add (new string[] {"3","3","3"});

			table1.Name = PostgresNaming.ToPostgresName("TestTruncationWorks");

			var table2 = new TypedTable (new string[] {"a","b","c"});

			table2.Add (new string[] {"4","4","4"});
			table2.Add (new string[] {"5","5","5"});
			table2.Add (new string[] {"6","6","6"});

			table2.Name = table1.Name;

			try
			{
				var pgConfig = new PostgresConfiguration () { ConnectionString = connString };

				// Let's create a table using the normal methods

				var destConfig1 = new DestinationConfiguration (){ CreateDestinationTable=true};

				var pgDestination1 = new PostgresDestination (destConfig1, pgConfig);

				pgDestination1.WriteTable (table1);

				var result1 = conn.Query<int>("select count(*) as a from " + table1.Name + " where a in (1,2,3)").First();

				Assert.AreEqual(3, result1);

				// Okay, now let's truncate to see if that works

				var destConfig2 = new DestinationConfiguration (){ TruncateDestinationTable=true};

				var pgDestination2 = new PostgresDestination (destConfig2, pgConfig);

				pgDestination2.WriteTable (table2);

				// We don't want any old rows
				var result2 = conn.Query<int>("select count(*) as a from " + table1.Name + " where a in (1,2,3)").First();

				Assert.AreEqual(0, result2);

				// We do want the new rows

				var result3 = conn.Query<int>("select count(*) as a from " + table1.Name + " where a in (4,5,6)").First();

				Assert.AreEqual(3, result3);
			}
			finally
			{
				conn.Execute ("drop table " + table1.Name );
			}
		}

		[Test()]
		public void TestTableIsCreatedWithPostgresNames ()
		{
			var types = new SqlType[] { new SqlTypes.Char (), new SqlTypes.Char (), new SqlTypes.Char (), new SqlTypes.Char () };
			var table = new TypedTable (new string[] {"A","CamelCase","finished_already"}, types);

			table.Name = "TestTableIsCreated";

			var pgName = PostgresNaming.ToPostgresName (table.Name);

			try
			{
				var destConfig = new DestinationConfiguration (){ CreateDestinationTable=true};
				var pgConfig = new PostgresConfiguration () { ConnectionString = connString };

				var pgDestination = new PostgresDestination (destConfig, pgConfig);

				pgDestination.WriteTable (table);

				var result1 = conn.Query("select * from information_schema.tables where table_name = @Name", new {Name=pgName});

				Assert.AreEqual(1, result1.Count());

				var result2 = conn.Query("select * from information_schema.columns where table_name = @Name and column_name in ('a', 'camel_case', 'finished_already')", new {Name=pgName});
			
				Assert.AreEqual(3, result2.Count());
			}
			finally
			{
				conn.Execute ("drop table " + PostgresNaming.ToPostgresName (table.Name) );
			}
		}
	}
}

