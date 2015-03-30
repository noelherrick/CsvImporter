using System;

namespace CsvImporter.SqlTypes
{
	/// <summary>
	/// The Int SQL type.
	/// </summary>
    public class Int : SqlType
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.SqlTypes.Int"/> class.
		/// </summary>
        public Int ()
        {
            TypeCategory = TypeCategory.INTEGER;
        }

        protected override SqlType GetBroadestTypeWithinCategory (SqlType type2)
        {
            return this;
        }
    }
}

