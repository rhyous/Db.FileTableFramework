/*
*	Created by: Jared Barneck (Rhyous)
*	Create date: 6/27/2016
*   Last Modified: 6/27/2016
*/

DROP FUNCTION FileTableExists
DROP FUNCTION PathExists
GO
DROP ASSEMBLY FileTableFramework
GO
CREATE ASSEMBLY FileTableFramework from 'C:\Program Files\Rhyous\Db.FileTables\Rhyous.Db.FileTableFramework.dll' WITH PERMISSION_SET = SAFE
GO
CREATE FUNCTION FileTableExists(@table NVARCHAR(100)) RETURNS BIT
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableExtensions].FileTableExists;
GO
CREATE FUNCTION PathExists(@table NVARCHAR(100), @path NVARCHAR(400)) RETURNS HierarchyId
AS EXTERNAL NAME FileTableFramework.[Rhyous.Db.FileTableFramework.FileTableFunctions].PathExists;
GO