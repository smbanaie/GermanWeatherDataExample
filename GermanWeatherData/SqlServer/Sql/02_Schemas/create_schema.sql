use $(dbname)
GO 

if not exists (select name from sys.schemas WHERE name = 'sample')
begin

	exec('create schema sample')
    
end

GO