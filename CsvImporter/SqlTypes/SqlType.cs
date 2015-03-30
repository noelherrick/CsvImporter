using System;

namespace CsvImporter.SqlTypes
{
	/// <summary>
	/// The base abstract class for all SQL types.
	/// </summary>
    public abstract class SqlType
    {
		/// <summary>
		/// Gets or sets the type category for the SQL type.
		/// </summary>
		/// <value>The type category.</value>
        public TypeCategory TypeCategory { get; set; }

		/// <summary>
		/// Gets the broadest type within type category.
		/// </summary>
		/// <returns>The broadest type within the type category.</returns>
		/// <param name="type2">The other type instance to compare the current instance with.</param>
        protected abstract SqlType GetBroadestTypeWithinCategory (SqlType type2);

		/// <summary>
		/// Gets the broadest type of the two compared (this instance and the one passed in).
		/// If they are of the same type category, GetBroadestTypeWithinCategory is called to compare them.
		/// </summary>
		/// <returns>The broadest type.</returns>
		/// <param name="otherType">Other type to compare this to.</param>
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

