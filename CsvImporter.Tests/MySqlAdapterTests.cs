using NUnit.Framework;
using System;

namespace CsvImporter.Tests
{
	[TestFixture ()]
	public class MySqlAdapterTests
	{
		private DbAdapterTests adapterTests;

		[TestFixtureSetUp()]
		public void TestFixtureSetUp ()
		{
			var engine = "mysql";

			adapterTests = new DbAdapterTests (engine);
		}

		[TestFixtureTearDown()]
		public void TestFixtureTearDown()
		{
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
	}
}

