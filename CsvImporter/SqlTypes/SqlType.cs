using System;

namespace CsvImporter.SqlTypes
{
    public abstract class SqlType
    {
        public TypeCategory TypeCategory { get; set; }

        protected abstract SqlType GetBroadestTypeWithinCategory (SqlType type2);

        public SqlType GetBroadestType (SqlType otherType)
        {
            if (this.TypeCategory < otherType.TypeCategory)
            {
                return this;
            }
            else if (this.TypeCategory > otherType.TypeCategory)
            {
                return otherType;
            }
            else
            {
                return GetBroadestTypeWithinCategory(otherType);
            }
        }
    }
}

