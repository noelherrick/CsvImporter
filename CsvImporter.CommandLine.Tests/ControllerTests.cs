using System;
using NUnit.Framework;
using CsvImporter;
using CsvImporter.SqlTypes;
using CsvImporter.CommandLine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CsvImporter.CommandLine.Tests
{
	[TestFixture()]
	public class ControllerTests
	{
		private TextWriter outWriter;

		private void noOpWriteTable (DestinationConfiguration destConfig, DbConfiguration pgConfig, IRowStream stream)
		{
		}

		private IRowStream readEmptyTable (SourceConfiguration sourceConfig, CsvFileConfiguration fileConfig)
		{
			return new MemoryRowStream ("", new string[0], new SqlType[0]);
		}

		private IList<string> getDummyArgs () {
			return new List<string>{"-h", "dummy", "-d", "dummy", "-u", "dummy", "-W", "dummy", "-f", "dummy"};
		}

		[SetUp()]
		public void SetUp () {
			outWriter = new StringWriter ();

			Program.OutWriter = outWriter;
		}

		[Test()]
		public void TestTruncateIsPassed ()
		{
			var argList = getDummyArgs ();

			argList.Add ("-t");

			var controller = new Controller (outWriter, readEmptyTable, (Controller.WriteTableFunc)((x,y,z) => Assert.AreEqual(true, x.TruncateDestinationTable)));
		
			controller.Run (argList.ToArray());
			Assert.AreEqual("", outWriter.ToString());
		}

		[Test()]
		public void TestCreateTableIsPassed ()
		{
			var argList = getDummyArgs ();

			argList.Add ("-c");

			var controller = new Controller (outWriter, readEmptyTable, (Controller.WriteTableFunc)((x,y,z) => Assert.AreEqual(true, x.CreateDestinationTable)));

			controller.Run (argList.ToArray());
			Assert.AreEqual("", outWriter.ToString());
		}

		[Test()]
		public void TestHasHeadersIsPassed ()
		{
			var argList = getDummyArgs ();

			argList.Add ("-H");

			var controller = new Controller (outWriter, (Controller.ReadTableFunc)((x,y) => {Assert.AreEqual(true, x.HasHeaders); return readEmptyTable(x,y); }), noOpWriteTable);

			controller.Run (argList.ToArray());
			Assert.AreEqual("", outWriter.ToString());
		}

		[Test()]
		public void TestFilePathIsPassed ()
		{
			var argList = getDummyArgs ();

			var controller = new Controller (outWriter, (Controller.ReadTableFunc)((x,y) => {Assert.AreEqual("dummy", y.Path); return readEmptyTable(x,y); }), noOpWriteTable);

			controller.Run (argList.ToArray());
			Assert.AreEqual("", outWriter.ToString());
		}
	}
}

