using System;

namespace CsvImporter.SqlTypes
{
	/// <summary>
	/// The Char SQL datatype.
	/// </summary>
    public class Char : SqlType
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.SqlTypes.Char"/> class.
		/// </summary>
        public Char ()
        {
            TypeCategory = TypeCategory.CHAR;
        }

		/// <summary>
		/// Gets or sets the width of the column. This will be used with engines that use VARCHAR.
		/// </summary>
		/// <value>The width.</value>
        public int Width { get; set; }

        protected override SqlType GetBroadestTypeWithinCategory (SqlType otherType)
        {
            if (otherType is Char) {
                var castedOtherType = otherType as Char; 
                return this.Width > castedOtherType.Width ? this : castedOtherType;
            } else {
                throw new ArgumentException ("Parameter otherType must be Char.");
            }
        }
    }
}