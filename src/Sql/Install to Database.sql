/*
*   Created by: Jared Barneck (Rhyous)
*   Create date: 6/27/2016
*   Last Modified: 6/30/2016
*	Modified: 1/29/2018
*/

DROP FUNCTION FileTableExists
DROP FUNCTION DirectoryExists
DROP FUNCTION FileExists
DROP PROCEDURE CreateDirectory
DROP PROCEDURE CreateFile
DROP PROCEDURE CreateTextFile
DROP FUNCTION ListFiles
DROP PROCEDURE DeleteFileByStreamId
DROP PROCEDURE DeleteFileByPathLocator
DROP PROCEDURE DeleteFileByPath
DROP PROCEDURE RenameFileByStreamId
DROP PROCEDURE RenameFileByPathLocator
DROP PROCEDURE RenameFileByPath
GO

DROP ASSEMBLY FileTableFramework
GO

CREATE ASSEMBLY FileTableFramework from 'C:\Program Files\Rhyous\Db.FileTableFramework\Rhyous.Db.FileTableFramework.dll' WITH PERMISSION_SET = SAFE
--CREATE ASSEMBLY FileTableFramework from 'C:\Program Files (x86)\Rhyous\Db.FileTableFramework\Rhyous.Db.FileTableFramework.dll' WITH PERMISSION_SET = SAFE
GO

CREATE FUNCTION FileTableExists(@table NVARCHAR(100)) RETURNS BIT
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].FileTableExists;
GO
CREATE FUNCTION DirectoryExists(@table NVARCHAR(100), @path NVARCHAR(400)) RETURNS HierarchyId
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].DirectoryExists;
GO
CREATE FUNCTION FileExists(@table NVARCHAR(100), @path NVARCHAR(400)) RETURNS HierarchyId
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].FileExists;
GO
CREATE Procedure CreateDirectory(@table NVARCHAR(100), @path NVARCHAR(400))
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].CreateDirectory;
GO
CREATE Procedure CreateFile(@table NVARCHAR(100), @file NVARCHAR(400), @data VARBINARY(MAX))
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].CreateFile;
GO
CREATE Procedure CreateTextFile(@table NVARCHAR(100), @file NVARCHAR(400), @text NVARCHAR(Max))
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].CreateTextFile;
GO
CREATE FUNCTION ListFiles(@table NVARCHAR(100), @directory NVARCHAR(MAX), @recursive bit, @excludeData bit, @excludeDirectories bit)
RETURNS TABLE (stream_id uniqueidentifier
, file_stream varbinary(max)
, name nvarchar(255)
, path_locator hierarchyid
, parent_path_locator hierarchyid
, file_type nvarchar(255)
, cached_file_size bigint
, creation_time datetimeoffset(7)
, last_write_time datetimeoffset(7)
, last_access_time datetimeoffset(7)
, is_directory bit
, is_offline bit
, is_hidden bit
, is_readonly bit
, is_archive bit
, is_system bit
, is_temporary bit
)  
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].ListFiles;
GO
CREATE Procedure DeleteFileByStreamId(@table NVARCHAR(100), @stream_id UniqueIdentifier)
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].DeleteFileByStreamId;
GO
CREATE Procedure DeleteFileByPathLocator(@table NVARCHAR(100), @path_locator HierarchyId)
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].DeleteFileByPathLocator;
GO
CREATE Procedure DeleteFileByPath(@table NVARCHAR(100), @path NVARCHAR(400))
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].DeleteFileByPath;
GO
CREATE Procedure RenameFileByStreamId(@table NVARCHAR(100), @stream_id UniqueIdentifier, @newFilename NVARCHAR(255))
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].RenameFileByStreamId;
GO
CREATE Procedure RenameFileByPathLocator(@table NVARCHAR(100), @path_locator HierarchyId, @newFilename NVARCHAR(255))
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].RenameFileByPathLocator;
GO
CREATE Procedure RenameFileByPath(@table NVARCHAR(100), @path NVARCHAR(400), @newFilename NVARCHAR(255))
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].RenameFileByPath;
GO