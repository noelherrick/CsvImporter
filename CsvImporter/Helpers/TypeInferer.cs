using System;
using System.Text.RegularExpressions;
using CsvImporter.SqlTypes;

namespace CsvImporter
{
	public class TypeInferer
	{
		private static Regex intRegex = new Regex(@"^-?\d+$");
		private static Regex decimalRegex = new Regex(@"^-?([0-9]+)\.([0-9]+)$");
		private static Regex dateRegex = new Regex(@"^(?<year>[0-9]{4})-?(?<month>1[0-2]|0[1-9])-?(?<day>3[01]|0[1-9]|[12][0-9])$");
		private static Regex timeRegex = new Regex (@"^(2[0-3]|[01][0-9]):?([0-5][0-9]):?([0-5][0-9])$");
		private static Regex timezRegex = new Regex(@"^(?<hour>2[0-3]|[01][0-9]):?(?<minute>[0-5][0-9]):?(?<second>[0-5][0-9])(?<timezone>Z|[+-](?:2[0-3]|[01][0-9])(?::?(?:[0-5][0-9]))?)$");
		private static Regex timestampRegex = new Regex (@"^(?<year>[0-9]{4})-?(?<month>1[0-2]|0[1-9])-?(?<day>3[01]|0[1-9]|[12][0-9]) (?<hour>2[0-3]|[01][0-9]):?(?<minute>[0-5][0-9]):?(?<second>[0-5][0-9])$");
		private static Regex timestampzRegex = new Regex (@"^([\+-]?\d{4}(?!\d{2}\b))((-?)((0[1-9]|1[0-2])(\3([12]\d|0[1-9]|3[01]))?|W([0-4]\d|5[0-2])(-?[1-7])?|(00[1-9]|0[1-9]\d|[12]\d{2}|3([0-5]\d|6[1-6])))([T\s]((([01]\d|2[0-3])((:?)[0-5]\d)?|24\:?00)([\.,]\d+(?!:))?)?(\17[0-5]\d([\.,]\d+)?)?([zZ]|([\+-])([01]\d|2[0-3]):?([0-5]\d)?)?)?)?$");

		public static SqlType InferCellDataType(string cell)
		{
			// TODO: If the type is later converted to a string, we need to know the length of all the previous data
			// TODO: We also need to talk about warnings. Of course, some cells are actually strings, but perhaps there should be warnings on type errors.
			MatchCollection matches;

			if (intRegex.IsMatch(cell)) {
				return new SqlTypes.Int();
			} else if ((matches = decimalRegex.Matches(cell)).Count == 1) {
				int rightOfPoint = matches [0].Groups[2].Length;
				int width = matches [0].Groups[1].Length + rightOfPoint;

				return new SqlTypes.Decimal () { Width = width, RightOfPoint = rightOfPoint };
			} else if (dateRegex.IsMatch(cell)) {
				return new SqlTypes.Date();
			} else if (timeRegex.IsMatch(cell)) {
				return new SqlTypes.Time();
			} else if (timezRegex.IsMatch(cell)) {
				return new SqlTypes.Timez();
			} else if (timestampRegex.IsMatch(cell)) {
				return new SqlTypes.Timestamp();
			} else if (timestampzRegex.IsMatch(cell)) {
				return new SqlTypes.Timestampz();
			} else {
				return new SqlTypes.Char() { Width = cell.Length };
			}
		}
	}
}

