using System;

namespace CsvImporter.SqlTypes
{
    public class Decimal : SqlType
    {
        public Decimal ()
        {
            TypeCategory = TypeCategory.DECIMAL;
        }

        public int Width { get; set; }
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