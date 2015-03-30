using System;

namespace CsvImporter.CommandLine
{
	/// <summary>
	/// User input exception. Used to indicate bad arguments.
	/// </summary>
	public class UserInputException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CsvImporter.CommandLine.UserInputException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		public UserInputException (string message)
			: base (message)
		{
		}
	}
}

