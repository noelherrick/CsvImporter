using System;

namespace CsvImporter.SqlTypes
{
	/// <summary>
	/// The Decimal SQL type.
	/// </summary>
    public class Decimal : SqlType
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.SqlTypes.Decimal"/> class.
		/// </summary>
        public Decimal ()
        {
            TypeCategory = TypeCategory.DECIMAL;
        }

		/// <summary>
		/// Gets or sets total length for the decimal (the total number of digits)
		/// </summary>
		/// <value>The width.</value>
        public int Width { get; set; }

		/// <summary>
		/// Gets or sets number of digits to the right of the decimal point.
		/// </summary>
		/// <value>The number of digitis to the right of the decimal point.</value>
        public int RightOfPoint { get; set; }

        protected override SqlType GetBroadestTypeWithinCategory (SqlType otherType)
        {
            if (otherType is Decimal) {
                var otherDecimal = otherType as Decimal; 
                if (Width == otherDecimal.Width && RightOfPoint == otherDecimal.RightOfPoint) {
                    return this;
                } else {
                    int biggestRight = RightOfPoint > otherDecimal.RightOfPoint ? RightOfPoint : otherDecimal.RightOfPoint;
                    int biggestWidth = Width > otherDecimal.Width ? Width : otherDecimal.Width;

                    return new Decimal () { Width = biggestWidth, RightOfPoint = biggestRight };
                }
            } else {
                throw new ArgumentException ("Parameter otherType must be Char.");
            }
        }
    }
}