IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'V5')
BEGIN
	EXEC('CREATE SCHEMA V5')
END

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'V6')
BEGIN
	EXEC('CREATE SCHEMA V6')
END

EXEC sp_msforeachtable @command1='TRUNCATE TABLE ?',@whereand='AND Schema_Id IN (Schema_id(''V5''),Schema_id(''V6''))'