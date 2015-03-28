using System;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace CsvImporter.Tests
{
    [TestFixture ()]
    public class CsvSourceTests
    {
        [Test ()]
        public void TestThreeRowsInThreeRowsOutNoHeaders ()
        {
            var srcConfig = new SourceConfiguration () { HasHeaders = false };
            var reader = new StringReader ("1,1,1\n2,2,2\n3,3,3");

            var src = new CsvSource (reader, srcConfig);

            var table = src.ReadTable ();

            Assert.AreEqual(3, table.Count ());
        }

        [Test ()]
        public void TestDefaultHeaderWithNoHeaders ()
        {
            var srcConfig = new SourceConfiguration () { HasHeaders = false };
            var reader = new StringReader ("1,1,1\n2,2,2\n3,3,3");

            var src = new CsvSource (reader, srcConfig);

            var table = src.ReadTable ();

			Assert.AreEqual(3, table.ColumnNames.Count ());
			Assert.AreEqual ("A", table.ColumnNames[0]);
			Assert.AreEqual ("B", table.ColumnNames[1]);
			Assert.AreEqual ("C", table.ColumnNames[2]);
        }

        [Test ()]
        public void TestThreeRowsInTwoRowsOutWithHeaders ()
        {
            var srcConfig = new SourceConfiguration () { HasHeaders = true };
            var reader = new StringReader ("HEADER1,HEADER2,HEADER3\n2,2,2\n3,3,3");

            var src = new CsvSource (reader, srcConfig);

            var table = src.ReadTable ();

            Assert.AreEqual(2, table.Count ());
			Assert.AreEqual(3, table.ColumnNames.Length);
			Assert.AreEqual ("HEADER1", table.ColumnNames[0]);
			Assert.AreEqual ("HEADER2", table.ColumnNames[1]);
			Assert.AreEqual ("HEADER3", table.ColumnNames[2]);
        }

        [Test ()]
        public void TestHeaderAreFirstRow ()
        {
            var srcConfig = new SourceConfiguration () { HasHeaders = true };
            var reader = new StringReader ("HEADER1,HEADER2,HEADER3\n2,2,2\n3,3,3");

            var src = new CsvSource (reader, srcConfig);

            var table = src.ReadTable ();

            Assert.AreEqual(2, table.Count ());
			Assert.AreEqual(3, table.First().Count ());
			Assert.AreEqual ("2", table.ElementAt(0)[0]);
			Assert.AreEqual ("3", table.ElementAt(1)[0]);
        }
    }
}

