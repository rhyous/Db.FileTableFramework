DECLARE @table NVARCHAR(100) = 'OrganizationFile'

-- Test create dir
EXEC CreateDirectory @table, 'A' 

-- Test create multiple dirs
EXEC CreateDirectory @table, 'A\B\C' 

-- Test create file
EXEC CreateFile @table, 'A\B\C\hw.txt', 0x48656C6C6F2C20776F726C6421 

 -- Test Create dir and file
EXEC CreateFile @table, 'A\B\C\D\hw.txt', 0x48656C6C6F2C20776F726C6421

 -- Test Create text file
EXEC CreateTextFile @table, 'A\B\C\D\hw2.txt', 'Hello, world!' -- Test Create dir and file

SELECT * FROM GetFilesInDirectory(@table, 'A\B\C\', 0) -- 0 = include directories
SELECT * FROM GetFilesInDirectory(@table, 'A\B\C\', 1) -- 1 = exclude directories