/*
*   Created by: Jared Barneck (Rhyous)
*   Create date: 6/27/2016
*   Last Modified: 6/30/2016
*/

DROP FUNCTION FileTableExists
DROP FUNCTION DirectoryExists
DROP FUNCTION FileExists
DROP PROCEDURE CreateDirectory
DROP PROCEDURE CreateFile
GO

DROP ASSEMBLY FileTableFramework
GO

CREATE ASSEMBLY FileTableFramework from 'C:\Program Files\Rhyous\Db.FileTableFramework\Rhyous.Db.FileTableFramework.dll' WITH PERMISSION_SET = SAFE
GO

CREATE FUNCTION FileTableExists(@table NVARCHAR(100)) RETURNS BIT
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].FileTableExists;
GO
CREATE FUNCTION DirectoryExists(@table NVARCHAR(100), @path NVARCHAR(400)) RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].DirectoryExists;
GO
CREATE FUNCTION FileExists(@table NVARCHAR(100), @path NVARCHAR(400)) RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].FileExists;
GO
CREATE Procedure CreateDirectory(@table NVARCHAR(100), @path NVARCHAR(400), @id NVARCHAR(MAX) OUTPUT)
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].CreateDirectory;
GO
CREATE Procedure CreateFile(@table NVARCHAR(100), @file NVARCHAR(400), @data VARBINARY(MAX), @id NVARCHAR(MAX) OUTPUT)
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].CreateFile;
GO
