using NUnit.Framework;
using System;
using CsvImporter;

namespace CsvImporter.Tests
{
    [TestFixture ()]
    public class SqlDataTypeTests
    {
        [Test ()]
        public void TestThatDecimalAndStringIsString ()
        {
            var decType = new SqlTypes.Decimal ();
            var stringType = new SqlTypes.Char ();

            var broadestType1 = decType.GetBroadestType (stringType);
            var broadestType2 = stringType.GetBroadestType (decType);

            Assert.IsTrue (broadestType1 is SqlTypes.Char);
            Assert.IsTrue (broadestType2 is SqlTypes.Char);
        }

        [Test ()]
        public void TestThatDecimal10_2AndDecimal10_3IsDecimal10_3 ()
        {
            var decType10_2 = new SqlTypes.Decimal () { Width = 10, RightOfPoint = 2};
            var decType10_3 = new SqlTypes.Decimal () { Width = 10, RightOfPoint = 3};

            var broadestType1 = (SqlTypes.Decimal)decType10_2.GetBroadestType (decType10_3);
            var broadestType2 = (SqlTypes.Decimal)decType10_3.GetBroadestType (decType10_2);

            Assert.AreEqual (3, broadestType1.RightOfPoint);
            Assert.AreEqual (3, broadestType2.RightOfPoint);
        }

        [Test ()]
        public void TestThatChar1AndChar2IsChar2 ()
        {
            var char1 = new SqlTypes.Char () { Width = 1};
            var char2 = new SqlTypes.Char () { Width = 2};

            var broadestType1 = (SqlTypes.Char)char1.GetBroadestType (char2);
            var broadestType2 = (SqlTypes.Char)char2.GetBroadestType (char1);

            Assert.AreEqual (2, broadestType1.Width);
            Assert.AreEqual (2, broadestType2.Width);
        }
    }
}

