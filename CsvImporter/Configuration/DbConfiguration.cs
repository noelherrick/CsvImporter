using System;

namespace CsvImporter
{
	/// <summary>
	/// Postgres-specific configuration for the destination.
	/// </summary>
    public class DbConfiguration
    {
		public string Engine { get; set;}
        public string Hostname {get;set;}
		public string Port {get;set;}
		public string Database {get;set;}
		public string Username {get;set;}
		public string Password {get;set;}
    }
}

