using System;
using System.Linq;
using CsvImporter;
using CsvImporter.SqlTypes;
using Dapper;
using System.Configuration;
using System.Data.Common;
using NUnit.Framework;
using System.Collections.Generic;

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
			var stream = new MemoryRowStream (adapter.GetEngineSpecificName("TestTableIsCreated"), new string[] {"a","b","c"}, types);

			try
			{
				var destConfig = new DestinationConfiguration (){ CreateDestinationTable=true};

				var pgDestination = new DbDestination (destConfig, dbConfig);

				pgDestination.WriteStream(stream);

				var result = conn.Query("select * from information_schema.tables where table_name = @Name", new {Name=stream.Name});

				Assert.AreEqual(1, result.Count());
			}
			finally
			{
				conn.Execute ("drop table " + stream.Name );
			}
		}
			
		public void TestRowsAreAdded ()
		{
			var rows = new List<string[]> ();

			rows.Add (new string[] {"1","1","1"});
			rows.Add (new string[] {"2","2","2"});
			rows.Add (new string[] {"3","3","3"});

			var source = new MemoryStreamSource(adapter.GetEngineSpecificName("TestRowsAreAdded"), new string[] {"a","b","c"}, rows.ToArray());
			var stream = new RowStream (source);

			try
			{
				var destConfig = new DestinationConfiguration (){ CreateDestinationTable=true};


				var pgDestination = new DbDestination (destConfig, dbConfig);

				pgDestination.WriteStream (stream);

				var result = conn.Query<int>("select count(*) as a from " + stream.Name).First();

				Assert.AreEqual(3, result);
			}
			finally
			{
				conn.Execute ("drop table " + stream.Name );
			}
		}

		public void TestTruncationWorks ()
		{
			var name = "test_truncation_works";

			var rows1 = new List<string[]> ();

			rows1.Add (new string[] {"1","1","1"});
			rows1.Add (new string[] {"2","2","2"});
			rows1.Add (new string[] {"3","3","3"});

			var source1 = new MemoryStreamSource(name, new string[] {"a","b","c"}, rows1.ToArray());
			var stream1 = new RowStream (source1, 1); // We want to trigger the batch logic

			var rows2 = new List<string[]> ();

			rows2.Add (new string[] {"4","4","4"});
			rows2.Add (new string[] {"5","5","5"});
			rows2.Add (new string[] {"6","6","6"});

			var source2 = new MemoryStreamSource(name, new string[] {"a","b","c"}, rows2.ToArray());
			var stream2 = new RowStream (source2, 1);

			try
			{
				// Let's create a table
				var destConfig1 = new DestinationConfiguration (){ CreateDestinationTable=true};

				var pgDestination1 = new DbDestination (destConfig1, dbConfig);

				pgDestination1.WriteStream (stream1);

				var result1 = conn.Query<int>("select count(*) as a from " + stream1.Name + " where a in (1,2,3)").First();

				Assert.AreEqual(3, result1);

				// Okay, now let's truncate to see if that works

				var destConfig2 = new DestinationConfiguration (){ TruncateDestinationTable=true};

				var pgDestination2 = new DbDestination (destConfig2, dbConfig);

				pgDestination2.WriteStream (stream2);

				// We don't want any old rows
				var result2 = conn.Query<int>("select count(*) as a from " + stream1.Name + " where a in (1,2,3)").First();

				Assert.AreEqual(0, result2);

				// We do want the new rows

				var result3 = conn.Query<int>("select count(*) as a from " + stream1.Name + " where a in (4,5,6)").First();

				Assert.AreEqual(3, result3);
			}
			finally
			{
				conn.Execute ("drop table " + stream1.Name );
			}
		}
	}
}

