using System;

namespace CsvImporter.CommandLine
{
	public class UserInputException : Exception
	{
		public UserInputException (string message)
			: base (message)
		{
		}
	}
}

