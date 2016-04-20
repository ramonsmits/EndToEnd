IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'V5')
BEGIN
	EXEC('CREATE SCHEMA V5')
END

EXEC sp_msforeachtable @command1='TRUNCATE TABLE ?',@whereand='AND Schema_Id = Schema_id(''V5'')'