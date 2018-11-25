IF DB_ID('$(dbname)') IS NULL
BEGIN
    CREATE DATABASE $(dbname)
END
GO