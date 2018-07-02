DECLARE @table NVARCHAR(100) = 'OrganizationFile'

-- Test create dir
EXEC CreateDirectory @table, 'A' 

-- Test Directory Exists
SELECT dbo.DirectoryExists(@table, 'A')

-- Test create multiple dirs
EXEC CreateDirectory @table, 'A\B\C' 

-- Test create file
EXEC CreateFile @table, 'A\B\C\hw.txt', 0x48656C6C6F2C20776F726C6421 

-- Test FileExists
SELECT dbo.FileExists(@table, 'A\B\C\hw.txt')

 -- Test Create dir and file
EXEC CreateFile @table, 'A\B\C\D\hw.txt', 0x48656C6C6F2C20776F726C6421

 -- Test Create text file
EXEC CreateTextFile @table, 'A\B\C\D\hw2.txt', 'Hello, world!' -- Test Create dir and file

-- Test Create text file and delete it
EXEC CreateTextFile @table, 'A\B\C\D\deleteByStreamId.txt', 'I must be deleted by stream_id.'
DECLARE @stream_id UniqueIdentifier = (SELECT TOP 1 stream_Id from @table where NAME = 'deleteByStreamId.txt')
EXEC DeletefileByStreamId @table, @stream_id

EXEC CreateTextFile @table, 'A\B\C\D\deleteByHierarchyId.txt', 'I must be deleted path_locator.'
DECLARE @path_locator HierarchyId = (SELECT TOP 1 path_locator from @table where NAME = 'deleteByHierarchyId.txt')
EXEC DeletefileByPathLocator @table, @path_locator

EXEC CreateTextFile @table, 'A\B\C\D\deleteByPath.txt', 'I must be deleted by full path.'
EXEC DeletefileByPath @table, 'A\B\C\D\deleteByPath.txt'

-- Test Create text file and rename it
EXEC CreateTextFile @table, 'A\B\C\D\RenameByStreamId.txt', 'I must be renamed by stream_id.'
SET @stream_id = (SELECT TOP 1 stream_Id from @table where NAME = 'RenameByStreamId.txt')
EXEC RenamefileByStreamId @table, @stream_id, 'RenameByStreamId1.txt'

EXEC CreateTextFile @table, 'A\B\C\D\RenameByHierarchyId.txt', 'I must be renamed path_locator.'
SET @path_locator = (SELECT TOP 1 path_locator from @table where NAME = 'RenameByHierarchyId.txt')
EXEC RenamefileByPathLocator @table, @path_locator, 'RenameByHierarchyId1.txt'

EXEC CreateTextFile @table, 'A\B\C\D\RenameByPath.txt', 'I must be renamed by full path.'
EXEC RenamefileByPath @table, 'A\B\C\D\RenameByPath.txt', 'RenameByPath1.txt'
