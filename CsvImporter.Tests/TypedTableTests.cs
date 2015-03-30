using System;
using NUnit.Framework;
using SimpleTable;
using System.Linq;

namespace CsvImporter.Tests
{
	[TestFixture()]
	public class TypedTableTests
	{
		[Test()]
		public void TestThatDateIsDetected ()
		{
			var table = new TypedTable (new string[] {"a"});

			table.Add(new string[] {"1970-01-01"});

			Assert.IsTrue(table.Headers.First().Type is SqlTypes.Date);
		}

		[Test()]
		public void TestThatDateIsNotDetectedForBadDate ()
		{
			var table = new TypedTable (new string[] {"a"});

			table.Add(new string[] {"1970-13-01"});

			Assert.IsFalse(table.Headers.First().Type is SqlTypes.Date);
			Assert.IsTrue(table.Headers.First().Type is SqlTypes.Char);
		}

		[Test()]
		public void TestThatDateTimeIsDetected ()
		{
			var table = new TypedTable (new string[] {"a"});

			table.Add(new string[] {"1970-01-01 00:00:01"});

			Assert.IsTrue(table.Headers.First().Type is SqlTypes.Date);
		}

		[Test()]
		public void TestThatDateTimeIsNotDetectedForBadDateTime ()
		{
			var table = new TypedTable (new string[] {"a"});

			table.Add(new string[] {"1970-01-01 00:00:61"});

			Assert.IsFalse(table.Headers.First().Type is SqlTypes.Date);
			Assert.IsTrue(table.Headers.First().Type is SqlTypes.Char);
		}

		[Test()]
		public void TestThatIntIsDetected ()
		{
			var table = new TypedTable (new string[] {"a"});

			table.Add(new string[] {"1331"});

			Assert.IsTrue(table.Headers.First().Type is SqlTypes.Int);
		}

		[Test()]
		public void TestThatIntIsDetectedWhenNegative ()
		{
			var table = new TypedTable (new string[] {"a"});

			table.Add(new string[] {"-1331"});

			Assert.IsTrue(table.Headers.First().Type is SqlTypes.Int);
		}

		[Test()]
		public void TestThatIntIsNotDetectedForBadInt ()
		{
			var table = new TypedTable (new string[] {"a"});

			table.Add(new string[] {"1331A"});

			Assert.IsFalse(table.Headers.First().Type is SqlTypes.Int);
			Assert.IsTrue(table.Headers.First().Type is SqlTypes.Char);
		}

		[Test()]
		public void TestThatDecimalIsDetected ()
		{
			var table = new TypedTable (new string[] {"a"});

			table.Add(new string[] {"3.14"});

			var thing = table.Headers.First ().Type;

			Assert.IsTrue(table.Headers.First().Type is SqlTypes.Decimal);

			var decType = table.Headers.First().Type as SqlTypes.Decimal;

			Assert.AreEqual(2, decType.RightOfPoint);
			Assert.AreEqual(3, decType.Width);
		}

		[Test()]
		public void TestThatDecimalIsDetectedWhenNegative ()
		{
			var table = new TypedTable (new string[] {"a"});

			table.Add(new string[] {"-3.14"});

			var thing = table.Headers.First ().Type;

			Assert.IsTrue(table.Headers.First().Type is SqlTypes.Decimal);

			var decType = table.Headers.First().Type as SqlTypes.Decimal;

			Assert.AreEqual(2, decType.RightOfPoint);
			Assert.AreEqual(3, decType.Width);
		}

		[Test()]
		public void TestThatDecimalIsNotDetectedForBadDecimal ()
		{
			var table = new TypedTable (new string[] {"a"});

			table.Add(new string[] {"3.14.3"});

			Assert.IsFalse(table.Headers.First().Type is SqlTypes.Decimal);
			Assert.IsTrue(table.Headers.First().Type is SqlTypes.Char);
		}

		[Test()]
		public void TestThatChar5IsDetectedForStringOfLength5 ()
		{
			var table = new TypedTable (new string[] {"a"});

			table.Add(new string[] {"abcde"});
			table.Add(new string[] {"abcd"});

			Assert.IsTrue(table.Headers.First().Type is SqlTypes.Char);

			var specificType = table.Headers.First ().Type as SqlTypes.Char;

			Assert.AreEqual(5, specificType.Width);
		}
	}
}

