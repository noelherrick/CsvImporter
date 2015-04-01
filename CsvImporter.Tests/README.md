CsvImporter
=
## Testing

This suite has both unit and integration tests. For some of the tests, you will need a development database instance. You will need to run the following DDL and then modify app.config with your hostname/database/username. To run all the tests, you will need a development instance of both Postgres and MySQL.

### DDL

For both MySQL and Postgres, you will need to run:

    create database csv_importer;

You can replace csv_importer with your own database name.

### DB-specific DDL

You will also need to create a user in your system. You can replace csv_importer with your username and password.

#### Postgres

    create user csv_importer with password 'csv_importer';
    alter database csv_importer owner to csv_importer;

#### MySQL

	create user csv_importer@'localhost' identified by 'csv_importer';
	grant all on csv_importer.* to csv_importer@'localhost';

### App.config

Each engine has its own keys. Prefix the engine name and an underscore (postgres_, mysql_) to these keys:

hostname
database
username
password

The tests assume you are using the standard port for your development instance.