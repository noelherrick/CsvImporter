using System;
using System.Collections.Generic;
using System.Linq;
using CsvImporter.SqlTypes;
using SimpleTable;

namespace CsvImporter
{
	public class TypedTable : Table
    {
		public TypedTable (string[] columnNames)
			: base(columnNames)
		{

		}

		public TypedTable (string[] columnNames, SqlType[] columnTypes)
			: base (columnNames)
		{
			_sqlDataTypes = columnTypes;
		}

        public string Name { get; set; }
        public IEnumerable<Header> Headers
		{
            get
			{
				int index = 0;
				return ColumnNames.Zip (DataTypes, (x, y) => new Header() { Name = x, Type = y, Index = index++ });
            }
        }

        private IEnumerable<SqlType> _sqlDataTypes;

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
		// TODO: Decide whether this logic is best in another class. Hiding all the logic behind a properties is strange
        private SqlType getCellDataType(string cell)
        {
            // TODO: We need more sophisticated DATETIME parsing. I assume .NET  has a standard format. We should try some of the most common.
            // TODO: We also need to talk about warnings. Of course, some cells are actually strings, but perhaps there should be warnings on type errors.
            int outInt;
            DateTime outDate;
            decimal outDecimal;

            if (DateTime.TryParse (cell, out outDate)) {
                return new SqlTypes.Date();
            } else if (int.TryParse (cell, out outInt)) {
                return new SqlTypes.Int();
            } else if (decimal.TryParse (cell, out outDecimal)) {
                return new SqlTypes.Decimal () { Width = 10, RightOfPoint = 2 };
            } else {
                return new SqlTypes.Char();
            }
        }
    }

    public class Header
    {
        public string Name  { get; set; }
        public SqlType Type { get; set; }
		public int Index { get; set; }
    }
}