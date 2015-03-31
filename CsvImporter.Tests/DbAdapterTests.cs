using System;
using System.Linq;
using CsvImporter;
using CsvImporter.SqlTypes;
using Dapper;
using System.Configuration;
using System.Data.Common;
using NUnit.Framework;

namespace CsvImporter.Tests
{
	/// <summary>
	/// Tests for all DB adapters. Not an actual test suite (it lack annotations).
	/// </summary>
	public class DbAdapterTests : IDisposable
	{
		public void Dispose ()
		{
			conn.Close ();
		}

		private DbConnection conn;
		private IDbAdapter adapter;
		private DbConfiguration dbConfig;

		public DbAdapterTests (string engine)
		{
			adapter = DbAdapterFactory.GetDbAdapter(engine);

			var appSettings = ConfigurationManager.AppSettings;

			var hostname = appSettings [engine+"_hostname"];
			var database = appSettings [engine+"_database"];
			var username = appSettings [engine+"_username"];
			var password = appSettings [engine+"_password"];

			dbConfig = new DbConfiguration () { Engine = engine, Hostname = hostname, Database = database, Username = username, Password = password };

			conn = adapter.GetConnection (dbConfig);
		}

		public void TestTableIsCreated ()
		{
			var types = new SqlType[] { new SqlTypes.Char (), new SqlTypes.Char (), new SqlTypes.Char (), new SqlTypes.Char () };
			var table = new TypedTable (new string[] {"a","b","c"}, types);

			table.Name = adapter.GetEngineSpecificName("TestTableIsCreated");

			try
			{
				var destConfig = new DestinationConfiguration (){ CreateDestinationTable=true};

				var pgDestination = new DbDestination (destConfig, dbConfig);

				pgDestination.WriteTable (table);

				var result = conn.Query("select * from information_schema.tables where table_name = @Name", new {Name=table.Name});

				Assert.AreEqual(1, result.Count());
			}
			finally
			{
				conn.Execute ("drop table " + table.Name );
			}
		}
			
		public void TestRowsAreAdded ()
		{
			var table = new TypedTable (new string[] {"a","b","c"});

			table.Add (new string[] {"1","1","1"});
			table.Add (new string[] {"2","2","2"});
			table.Add (new string[] {"3","3","3"});

			table.Name = adapter.GetEngineSpecificName("TestRowsAreAdded");

			try
			{
				var destConfig = new DestinationConfiguration (){ CreateDestinationTable=true};


				var pgDestination = new DbDestination (destConfig, dbConfig);

				pgDestination.WriteTable (table);

				var result = conn.Query<int>("select count(*) as a from " + table.Name).First();

				Assert.AreEqual(3, result);
			}
			finally
			{
				conn.Execute ("drop table " + table.Name );
			}
		}

		public void TestTruncationWorks ()
		{
			var table1 = new TypedTable (new string[] {"a","b","c"});

			table1.Add (new string[] {"1","1","1"});
			table1.Add (new string[] {"2","2","2"});
			table1.Add (new string[] {"3","3","3"});

			table1.Name = adapter.GetEngineSpecificName("TestTruncationWorks");

			var table2 = new TypedTable (new string[] {"a","b","c"});

			table2.Add (new string[] {"4","4","4"});
			table2.Add (new string[] {"5","5","5"});
			table2.Add (new string[] {"6","6","6"});

			table2.Name = table1.Name;

			try
			{
				// Let's create a table
				var destConfig1 = new DestinationConfiguration (){ CreateDestinationTable=true};

				var pgDestination1 = new DbDestination (destConfig1, dbConfig);

				pgDestination1.WriteTable (table1);

				var result1 = conn.Query<int>("select count(*) as a from " + table1.Name + " where a in (1,2,3)").First();

				Assert.AreEqual(3, result1);

				// Okay, now let's truncate to see if that works

				var destConfig2 = new DestinationConfiguration (){ TruncateDestinationTable=true};

				var pgDestination2 = new DbDestination (destConfig2, dbConfig);

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
	}
}

