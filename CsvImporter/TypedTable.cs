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
					var referenceRow = this.First ().Select (c => TypeInferer.InferCellDataType (c));

					foreach (var row in this) {

						var dataTypes = row.Select (c => TypeInferer.InferCellDataType (c));

						referenceRow = dataTypes.Zip (referenceRow, (x, y) => x.GetBroadestType (y));
					}

					_sqlDataTypes = referenceRow;
				}

				return _sqlDataTypes;
			}
        }


    }


}