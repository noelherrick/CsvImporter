using System;
using System.Collections.Generic;
using System.Linq;

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
			if (this.TypeCategory == otherType.TypeCategory)
			{
				return GetBroadestTypeWithinCategory(otherType);
			}

			var narrowerType = this.TypeCategory < otherType.TypeCategory ? this.GetType() : otherType.GetType();
			var broaderType = this.TypeCategory > otherType.TypeCategory ? this.GetType() : otherType.GetType();

			if (compatibleTypes.ContainsKey(narrowerType))
			{
				if (compatibleTypes[narrowerType].Contains(broaderType))
				{
					return (SqlType)Activator.CreateInstance(broaderType);
				}

				var intersection = compatibleTypes [narrowerType].Intersect (compatibleTypes [broaderType]);

				if (intersection.Any())
				{
					var firstType = intersection.First ();

					return (SqlType)Activator.CreateInstance(firstType);
				}
			}

			return new SqlTypes.Char () { Width = 8000 };
        }

		// Char or string is implicit
		private Dictionary<Type, IList<Type>> compatibleTypes
		= new Dictionary<Type, IList<Type>> () {
			{ typeof(SqlTypes.Int), new List<Type> () { typeof(SqlTypes.Decimal) } },
			{ typeof(SqlTypes.Decimal), new List<Type> () { typeof(SqlTypes.Timez), typeof(SqlTypes.Timestamp), typeof(SqlTypes.Timestampz) } },
			{ typeof(SqlTypes.Time), new List<Type> () { typeof(SqlTypes.Timestamp), typeof(SqlTypes.Timestampz) } },
			{ typeof(SqlTypes.Date), new List<Type> () { typeof(SqlTypes.Timestamp), typeof(SqlTypes.Timestampz) } },
			{ typeof(SqlTypes.Timestamp), new List<Type> () { typeof(SqlTypes.Timestampz) } },
		};
    }
}

