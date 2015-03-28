using System;

namespace CsvImporter.SqlTypes
{
    public class Int : SqlType
    {
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

