# Db.FileTableFramework

CLR Integration functions and stored procedures for SQL FileTables.

This allows for doing work directly against the database, which is useful especially in code, to make CRUD calls simpler and to avoid having to use the share.

## Steps to Deploy
1. Download and build the solution. There are two solutions, one with only code and one in the Install directory that has an additional Windows Installer Xml (WIX) project. If you build the WIX project as Any CPU or X64, you get a 64 bit msi.
2. Use the msi installer to install on your database server. Alternately, you can manually copy the Rhyous.Db.FileTableFramework.dll file to a location on your database server.
3. Enable CLR integration on your database.
4. Run his SQL file against your database.
[Install to Database.sql](https://github.com/rhyous/Db.FileTableFramework/blob/master/src/Sql/Install%20to%20Database.sql)

Done

## Scalar-valued Functions

- DirectoryExists
- FileExists
- FileTableExists

## Table-valued Functions

- ListFiles

## Stored Procedures

- CreateDirectory
- CreateFile
- CreateTextFile
- DeleteFileByPath
- DeleteFileByPathLocator
- DeleteFileByStreamId
- RenameFileByPath
- RenameFileByPathLocator
- RenameFileByStreamId

