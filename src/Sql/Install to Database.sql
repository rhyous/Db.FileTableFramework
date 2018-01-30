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
DROP FUNCTION GetFilesInDirectory
GO

DROP ASSEMBLY FileTableFramework
GO

CREATE ASSEMBLY FileTableFramework from 'C:\Program Files (x86)\Rhyous\Db.FileTableFramework\Rhyous.Db.FileTableFramework.dll' WITH PERMISSION_SET = SAFE
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
CREATE FUNCTION GetFilesInDirectory(@table NVARCHAR(100), @directory NVARCHAR(MAX), @excludeDirectories bit)
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
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].GetFilesInDirectory;
GO