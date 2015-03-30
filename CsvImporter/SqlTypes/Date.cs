using System;

namespace CsvImporter.SqlTypes
{
	/// <summary>
	/// The Date SQL datatype.
	/// </summary>
    public class Date : SqlType
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.SqlTypes.Date"/> class.
		/// </summary>
        public Date ()
        {
            TypeCategory = TypeCategory.DATE;
        }

        protected override SqlType GetBroadestTypeWithinCategory (SqlType type2)
        {
            return this;
        }
    }
}

