using System;

namespace CsvImporter.SqlTypes
{
    public class Date : SqlType
    {
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

