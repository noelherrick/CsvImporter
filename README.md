CsvImporter
=
### (BETA)

CsvImporter is a command line tool that takes a CSV file and imports it into a SQL database.

Currently, it is only available for Postgres and MySQL, but I plan to make it work for SQL Server.

Using the SSIS functionality in SSMS, one can easily import a CSV into SQL Server. Doing so with other RDBMSes is a pain, so I created this project.

## Features

- Imports CSV into RDBMes
- Creates the table at the destination if desired
- Truncates table at the destination if desired
- Infers types
- Works with Postgres and MySQL
- Cross-platform (using .NET/Mono)

## Example

This imports a file called test1.csv to a localhost server. It will create the destination table (-c) and it will use the first row as the header names (-H).

    csv-importer.exe -d csv_importer -u csv_importer -W csv_importer \
    -e postgres -f test1.csv -c -H

## Parts

CSV importer is currently in five parts.

- CsvImporter - the library portion of the project
- CsvImporter.Tests - tests for the library
- CsvImporter.CommandLine - the frontend, command line tool
- CsvImporter.CommandLine.Tests - the tests for the front end (unit tests)
- SimpleTable - a small project for a table data structure (see that project's README.md for more details)

## License

GPLv2. Please see the LICENSE file in each project for the full version.