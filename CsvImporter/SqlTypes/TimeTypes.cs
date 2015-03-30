using System;

namespace CsvImporter.SqlTypes
{
	/// <summary>
	/// The Date SQL datatype.
	/// </summary>
    public class Timestamp : SqlType
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.SqlTypes.Date"/> class.
		/// </summary>
        public Timestamp ()
        {
			TypeCategory = TypeCategory.TIMESTAMP;
        }

        protected override SqlType GetBroadestTypeWithinCategory (SqlType type2)
        {
            return this;
        }
    }

	public class Timestampz : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.SqlTypes.Timestampz"/> class.
		/// </summary>
		public Timestampz ()
		{
			TypeCategory = TypeCategory.TIMESTAMPZ;
		}

		protected override SqlType GetBroadestTypeWithinCategory (SqlType type2)
		{
			return this;
		}
	}

	/// <summary>
	/// Date.
	/// </summary>
	public class Date : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.SqlTypes.Date"/> class.
		/// </summary>
		public Date ()
		{
			TypeCategory = TypeCategory.DATE;
		}

		protected override SqlType GetBroadestTypeWithinCategory (SqlType type2)
		{
			return this;
		}
	}

	/// <summary>
	/// Time.
	/// </summary>
	public class Time : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.SqlTypes.Date"/> class.
		/// </summary>
		public Time ()
		{
			TypeCategory = TypeCategory.TIME;
		}

		protected override SqlType GetBroadestTypeWithinCategory (SqlType type2)
		{
			return this;
		}
	}

	/// <summary>
	/// Timez.
	/// </summary>
	public class Timez : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.SqlTypes.Timez"/> class.
		/// </summary>
		public Timez ()
		{
			TypeCategory = TypeCategory.TIMEZ;
		}

		protected override SqlType GetBroadestTypeWithinCategory (SqlType type2)
		{
			return this;
		}
	}
}

