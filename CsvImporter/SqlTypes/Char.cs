using System;

namespace CsvImporter.SqlTypes
{
    public class Char : SqlType
    {
        public Char ()
        {
            TypeCategory = TypeCategory.CHAR;
        }

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