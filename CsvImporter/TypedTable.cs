using System;
using System.Collections.Generic;
using System.Linq;
using CsvImporter.SqlTypes;
using SimpleTable;
using System.Text.RegularExpressions;

namespace CsvImporter
{
	/// <summary>
	/// A TypedTable.
	/// </summary>
	public class TypedTable : Table
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.TypedTable"/> class. Types are infered based on the data passed in
		/// </summary>
		/// <param name="columnNames">Column names.</param>
		public TypedTable (string[] columnNames)
			: base(columnNames)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.TypedTable"/> class.
		/// </summary>
		/// <param name="columnNames">Column names.</param>
		/// <param name="columnTypes">Column types.</param>
		public TypedTable (string[] columnNames, SqlType[] columnTypes)
			: base (columnNames)
		{
			_sqlDataTypes = columnTypes;
		}

		/// <summary>
		/// Gets or sets the name of the table.
		/// </summary>
		/// <value>The name.</value>
        public string Name { get; set; }

		/// <summary>
		/// Gets the headers. Each header has a name, a type, and a column position number.
		/// </summary>
		/// <value>The headers.</value>
        public IEnumerable<Header> Headers
		{
            get
			{
				int index = 0;
				return ColumnNames.Zip (DataTypes, (x, y) => new Header() { Name = x, Type = y, Index = index++ });
            }
        }

        private IEnumerable<SqlType> _sqlDataTypes;

		/// <summary>
		/// Gets the data types. If the data types are not passed in, this function reads the data in
		/// the tables to infer the data types.
		/// </summary>
		/// <value>The data types.</value>
        public IEnumerable<SqlType> DataTypes {
			get {
				if (_sqlDataTypes == null) {
					var referenceRow = this.First ().Select (c => getCellDataType (c));

					foreach (var row in this) {

						var dataTypes = row.Select (c => getCellDataType (c));

						referenceRow = dataTypes.Zip (referenceRow, (x, y) => x.GetBroadestType (y));
					}

					_sqlDataTypes = referenceRow;
				}

				return _sqlDataTypes;
			}
        }

		private Regex intRegex = new Regex(@"^-?\d+$");
		private Regex decimalRegex = new Regex(@"^-?([0-9]+)\.([0-9]+)$");
		private Regex dateRegex = new Regex (@"^([\+-]?\d{4}(?!\d{2}\b))((-?)((0[1-9]|1[0-2])(\3([12]\d|0[1-9]|3[01]))?|W([0-4]\d|5[0-2])(-?[1-7])?|(00[1-9]|0[1-9]\d|[12]\d{2}|3([0-5]\d|6[1-6])))([T\s]((([01]\d|2[0-3])((:?)[0-5]\d)?|24\:?00)([\.,]\d+(?!:))?)?(\17[0-5]\d([\.,]\d+)?)?([zZ]|([\+-])([01]\d|2[0-3]):?([0-5]\d)?)?)?)?$");

		// TODO: Decide whether this logic is best in another class. Hiding all the logic behind a property is strange
        private SqlType getCellDataType(string cell)
        {
            // TODO: We need more sophisticated DATETIME parsing. I assume .NET  has a standard format. We should try some of the most common.
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
			} else {
				return new SqlTypes.Char() { Width = cell.Length };
            }
        }
    }

	/// <summary>
	/// Data object for headers.
	/// </summary>
    public class Header
    {
        public string Name  { get; set; }
        public SqlType Type { get; set; }
		public int Index { get; set; }
    }
}